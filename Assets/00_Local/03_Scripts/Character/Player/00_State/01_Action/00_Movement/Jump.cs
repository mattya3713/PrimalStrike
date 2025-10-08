using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerState
{
    public class Jump : IState
    {
        private PlayerController _owner;
        private float _jumpVelocityY;

        // ジャンプの強さや重力をPlayerControllerで管理することが多いですが、ここでは仮定値
        private const float JUMP_POWER = 7.0f;
        private const float GRAVITY = -9.81f * 2.0f; // 重力加速を強めに設定

        public Jump(PlayerController owner)
        {
            _owner = owner;
        }

        public void Enter()
        {
            Debug.Log("State: Jumpに入りました。");

            // ログメッセージを 'Idle' から 'Jump' に修正
            // Debug.Log("State: Idleに入りました。"); 

            // --- ジャンプの実行 ---
            // Y軸の上方向速度を設定して、ジャンプを開始
            _jumpVelocityY = JUMP_POWER;

            // ジャンプ中でも攻撃を可能にするため、Confirm（攻撃）イベントを購読
            _owner.InputHandler.Confirm.performed += OnConfirmInput;
        }

        public void Update()
        {
            // 修正: ジャンプ中の重力計算と移動の継続

            // 1. 重力適用: 時間と共に速度を減衰/加速（落下の加速）
            _jumpVelocityY += GRAVITY * Time.deltaTime;

            // 2. 水平移動: ジャンプ中でも Move 入力を受け付ける
            Vector2 moveInput = _owner.InputHandler.Move.ReadValue<Vector2>();
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * _owner.MoveSpeed; // MoveSpeedはPlayerControllerで公開されていると仮定

            // 3. 実行: CharacterController.Move は重力を含めたベクトルで実行
            Vector3 finalMovement = movement + new Vector3(0, _jumpVelocityY, 0);
            _owner.CharacterController.Move(finalMovement * Time.deltaTime);

            // 4. 着地判定: CharacterControllerのIsGroundedを使う
            // ※ IsGroundedはMoveの直後でのみ正確に判断可能
            if (_owner.CharacterController.isGrounded && _jumpVelocityY < 0.0f)
            {
                // 着地し、かつ落下中（重力でY速度が負）であれば、着地処理
                _owner.SwitchState(PlayerStateKey.Idle);
            }
        }

        // LateUpdate は通常ジャンプ処理では不要
        public void LateUpdate() { }

        public void Exit()
        {
            Debug.Log("State: Jumpを抜けました。");

            // イベント購読解除（重要！）
            // _owner.InputHandler.OnAttackPressed -> Confirm に修正
            _owner.InputHandler.Confirm.performed -= OnConfirmInput;

            // 着地時に Y軸速度をリセット
            // _jumpVelocityY = 0f; // PlayerControllerの移動処理で行う方が良い場合もある
        }

        // --- イベント駆動で呼び出されるメソッド ---
        // Jumpステート中でも攻撃（Confirm）を受け付ける
        private void OnConfirmInput(InputAction.CallbackContext context)
        {
            // 攻撃ステートへの遷移を試みる
            _owner.SwitchState(PlayerStateKey.Attack);
        }
    }
}