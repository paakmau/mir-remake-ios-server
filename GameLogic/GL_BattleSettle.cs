using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 处理战斗结算相关逻辑  
    /// 作用目标确定  
    /// 对作用目标施加影响  
    /// </summary>
    partial class GL_BattleSettle : GameLogicBase {
        private TargetStage m_targetStage = new TargetStage ();
        public GL_BattleSettle (INetworkService networkService) : base (networkService) {
            Messenger.AddListener<E_ActorUnit, DE_Skill, DE_SkillData, SkillParam> ("NotifySkillSettle", NotifySkillSettle);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        void NotifySkillSettle (E_ActorUnit self, DE_Skill skillDe, DE_SkillData skillDataDe, SkillParam parm) {
            var targetList = m_targetStage.GetTargetList (self, skillDe, skillDataDe, parm);
            for (int i = 0; i < targetList.Count; i++) {
                Messenger.Broadcast<DE_Effect, E_ActorUnit, E_ActorUnit> ("NotifyApplyEffect", skillDataDe.m_skillEffect, self, targetList[i]);
                // TODO: 向Client发Effect
            }
        }
    }
}