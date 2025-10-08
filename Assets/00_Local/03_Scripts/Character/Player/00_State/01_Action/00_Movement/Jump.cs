using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerState
{
    public class Jump : IState
    {
        private PlayerController _owner;
        private float _jumpVelocityY;

        // �W�����v�̋�����d�͂�PlayerController�ŊǗ����邱�Ƃ������ł����A�����ł͉���l
        private const float JUMP_POWER = 7.0f;
        private const float GRAVITY = -9.81f * 2.0f; // �d�͉��������߂ɐݒ�

        public Jump(PlayerController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            Debug.Log("State: Jump�ɓ���܂����B");

            // ���O���b�Z�[�W�� 'Idle' ���� 'Jump' �ɏC��
            // Debug.Log("State: Idle�ɓ���܂����B"); 

            // --- �W�����v�̎��s ---
            // Y���̏�������x��ݒ肵�āA�W�����v���J�n
            _jumpVelocityY = JUMP_POWER;

            // �W�����v���ł��U�����\�ɂ��邽�߁AConfirm�i�U���j�C�x���g���w��
            _owner.InputHandler.Confirm.performed += OnConfirmInput;
        }

        public void Update()
        {
            // �C��: �W�����v���̏d�͌v�Z�ƈړ��̌p��

            // 1. �d�͓K�p: ���ԂƋ��ɑ��x������/�����i�����̉����j
            _jumpVelocityY += GRAVITY * Time.deltaTime;

            // 2. �����ړ�: �W�����v���ł� Move ���͂��󂯕t����
            Vector2 moveInput = _owner.InputHandler.Move.ReadValue<Vector2>();
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * _owner.MoveSpeed; // MoveSpeed��PlayerController�Ō��J����Ă���Ɖ���

            // 3. ���s: CharacterController.Move �͏d�͂��܂߂��x�N�g���Ŏ��s
            Vector3 finalMovement = movement + new Vector3(0, _jumpVelocityY, 0);
            _owner.CharacterController.Move(finalMovement * Time.deltaTime);

            // 4. ���n����: CharacterController��IsGrounded���g��
            // �� IsGrounded��Move�̒���ł̂ݐ��m�ɔ��f�\
            if (_owner.CharacterController.isGrounded && _jumpVelocityY < 0.0f)
            {
                // ���n���A���������i�d�͂�Y���x�����j�ł���΁A���n����
                _owner.SwitchState(PlayerStateKey.Idle);
            }
        }

        // LateUpdate �͒ʏ�W�����v�����ł͕s�v
        public void LateUpdate() { }

        public void Exit()
        {
            Debug.Log("State: Jump�𔲂��܂����B");

            // �C�x���g�w�ǉ����i�d�v�I�j
            // _owner.InputHandler.OnAttackPressed -> Confirm �ɏC��
            _owner.InputHandler.Confirm.performed -= OnConfirmInput;

            // ���n���� Y�����x�����Z�b�g
            // _jumpVelocityY = 0f; // PlayerController�̈ړ������ōs�������ǂ��ꍇ������
        }

        // --- �C�x���g�쓮�ŌĂяo����郁�\�b�h ---
        // Jump�X�e�[�g���ł��U���iConfirm�j���󂯕t����
        private void OnConfirmInput(InputAction.CallbackContext context)
        {
            // �U���X�e�[�g�ւ̑J�ڂ����݂�
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
}