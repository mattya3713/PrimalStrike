using UnityEngine;
using UnityEngine.InputSystem; // InputSystemの型（InputAction.CallbackContext）のために必要

namespace PlayerState // 名前空間を定義
{
    public class Idle : IState // IStateインターフェースの実装を前提
    {
        private PlayerController _owner;

        public Idle(PlayerController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            Debug.Log("State: Idleに入りました。");
        }

        public void Update()
        {
            // 修正点 2: 移動入力の有無をチェック
            // InputHandler.Moveアクションから現在の移動値をポーリングで取得
            Vector2 moveInput = _owner.InputHandler.Move.ReadValue<Vector2>();

            if (moveInput.magnitude > 0.1f)
            {
                // 移動入力があった場合、Runステートへ遷移
                _owner.SwitchState(PlayerStateKey.Run);
            }
        }

        public void LateUpdate()
        {
            // LateUpdateはカメラやアニメーションなど特殊な用途に使うため、ここでは何もしない
        }

        public void Exit()
        {
            Debug.Log("State: Idleを抜けました。");

            // 購読解除（重要！）
            _owner.InputHandler.Confirm.performed -= OnConfirmInput;
        }

        // --- イベント駆動で呼び出されるメソッド ---
        private void OnConfirmInput(InputAction.CallbackContext context)
        {
            // Confirm（攻撃）ボタンが押されたら、Attackステートに遷移
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
}