using UnityEngine;

/**************************************
*   �v���C���[�̃X�e�[�g��.
*   �S���F���e ����
**************************************/

namespace PlayerState 
{
    public class Attack : IState
    {
        private PlayerController _owner;

        public Attack(PlayerController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
        }

        public void Update()
        {
            // ... �ړ��`�F�b�N���W�b�N�Ȃ� ...
        }

        public void LateUpdate()
        {
            // ... �ړ��`�F�b�N���W�b�N�Ȃ� ...
        }

        public void Exit()
        {
            // �C�x���g�w�ǉ���
        }

        private void OnAttackInput()
        {
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
} // PlayerState.