
/**************************************************
*	�v���C���[�̏�Ԃ̊��(Base).
*	�S��:���e ����.
**/
using UnityEngine;

public abstract class PlayerStateBase
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void LateUpdate();
    public abstract void Draw();
    public abstract void Exit();

    protected Player _owner;
}