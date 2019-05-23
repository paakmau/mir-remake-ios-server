using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using UnityEngine;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 处理战斗结算相关逻辑
    /// </summary>
    partial class GL_BattleSettle : GameLogicBase {
        private TargetStage m_targetStage = new TargetStage ();
        public GL_BattleSettle (INetworkService networkService) : base (networkService) {
            Messenger.AddListener<int, DE_Skill, DE_SkillData, SkillParam> ("NotifySkillSettle", NotifySkillSettle);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        void NotifySkillSettle (int netId, DE_Skill skillDe, DE_SkillData skillDataDe, SkillParam parm) {
            m_targetStage.
        }
    }
}