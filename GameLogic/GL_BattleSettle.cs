using System.Collections.Generic;
using UnityEngine;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 处理战斗结算相关逻辑
    /// </summary>
    partial class GL_BattleSettle : GameLogicBase {
        private TargetStage m_targetStage = new TargetStage ();
        public GL_BattleSettle (INetworkService networkService) : base (networkService) {
            Messenger.AddListener<int, E_Skill, SkillParam> ("NotifySkillSettle", NotifySkillSettle);
        }
        public override void Tick(float dT) { }
        public override void NetworkTick() { }
        void NotifySkillSettle (int netId, E_Skill skill, SkillParam parm) {
        }
    }
}