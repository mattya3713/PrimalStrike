// PlayerController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static MainInput; // MainInputクラスの直接利用を許可
using PlayerState; // PlayerState名前空間を使用

public class PlayerController : MonoBehaviour
{
    // 所有者T: PlayerController, キーK: PlayerStateKey
    private StateMachine<PlayerController, PlayerStateKey> _stateMachine;

    private MainInput _inputActions;
    public BasicActions InputHandler { get; private set; } // BasicActionsを公開

    [Header("-----待機ステート-----")]
    [SerializeField] private float _idleAnimationStartTime = 3f;
    public float idleAnimationStartTime{ get; private set; }

    [Header("-----歩きステート-----")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("-----走りステート-----")]
    [SerializeField] private float jumpPower = 7.0f; // ジャンプ力を公開

    // CharacterControllerをプライベートとパブリックで保持（プロパティ経由でアクセス）
    private CharacterController _characterController;
    public CharacterController CharacterController => _characterController;
    public float MoveSpeed => moveSpeed;
    public float JumpPower => jumpPower; // Jumpステートで使用するため公開

    void Awake()
    {
        _inputActions = new MainInput();
        InputHandler = _inputActions.Basic;

        // 1. ステートマシンの初期化
        _stateMachine = new StateMachine<PlayerController, PlayerStateKey>(this);

        // 各ステートの登録
        _stateMachine.AddState(PlayerStateKey.Idle, new Idle(this));
        _stateMachine.AddState(PlayerStateKey.Run, new Run(this));
        _stateMachine.AddState(PlayerStateKey.Attack, new Attack(this));
        _stateMachine.AddState(PlayerStateKey.Jump, new Jump(this)); // Jumpステートを追加

        _characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        _inputActions.Enable();
        InputHandler.Enable();

        // ジャンプ入力イベントの購読（Start()やAwake()で一度だけ行う）
        // ※ JumpアクションがBasicマップ内にあると仮定
        InputHandler.Confirm.performed += OnJumpInput; // 今回はConfirmをジャンプにも使うと仮定

        // 3. 初期ステートを設定
        _stateMachine.SwitchState(PlayerStateKey.Idle);
    }

    void Update()
    {
        _stateMachine.Update(); 

        HandleMovementAndTransition();
    }

    void LateUpdate()
    {
        _stateMachine.LateUpdate(); 

        HandleMovementAndTransition();
    }

    private void HandleMovementAndTransition()
    {
        // Z方向も使う3D空間での移動ベクトルに変換
        Vector2 input = InputHandler.Move.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, 0, input.y);

        // 1. 移動の有無を判断
        bool isMoving = movement.magnitude > 0.1f;

        // 2. 物理的な移動処理（Idle/Runステートでのみ、ここで直接実行）
        // Jump/Attackステートではステート内で重力やアニメーション考慮の移動を行うため、除外する
        bool isGroundState = _stateMachine.CurrentState is Idle || _stateMachine.CurrentState is Run;

        if (isGroundState)
        {
            // 地面にいる間の移動処理
            CharacterController.Move(movement * moveSpeed * Time.deltaTime);

            // 遷移判断
            if (isMoving && _stateMachine.CurrentState is Idle)
            {
                SwitchState(PlayerStateKey.Run); // Idle → Run
            }
            else if (!isMoving && _stateMachine.CurrentState is Run)
            {
                SwitchState(PlayerStateKey.Idle); // Run → Idle
            }
        }
        // Jump/Attackステートでは、このメソッドは遷移判定のみに使うか、
        // 処理をステートクラスのUpdate()に完全に委譲します。
    }

    // --- イベント駆動のジャンプ入力処理 ---
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // 現在、IdleまたはRunステートであり、かつ地面にいる場合のみジャンプを許可
        bool isGroundState = _stateMachine.CurrentState is Idle || _stateMachine.CurrentState is Run;

        if (CharacterController.isGrounded && isGroundState)
        {
            SwitchState(PlayerStateKey.Jump);
        }
    }

    // 外部からステートを切り替えるためのヘルパーメソッド
    public void SwitchState(PlayerStateKey key)
    {
        _stateMachine.SwitchState(key);
    }

    void OnDestroy()
    {
        // 忘れずにInput Systemを無効化（メモリ管理のために重要！）
        // 購読解除もここで行うのが安全
        InputHandler.Confirm.performed -= OnJumpInput;

        _inputActions.Dispose();
    }
}