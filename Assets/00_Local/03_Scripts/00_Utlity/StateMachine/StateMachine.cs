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
 * �ėp�I�ȃX�e�[�g�}�V���N���X.
 * T �̓X�e�[�g�}�V���̏��L�ҁiPlayer�AEnemy�Ȃǁj�̌^���`����.
 * K �̓X�e�[�g��Enum��Type�Ȃǂ̃L�[�̌^���`����.
 */
public class StateMachine<T, K>
{
    private T _owner;
    private IState _currentState;
    private Dictionary<K, IState> _states = new Dictionary<K, IState>();

    // ���݂̃X�e�[�g���O������Q�Ƃ��邽�߂̃v���p�e�B
    public IState CurrentState => _currentState;

    // �R���X�g���N�^: �X�e�[�g�}�V���̏��L�҂�ݒ�
    public StateMachine(T owner)
    {
        _owner = owner;
    }

    // �X�e�[�g�������ɓo�^����
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
     * �X�e�[�g��؂�ւ��郁�C�����\�b�h
     * @param newKey �؂�ւ������X�e�[�g�̃L�[
     */
    public void SwitchState(K newKey)
    {
        if (!_states.TryGetValue(newKey, out IState newState))
        {
            Debug.LogError($"State with key {newKey} not found in the dictionary.");
            return;
        }

        // �����X�e�[�g�ւ̑J�ڂ͖���
        if (_currentState == newState)
        {
            return;
        }

        // 1. ���݂̃X�e�[�g�̏I������
        _currentState?.Exit();

        // 2. �V�����X�e�[�g�ɐ؂�ւ�
        _currentState = newState;

        // 3. �V�����X�e�[�g�̊J�n����
        _currentState.Enter();
    }
}