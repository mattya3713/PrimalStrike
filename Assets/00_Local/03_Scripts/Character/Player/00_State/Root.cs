
/**************************************************
*	プレイヤーの状態の最上位(Root).
*   SystemとGamePlayerの切り替えを最優先で管理する.
*	担当:淵脇 未来.
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

        //		// ステートの変更.
        //        void ChangeState(PlayerState::eID id);

        //    // 階層ステートの取得.
        //    System* GetSystemState() { return m_pSystem.get(); }
        //    Action* GetGameplayState() { return m_pGameplay.get(); }

        //    private:
        //        // 第二階層のステート.
        //        std::unique_ptr<System> m_pSystem;
        //    std::unique_ptr<Action> m_pGameplay;

        //    // 現在アクティブな最上位の状態を保持.
        //    std::reference_wrapper<PlayerStateBase> m_CurrentActiveState;


    }

}
} // PlayerState