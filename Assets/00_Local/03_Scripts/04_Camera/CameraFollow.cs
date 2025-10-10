using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    // --- �C���X�y�N�^�[�ݒ�p�����[�^ ---
    public GameObject playerObject;      // �ǔ��I�u�W�F�N�g

    // PlayerController�̃C���X�^���X�iInputHandler�A�N�Z�X�p�j
    [SerializeField] private PlayerController _playerController;

    [Header("Rotation")]
    [Tooltip("�}�E�XX, Y�̉�]���x")]
    public Vector2 rotationSpeed = new Vector2(5f, 5f);
    [Tooltip("�J�������㉺�ɓ����ő�p�x")]
    public float pitchMax = 80f;
    public float pitchMin = -20f;
    [Tooltip("�Q�[���p�b�h�̃X�e�B�b�N��]�W��")]
    public Vector2 stickRotationMultiplier = new Vector2(200f, 200f); // �X�e�B�b�N���͂̓}�E�X���傫������

    [Header("Zoom")]
    public float zoomDistance = 5f; // �J�����ƃv���C���[�̌��݂̋���

    public Vector3 offset;

    [Header("Smoothness")]
    [Tooltip("�ڕW�ɓ��B����܂łɂ����邨���悻�̎��� (�b)")]
    public float followTime = 0.1f;

    // --- ������� ---
    private Vector3 _followVelocity = Vector3.zero; // SmoothDamp�p
    private Vector3 _lastMousePosition;
    private float _currentPitch; // ���݂̏㉺�p�x

    void Start()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player Object���ݒ肳��Ă��܂���BInspector�Őݒ肵�Ă��������B");
            return;
        }

        _currentPitch = transform.eulerAngles.x;
        _lastMousePosition = Input.mousePosition;

        // �v���C���[�̐^���������悤�ɏ�����
        transform.LookAt(playerObject.transform.position);
    }

    // �J�����̒Ǐ]�����́A�����I�� LateUpdate �̍Ō�Ɏ��s�����ׂ�
    public void UpdateCamera()
    {
        if (playerObject == null) return;

        Cursor.lockState = CursorLockMode.Locked;

        // �}�E�X�J�[�\�����\���ɂ���
        Cursor.visible = false;
        // 1. ��]���� (Rotate)
        Rotate();

        // 3. �Ǐ]���� (�ʒu�̍X�V)
        FollowTarget();
    }


    void Rotate()
    {
        // --- 1. ���͒l�̎擾 ---
        Vector3 rotationInput = Vector3.zero;
        bool isInputSystemLookActive = false;

        // PlayerController�o�R��Input System��Look�A�N�V�����̒l��ǂݎ��
        if (_playerController != null)
        {
            Vector2 lookValue = _playerController.InputHandler.Look.ReadValue<Vector2>();

            // Input System�Ń}�E�X�J�[�\�������b�N����Ă����Ԃł́A
            // Look�A�N�V�����ɂ̓}�E�X�̃f���^�l������܂��B

            // �}�E�X�E�N���b�N��������Ă���i�}�E�X����j
            if (Input.GetMouseButton(1))
            {
                // �}�E�X��Look���́i�f���^�j�Ƀ}�E�X���x��K�p
                // Time.deltaTime�͕s�v�iLook��Delta�̓t���[�����ƂɃ��Z�b�g����邽�߁j
                rotationInput.x = lookValue.x * rotationSpeed.x;
                rotationInput.y = lookValue.y * rotationSpeed.y;
                isInputSystemLookActive = true;
            }
            else // �Q�[���p�b�h�̑���i�E�X�e�B�b�N�j
            {
                // �X�e�B�b�N���͂̏ꍇ�ATime.deltaTime���|���ĉ�]���x�𒲐�
                // Y���͑��슴�ɍ��킹�ĕ����𔽓]������ (�X�e�B�b�N�ƃ}�E�X�̊��x���t�̏ꍇ�����邽��)
                rotationInput.x = lookValue.x * stickRotationMultiplier.x * Time.deltaTime;
                rotationInput.y = -lookValue.y * stickRotationMultiplier.y * Time.deltaTime;

                // �X�e�B�b�N���킸���ł������Ă��邩�`�F�b�N
                if (lookValue.magnitude > 0.001f)
                {
                    isInputSystemLookActive = true;
                }
            }
        }

        // �Â� Input.mousePosition �̓J�[�\�����b�N���ɂ͋@�\���Ȃ����ߍ폜���܂��B
        // _lastMousePosition �̍X�V���s�v�ɂȂ�܂��B


        // --- 2. ��]�̓K�p ---
        // �}�E�X�E�N���b�N�A�܂��̓X�e�B�b�N���삪����ꍇ
        if (Input.GetMouseButton(1) || isInputSystemLookActive)
        {
            // Yaw (Y����]: ���E)
            float yawChange = rotationInput.x;

            // Pitch (X����]: �㉺)
            float pitchChange = rotationInput.y;

            // ���E�̉�]�̓v���C���[�𒆐S�ɍs��
            transform.RotateAround(playerObject.transform.position, Vector3.up, yawChange);

            // �㉺�̉�]�p�x���v�Z���A������������
            _currentPitch += pitchChange;
            _currentPitch = Mathf.Clamp(_currentPitch, pitchMin, pitchMax);

            // �J�����̌��݂̉�]���Đݒ� (Y���͂��̂܂܁AX���̓N�����v�����l)
            Quaternion currentYRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Quaternion targetRotation = currentYRotation * Quaternion.Euler(_currentPitch, 0, 0);

            // �v���C���[�̒��S�ʒu����A�v�Z������]�Ƌ������g���ăJ�����ʒu���v�Z
            Vector3 targetPosition = playerObject.transform.position + targetRotation * Vector3.back * zoomDistance;

            // �J�����̈ʒu�Ɖ�]�𑦎��X�V
            transform.position = targetPosition;
            transform.LookAt(playerObject.transform.position + offset);
        }
    }
    void FollowTarget()
    {
        // �J�����̌��݂̉�]���Đݒ� (Y���͂��̂܂܁AX���̓N�����v�����l)
        Quaternion currentYRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        Quaternion targetRotation = currentYRotation * Quaternion.Euler(_currentPitch, 0, 0);

        // �v���C���[�̒��S�ʒu����A�v�Z������]�Ƌ������g���ăJ�����ʒu���v�Z
        Vector3 targetPosition = playerObject.transform.position + targetRotation * Vector3.back * zoomDistance;

        // �J�����̈ʒu�Ɖ�]�𑦎��X�V
        transform.position = targetPosition;

        // SmoothDamp�Ŋ��炩�ɖڕW�ʒu�ɋ߂Â���
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _followVelocity,
            followTime
        );

        // �J�������v���C���[�̒��S�������Ă��邱�Ƃ�ۏ�
        transform.LookAt(playerObject.transform.position + offset);
    }
}