
/**************************************************
*	�v���C���[�̏�Ԃ̍ŏ��(Root).
*   System��GamePlayer�̐؂�ւ����ŗD��ŊǗ�����.
*	�S��:���e ����.
**/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class Root : public PlayerStateBase
{
  
        System _system;
        Action _action;

    override void Enter()
    {

    }

    override void Update()
    {
    }
    override void LateUpdate()
    {

    }
    override void Exit()
    {

        //}

        //		// �X�e�[�g�̕ύX.
        //        void ChangeState(PlayerState::eID id);

        //    // �K�w�X�e�[�g�̎擾.
        //    System* GetSystemState() { return m_pSystem.get(); }
        //    Action* GetGameplayState() { return m_pGameplay.get(); }

        //    private:
        //        // ���K�w�̃X�e�[�g.
        //        std::unique_ptr<System> m_pSystem;
        //    std::unique_ptr<Action> m_pGameplay;

        //    // ���݃A�N�e�B�u�ȍŏ�ʂ̏�Ԃ�ێ�.
        //    std::reference_wrapper<PlayerStateBase> m_CurrentActiveState;


    }

}
} // PlayerState