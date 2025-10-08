using UnityEngine;
using UnityEngine.InputSystem; // InputSystem�̌^�iInputAction.CallbackContext�j�̂��߂ɕK�v

namespace PlayerState // ���O��Ԃ��`
{
    public class Idle : IState // IState�C���^�[�t�F�[�X�̎�����O��
    {
        private PlayerController _owner;

        public Idle(PlayerController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            Debug.Log("State: Idle�ɓ���܂����B");
        }

        public void Update()
        {
            // �C���_ 2: �ړ����̗͂L�����`�F�b�N
            // InputHandler.Move�A�N�V�������猻�݂̈ړ��l���|�[�����O�Ŏ擾
            Vector2 moveInput = _owner.InputHandler.Move.ReadValue<Vector2>();

            if (moveInput.magnitude > 0.1f)
            {
                // �ړ����͂��������ꍇ�ARun�X�e�[�g�֑J��
                _owner.SwitchState(PlayerStateKey.Run);
            }
        }

        public void LateUpdate()
        {
            // LateUpdate�̓J������A�j���[�V�����ȂǓ���ȗp�r�Ɏg�����߁A�����ł͉������Ȃ�
        }

        public void Exit()
        {
            Debug.Log("State: Idle�𔲂��܂����B");

            // �w�ǉ����i�d�v�I�j
            _owner.InputHandler.Confirm.performed -= OnConfirmInput;
        }

        // --- �C�x���g�쓮�ŌĂяo����郁�\�b�h ---
        private void OnConfirmInput(InputAction.CallbackContext context)
        {
            // Confirm�i�U���j�{�^���������ꂽ��AAttack�X�e�[�g�ɑJ��
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
}