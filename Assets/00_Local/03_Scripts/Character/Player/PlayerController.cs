// Player.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static MainInput;

public class PlayerController : MonoBehaviour
{
    // 所有者T: PlayerController, キーK: PlayerStateKey
    private StateMachine<PlayerController, PlayerStateKey> _stateMachine;

    private MainInput _inputActions; 
    public BasicActions InputHandler { get; private set; }

    [SerializeField] private float moveSpeed = 5.0f;
    private CharacterController _characterController;

    public CharacterController CharacterController { get; private set; }
    public float MoveSpeed => moveSpeed; // moveSpeedのgetterを公開
    void Awake()
    {
        _inputActions = new MainInput();

        // 1. ステートマシンの初期化
        _stateMachine = new StateMachine<PlayerController, PlayerStateKey>(this);

        // 各ステートは所有者（this）を受け取るコンストラクタを持つ必要があります
        _stateMachine.AddState(PlayerStateKey.Idle, new PlayerState.Idle(this));
        _stateMachine.AddState(PlayerStateKey.Run, new PlayerState.Run(this));
        _stateMachine.AddState(PlayerStateKey.Attack, new PlayerState.Attack(this));
        _characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        // 3. 初期ステートを設定
        _stateMachine.SwitchState(PlayerStateKey.Idle);
    }

    void Update()
    {
        _stateMachine.Update(); // ステート固有のロジック実行

        // ステートに関係なく、移動を試みる（ポーリング）
        HandleMovementAndTransition();
    }

    private void HandleMovementAndTransition()
    {
        Vector2 input = InputHandler.Move.ReadValue<Vector2>();
        Debug.Log(input);
        // Z方向も使う3D空間での移動ベクトルに変換
        Vector3 movement = new Vector3(input.x, 0, input.y);

        // 1. 移動の有無を判断
        bool isMoving = movement.magnitude > 0.1f;

        // 2. 物理的な移動処理
        if (isMoving)
        {
            // 実際にプレイヤーを移動させる処理
            // CharacterControllerを使用する場合の例
            _characterController.Move(movement * moveSpeed * Time.deltaTime);

            // ステート遷移の判断（Idle → Move）
            if (_stateMachine.CurrentState is PlayerState.Idle)
            {
                // IdleステートからMoveステートへ遷移
                // SwitchState(PlayerStateKey.Move); 
            }
        }
        else
        {
            // 移動入力がない場合の処理
            // ステート遷移の判断（Move → Idle）
            // if (_stateMachine.CurrentState is PlayerState.Move)
            // {
            //     SwitchState(PlayerStateKey.Idle);
            // }
        }
    }

    // 外部からステートを切り替えるためのヘルパーメソッド
    public void SwitchState(PlayerStateKey key)
    {
        _stateMachine.SwitchState(key);
    }
    void OnDestroy()
    {
        // 忘れずにInput Systemを無効化（メモリ管理のために重要！）
        _inputActions.Dispose();
    }
}