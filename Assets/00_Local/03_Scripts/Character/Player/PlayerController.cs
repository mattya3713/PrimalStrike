// PlayerController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static MainInput; // MainInput�N���X�̒��ڗ��p������
using PlayerState; // PlayerState���O��Ԃ��g�p

public class PlayerController : MonoBehaviour
{
    // ���L��T: PlayerController, �L�[K: PlayerStateKey
    private StateMachine<PlayerController, PlayerStateKey> _stateMachine;

    private MainInput _inputActions;
    public BasicActions InputHandler { get; private set; } // BasicActions�����J

    [Header("-----�ҋ@�X�e�[�g-----")]
    [SerializeField] private float _idleAnimationStartTime = 3f;
    public float idleAnimationStartTime{ get; private set; }

    [Header("-----�����X�e�[�g-----")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("-----����X�e�[�g-----")]
    [SerializeField] private float jumpPower = 7.0f; // �W�����v�͂����J

    // CharacterController���v���C�x�[�g�ƃp�u���b�N�ŕێ��i�v���p�e�B�o�R�ŃA�N�Z�X�j
    private CharacterController _characterController;
    public CharacterController CharacterController => _characterController;
    public float MoveSpeed => moveSpeed;
    public float JumpPower => jumpPower; // Jump�X�e�[�g�Ŏg�p���邽�ߌ��J

    void Awake()
    {
        _inputActions = new MainInput();
        InputHandler = _inputActions.Basic;

        // 1. �X�e�[�g�}�V���̏�����
        _stateMachine = new StateMachine<PlayerController, PlayerStateKey>(this);

        // �e�X�e�[�g�̓o�^
        _stateMachine.AddState(PlayerStateKey.Idle, new Idle(this));
        _stateMachine.AddState(PlayerStateKey.Run, new Run(this));
        _stateMachine.AddState(PlayerStateKey.Attack, new Attack(this));
        _stateMachine.AddState(PlayerStateKey.Jump, new Jump(this)); // Jump�X�e�[�g��ǉ�

        _characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        _inputActions.Enable();
        InputHandler.Enable();

        // �W�����v���̓C�x���g�̍w�ǁiStart()��Awake()�ň�x�����s���j
        // �� Jump�A�N�V������Basic�}�b�v���ɂ���Ɖ���
        InputHandler.Confirm.performed += OnJumpInput; // �����Confirm���W�����v�ɂ��g���Ɖ���

        // 3. �����X�e�[�g��ݒ�
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
        // Z�������g��3D��Ԃł̈ړ��x�N�g���ɕϊ�
        Vector2 input = InputHandler.Move.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, 0, input.y);

        // 1. �ړ��̗L���𔻒f
        bool isMoving = movement.magnitude > 0.1f;

        // 2. �����I�Ȉړ������iIdle/Run�X�e�[�g�ł̂݁A�����Œ��ڎ��s�j
        // Jump/Attack�X�e�[�g�ł̓X�e�[�g���ŏd�͂�A�j���[�V�����l���̈ړ����s�����߁A���O����
        bool isGroundState = _stateMachine.CurrentState is Idle || _stateMachine.CurrentState is Run;

        if (isGroundState)
        {
            // �n�ʂɂ���Ԃ̈ړ�����
            CharacterController.Move(movement * moveSpeed * Time.deltaTime);

            // �J�ڔ��f
            if (isMoving && _stateMachine.CurrentState is Idle)
            {
                SwitchState(PlayerStateKey.Run); // Idle �� Run
            }
            else if (!isMoving && _stateMachine.CurrentState is Run)
            {
                SwitchState(PlayerStateKey.Idle); // Run �� Idle
            }
        }
        // Jump/Attack�X�e�[�g�ł́A���̃��\�b�h�͑J�ڔ���݂̂Ɏg�����A
        // �������X�e�[�g�N���X��Update()�Ɋ��S�ɈϏ����܂��B
    }

    // --- �C�x���g�쓮�̃W�����v���͏��� ---
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // ���݁AIdle�܂���Run�X�e�[�g�ł���A���n�ʂɂ���ꍇ�̂݃W�����v������
        bool isGroundState = _stateMachine.CurrentState is Idle || _stateMachine.CurrentState is Run;

        if (CharacterController.isGrounded && isGroundState)
        {
            SwitchState(PlayerStateKey.Jump);
        }
    }

    // �O������X�e�[�g��؂�ւ��邽�߂̃w���p�[���\�b�h
    public void SwitchState(PlayerStateKey key)
    {
        _stateMachine.SwitchState(key);
    }

    void OnDestroy()
    {
        // �Y�ꂸ��Input System�𖳌����i�������Ǘ��̂��߂ɏd�v�I�j
        // �w�ǉ����������ōs���̂����S
        InputHandler.Confirm.performed -= OnJumpInput;

        _inputActions.Dispose();
    }
}