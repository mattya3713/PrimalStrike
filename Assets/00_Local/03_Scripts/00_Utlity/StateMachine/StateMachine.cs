using UnityEngine;
using System.Collections.Generic;

// IStateBase.
public interface IState
{
    void Enter();
    void Update();
    void LateUpdate();
    void Exit();
}

/**
 * 汎用的なステートマシンクラス.
 * T はステートマシンの所有者（Player、Enemyなど）の型を定義する.
 * K はステートのEnumやTypeなどのキーの型を定義する.
 */
public class StateMachine<T, K>
{
    private T _owner;
    private IState _currentState;
    private Dictionary<K, IState> _states = new Dictionary<K, IState>();

    // 現在のステートを外部から参照するためのプロパティ
    public IState CurrentState => _currentState;

    // コンストラクタ: ステートマシンの所有者を設定
    public StateMachine(T owner)
    {
        _owner = owner;
    }

    // ステートを辞書に登録する
    public void AddState(K key, IState state)
    {
        if (!_states.ContainsKey(key))
        {
            _states.Add(key, state);
        }
        else
        {
            Debug.LogWarning($"State with key {key} already exists.");
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public void LateUpdate()
    {
        _currentState?.LateUpdate();
    }

    /**
     * ステートを切り替えるメインメソッド
     * @param newKey 切り替えたいステートのキー
     */
    public void SwitchState(K newKey)
    {
        if (!_states.TryGetValue(newKey, out IState newState))
        {
            Debug.LogError($"State with key {newKey} not found in the dictionary.");
            return;
        }

        // 同じステートへの遷移は無視
        if (_currentState == newState)
        {
            return;
        }

        // 1. 現在のステートの終了処理
        _currentState?.Exit();

        // 2. 新しいステートに切り替え
        _currentState = newState;

        // 3. 新しいステートの開始処理
        _currentState.Enter();
    }
}