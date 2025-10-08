using UnityEngine;

/**************************************
*   プレイヤーのステート列挙.
*   担当：淵脇 未来
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
            // ... 移動チェックロジックなど ...
        }

        public void LateUpdate()
        {
            // ... 移動チェックロジックなど ...
        }

        public void Exit()
        {
            // イベント購読解除
        }

        private void OnAttackInput()
        {
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
} // PlayerState.