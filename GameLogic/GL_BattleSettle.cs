using System;
using System.Collections.Generic;
using System.Linq;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic
{
    /// <summary>
    /// 处理战斗结算相关逻辑  
    /// 作用目标确定  
    /// 对作用目标施加影响  
    /// </summary>
    partial class GL_BattleSettle : GameLogicBase {
        private class TargetStage {
            private Dictionary<SkillAimType, EffectTargetChooserBase> m_targetChooserDict = new Dictionary<SkillAimType, EffectTargetChooserBase> ();
            private List<E_Unit> m_targetList = new List<E_Unit> ();
            public TargetStage () {
                // 实例化ETC的所有子类
                var etcType = typeof (EffectTargetChooserBase);
                var etcImplTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => !p.IsAbstract && etcType.IsAssignableFrom (p));
                foreach (var type in etcImplTypes) {
                    EffectTargetChooserBase etcObj = type.GetConstructor (Type.EmptyTypes).Invoke (null) as EffectTargetChooserBase;
                    m_targetChooserDict.Add (etcObj.m_TargetAimType, etcObj);
                }
            }
            public IReadOnlyList<E_Unit> GetTargetList (E_Unit self, E_MonsterSkill skill, SkillParam skillParm) {
                EffectTargetChooserBase targetChooser = m_targetChooserDict[skill.m_AimType];
                targetChooser.Reset (skill.m_TargetCamp, skill.m_TargetNumber, skill.m_DamageParamList);
                targetChooser.GetEffectTargets (self, skillParm, m_targetList);
                return m_targetList;
            }
        }
        public static GL_BattleSettle s_instance;
        private TargetStage m_targetStage = new TargetStage ();
        public GL_BattleSettle (INetworkService networkService) : base (networkService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifySkillSettle (E_Unit self, E_MonsterSkill skill, SkillParam parm) {
            NotifySkillSettle (self, skill, m_targetStage.GetTargetList (self, skill, parm));
        }
        public void NotifySkillSettle (E_Unit self, E_MonsterSkill skill, IReadOnlyList<E_Unit> targetList) {
            for (int i = 0; i < targetList.Count; i++)
                GL_UnitBattleAttribute.s_instance.NotifyApplyEffect (skill.m_SkillEffect, skill.m_SkillId, self, targetList[i]);
        }
    }
}