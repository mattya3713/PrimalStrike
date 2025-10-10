using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    // --- インスペクター設定パラメータ ---
    public GameObject playerObject;      // 追尾オブジェクト

    // PlayerControllerのインスタンス（InputHandlerアクセス用）
    [SerializeField] private PlayerController _playerController;

    [Header("Rotation")]
    [Tooltip("マウスX, Yの回転速度")]
    public Vector2 rotationSpeed = new Vector2(5f, 5f);
    [Tooltip("カメラが上下に動く最大角度")]
    public float pitchMax = 80f;
    public float pitchMin = -20f;
    [Tooltip("ゲームパッドのスティック回転係数")]
    public Vector2 stickRotationMultiplier = new Vector2(200f, 200f); // スティック入力はマウスより大きく調整

    [Header("Zoom")]
    public float zoomDistance = 5f; // カメラとプレイヤーの現在の距離

    public Vector3 offset;

    [Header("Smoothness")]
    [Tooltip("目標に到達するまでにかかるおおよその時間 (秒)")]
    public float followTime = 0.1f;

    // --- 内部状態 ---
    private Vector3 _followVelocity = Vector3.zero; // SmoothDamp用
    private Vector3 _lastMousePosition;
    private float _currentPitch; // 現在の上下角度

    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player Objectが設定されていません。Inspectorで設定してください。");
            return;
        }

        _currentPitch = transform.eulerAngles.x;
        _lastMousePosition = Input.mousePosition;

        // プレイヤーの真後ろを向くように初期化
        transform.LookAt(playerObject.transform.position);
    }

    // カメラの追従処理は、明示的に LateUpdate の最後に実行されるべき
    public void UpdateCamera()
    {
        if (playerObject == null) return;

        Cursor.lockState = CursorLockMode.Locked;

        // マウスカーソルを非表示にする
        Cursor.visible = false;
        // 1. 回転処理 (Rotate)
        Rotate();

        // 3. 追従処理 (位置の更新)
        FollowTarget();
    }


    void Rotate()
    {
        // --- 1. 入力値の取得 ---
        Vector3 rotationInput = Vector3.zero;
        bool isInputSystemLookActive = false;

        // PlayerController経由でInput SystemのLookアクションの値を読み取る
        if (_playerController != null)
        {
            Vector2 lookValue = _playerController.InputHandler.Look.ReadValue<Vector2>();

            // Input Systemでマウスカーソルがロックされている状態では、
            // Lookアクションにはマウスのデルタ値が入ります。

            // マウス右クリックが押されている（マウス操作）
            if (Input.GetMouseButton(1))
            {
                // マウスのLook入力（デルタ）にマウス感度を適用
                // Time.deltaTimeは不要（LookのDeltaはフレームごとにリセットされるため）
                rotationInput.x = lookValue.x * rotationSpeed.x;
                rotationInput.y = lookValue.y * rotationSpeed.y;
                isInputSystemLookActive = true;
            }
            else // ゲームパッドの操作（右スティック）
            {
                // スティック入力の場合、Time.deltaTimeを掛けて回転速度を調整
                // Y軸は操作感に合わせて符号を反転させる (スティックとマウスの感度が逆の場合があるため)
                rotationInput.x = lookValue.x * stickRotationMultiplier.x * Time.deltaTime;
                rotationInput.y = -lookValue.y * stickRotationMultiplier.y * Time.deltaTime;

                // スティックがわずかでも動いているかチェック
                if (lookValue.magnitude > 0.001f)
                {
                    isInputSystemLookActive = true;
                }
            }
        }

        // 古い Input.mousePosition はカーソルロック時には機能しないため削除します。
        // _lastMousePosition の更新も不要になります。


        // --- 2. 回転の適用 ---
        // マウス右クリック、またはスティック操作がある場合
        if (Input.GetMouseButton(1) || isInputSystemLookActive)
        {
            // Yaw (Y軸回転: 左右)
            float yawChange = rotationInput.x;

            // Pitch (X軸回転: 上下)
            float pitchChange = rotationInput.y;

            // 左右の回転はプレイヤーを中心に行う
            transform.RotateAround(playerObject.transform.position, Vector3.up, yawChange);

            // 上下の回転角度を計算し、制限をかける
            _currentPitch += pitchChange;
            _currentPitch = Mathf.Clamp(_currentPitch, pitchMin, pitchMax);

            // カメラの現在の回転を再設定 (Y軸はそのまま、X軸はクランプした値)
            Quaternion currentYRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Quaternion targetRotation = currentYRotation * Quaternion.Euler(_currentPitch, 0, 0);

            // プレイヤーの中心位置から、計算した回転と距離を使ってカメラ位置を計算
            Vector3 targetPosition = playerObject.transform.position + targetRotation * Vector3.back * zoomDistance;

            // カメラの位置と回転を即時更新
            transform.position = targetPosition;
            transform.LookAt(playerObject.transform.position + offset);
        }
    }
    void FollowTarget()
    {
        // カメラの現在の回転を再設定 (Y軸はそのまま、X軸はクランプした値)
        Quaternion currentYRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        Quaternion targetRotation = currentYRotation * Quaternion.Euler(_currentPitch, 0, 0);

        // プレイヤーの中心位置から、計算した回転と距離を使ってカメラ位置を計算
        Vector3 targetPosition = playerObject.transform.position + targetRotation * Vector3.back * zoomDistance;

        // カメラの位置と回転を即時更新
        transform.position = targetPosition;

        // SmoothDampで滑らかに目標位置に近づける
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _followVelocity,
            followTime
        );

        // カメラがプレイヤーの中心を向いていることを保証
        transform.LookAt(playerObject.transform.position + offset);
    }
}