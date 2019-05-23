using System.Numerics;
using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色的行为
    /// 例如技能释放, 移动等
    /// </summary>
    class GL_CharacterAction : GameLogicBase {
        private Dictionary<int, ValueTuple<MyTimer.Time, DE_Skill, DE_SkillData, SkillParam>> m_networkIdAndSkillCastDict = new Dictionary<int, ValueTuple<MyTimer.Time, DE_Skill, DE_SkillData, SkillParam>> ();
        public GL_CharacterAction (INetworkService netService) : base (netService) {
            Messenger.AddListener<int, Vector2> ("CommandSetPosition", CommandSetPosition);
            Messenger.AddListener<int, short, NO_SkillParam> ("CommandApplyCastSkillBegin", CommandApplyCastSkillBegin);
        }
        public override void Tick (float dT) {
            List<int> netIdToCastSkillList = new List<int> ();
            var skillCastEn = m_networkIdAndSkillCastDict.GetEnumerator ();
            while (skillCastEn.MoveNext ())
                if (MyTimer.CheckTimeUp (skillCastEn.Current.Value.Item1))
                    netIdToCastSkillList.Add (skillCastEn.Current.Key);
            for (int i = 0; i < netIdToCastSkillList.Count; i++) {
                ValueTuple<MyTimer.Time, DE_Skill, DE_SkillData, SkillParam> skillToCast = m_networkIdAndSkillCastDict[netIdToCastSkillList[i]];
                m_networkIdAndSkillCastDict.Remove (netIdToCastSkillList[i]);
                // 通知技能结算逻辑
                Messenger.Broadcast<int, DE_Skill, DE_SkillData, SkillParam> ("NotifySkillSettle", netIdToCastSkillList[i], skillToCast.Item2, skillToCast.Item3, skillToCast.Item4);
            }
        }
        public override void NetworkTick () { }
        public void CommandSetPosition (int netId, Vector2 pos) {
            E_Character charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            charObj.m_position = pos;
        }
        public void CommandApplyCastSkillBegin (int netId, short skillId, NO_SkillParam parmNo) {
            // 获取释放者, 技能, 决定技能参数
            E_Character charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            E_Skill skillObj = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            E_ActorUnit targetObj = EM_Sight.s_instance.GetActorUnitVisibleByNetworkId (parmNo.m_targetNetworkId);
            if (charObj == null || skillObj == null || targetObj == null) return;
            MyTimer.Time castTime = MyTimer.s_CurTime.Ticked (skillObj.m_SingAndCastFrontTime);
            DE_Skill skillDe = skillObj.m_skillDe;
            DE_SkillData skillDataDe = skillObj.m_skillDataDe;
            SkillParam skillParam = new SkillParam (skillDe.m_skillAimType, targetObj, parmNo.m_direction, parmNo.m_position);
            // 添加到技能待释放字典
            m_networkIdAndSkillCastDict[netId] = new ValueTuple<MyTimer.Time, DE_Skill, DE_SkillData, SkillParam> (castTime, skillDe, skillDataDe, skillParam);
            // 向Client发送CastBegin事件
            var otherList = EM_Sight.s_instance.GetActorUnitsInSightNetworkIdByNetworkId (netId, false);
            m_networkService.SendServerCommand (new SC_ApplyOtherCastSkillBegin (otherList, netId, skillId, parmNo));
        }
    }
}