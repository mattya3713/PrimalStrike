using UnityEngine;

namespace PlayerState // 名前空間を定義
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
            Debug.Log("State: Idleに入りました。");
            // イベント購読（InputHandlerは_ownerから取得）
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
            Debug.Log("State: Idleを抜けました。");
            // イベント購読解除
        }

        private void OnAttackInput()
        {
            // 名前空間なしでAttackを指定
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
}