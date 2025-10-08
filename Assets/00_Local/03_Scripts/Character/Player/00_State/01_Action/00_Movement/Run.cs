using UnityEngine;

namespace PlayerState // ���O��Ԃ��`
{
    public class Run : IState
    {
        private PlayerController _owner;

        public Run(PlayerController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            Debug.Log("State: Idle�ɓ���܂����B");
            // �C�x���g�w�ǁiInputHandler��_owner����擾�j
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
            Debug.Log("State: Idle�𔲂��܂����B");
            // �C�x���g�w�ǉ���
        }

        private void OnAttackInput()
        {
            // ���O��ԂȂ���Attack���w��
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
}