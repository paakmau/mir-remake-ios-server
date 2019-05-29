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
        public static GL_BattleSettle s_instance;
        private TargetStage m_targetStage = new TargetStage ();
        public GL_BattleSettle (INetworkService networkService) : base (networkService) {
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifySkillSettle (E_ActorUnit self, DE_Skill skillDe, DE_SkillData skillDataDe, SkillParam parm) {
            var targetList = m_targetStage.GetTargetList (self, skillDe, skillDataDe, parm);
            for (int i = 0; i < targetList.Count; i++) {
                GL_Effect.s_instance.NotifyApplyEffect (skillDataDe.m_skillEffect, self, targetList[i]);
                // TODO: 向Client发Effect
            }
        }
    }
}