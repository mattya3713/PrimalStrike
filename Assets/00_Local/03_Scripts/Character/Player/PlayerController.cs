// Player.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MainInput;

public class PlayerController : MonoBehaviour
{
    // ���L��T: PlayerController, �L�[K: PlayerStateKey
    private StateMachine<PlayerController, PlayerStateKey> _stateMachine;

    private MainInput _inputActions; 
    public BasicActions InputHandler { get; private set; }

    [SerializeField] private float moveSpeed = 5.0f;
    private CharacterController _characterController;

    public CharacterController CharacterController { get; private set; }
    public float MoveSpeed => moveSpeed; // moveSpeed��getter�����J
    void Awake()
    {
        _inputActions = new MainInput();

        // 1. �X�e�[�g�}�V���̏�����
        _stateMachine = new StateMachine<PlayerController, PlayerStateKey>(this);

        // �e�X�e�[�g�͏��L�ҁithis�j���󂯎��R���X�g���N�^�����K�v������܂�
        _stateMachine.AddState(PlayerStateKey.Idle, new PlayerState.Idle(this));
        _stateMachine.AddState(PlayerStateKey.Run, new PlayerState.Run(this));
        _stateMachine.AddState(PlayerStateKey.Attack, new PlayerState.Attack(this));
        _characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        // 3. �����X�e�[�g��ݒ�
        _stateMachine.SwitchState(PlayerStateKey.Idle);
    }

    void Update()
    {
        _stateMachine.Update(); // �X�e�[�g�ŗL�̃��W�b�N���s

        // �X�e�[�g�Ɋ֌W�Ȃ��A�ړ������݂�i�|�[�����O�j
        HandleMovementAndTransition();
    }

    private void HandleMovementAndTransition()
    {
        Vector2 input = InputHandler.Move.ReadValue<Vector2>();
        Debug.Log(input);
        // Z�������g��3D��Ԃł̈ړ��x�N�g���ɕϊ�
        Vector3 movement = new Vector3(input.x, 0, input.y);

        // 1. �ړ��̗L���𔻒f
        bool isMoving = movement.magnitude > 0.1f;

        // 2. �����I�Ȉړ�����
        if (isMoving)
        {
            // ���ۂɃv���C���[���ړ������鏈��
            // CharacterController���g�p����ꍇ�̗�
            _characterController.Move(movement * moveSpeed * Time.deltaTime);

            // �X�e�[�g�J�ڂ̔��f�iIdle �� Move�j
            if (_stateMachine.CurrentState is PlayerState.Idle)
            {
                // Idle�X�e�[�g����Move�X�e�[�g�֑J��
                // SwitchState(PlayerStateKey.Move); 
            }
        }
        else
        {
            // �ړ����͂��Ȃ��ꍇ�̏���
            // �X�e�[�g�J�ڂ̔��f�iMove �� Idle�j
            // if (_stateMachine.CurrentState is PlayerState.Move)
            // {
            //     SwitchState(PlayerStateKey.Idle);
            // }
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
        _inputActions.Dispose();
    }
}