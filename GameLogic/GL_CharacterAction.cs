using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色的行为
    /// 例如技能释放, 移动等
    /// </summary>
    class GL_CharacterAction : GameLogicBase {
        public static GL_CharacterAction s_instance;
        private List<ValueTuple<MyTimer.Time, E_ActorUnit, E_Skill, SkillParam>> m_skillToCastList = new List < (MyTimer.Time, E_ActorUnit, E_Skill, SkillParam) > ();
        public GL_CharacterAction (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            // 判断角色技能吟唱与前摇结束并释放
            List<int> netIdToCastSkillList = new List<int> ();
            for (int i = m_skillToCastList.Count - 1; i >= 0; i--) {
                var item = m_skillToCastList[i];
                if (!MyTimer.CheckTimeUp (item.Item1))
                    continue;
                m_skillToCastList.RemoveAt (i);
                GL_BattleSettle.s_instance.NotifySkillSettle (item.Item2, item.Item3, item.Item4);
            }
        }
        public override void NetworkTick () {
            var charEn = EM_ActorUnit.s_instance.GetCharacterEnumerator ();
            while (charEn.MoveNext ()) {
                // 为角色发送他视野内的其他单位位置
                var charObj = charEn.Current.Value;
                List<int> selfNetIdList = new List<int> ();
                selfNetIdList.Add (charObj.m_networkId);
                var sightUnits = EM_Sight.s_instance.GetCharacterRawSight (charObj.m_networkId);
                List < (int, Vector2) > sightNetIdPositionList = new List < (int, Vector2) > (sightUnits.Count);
                for (int i = 0; i < sightUnits.Count; i++)
                    sightNetIdPositionList.Add ((sightUnits[i].m_networkId, sightUnits[i].m_position));
                m_networkService.SendServerCommand (SC_SetOtherPosition.Instance (
                    selfNetIdList,
                    sightNetIdPositionList
                ));
            }
        }
        public void CommandSetPosition (int netId, Vector2 pos) {
            E_Character charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            charObj.m_position = pos;
        }
        /// <summary>
        /// client的角色开始释放技能
        /// server需要通知视野内其他client播放动画
        /// 并添加待结算技能计时, 计时结束后进入技能结算
        /// </summary>
        public void CommandApplyCastSkillBegin (int netId, short skillId, NO_SkillParam parmNo) {
            // 获取释放者, 技能, 获取技能参数实例
            E_Character charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            E_Skill skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            E_ActorUnit targetObj = EM_Sight.s_instance.GetActorUnitVisibleByNetworkId (parmNo.m_targetNetworkId);
            if (charObj == null || skill == null || targetObj == null) return;
            MyTimer.Time castTime = MyTimer.s_CurTime.Ticked (skill.m_SingAndCastFrontTime);
            SkillParam skillParam = new SkillParam (skill.m_AimType, targetObj, parmNo.m_direction, parmNo.m_position);
            // 添加到技能待释放列表
            m_skillToCastList.Add (new ValueTuple<MyTimer.Time, E_ActorUnit, E_Skill, SkillParam> (castTime, charObj, skill, skillParam));
            // 向Client发送CastBegin事件
            var otherList = EM_Sight.s_instance.GetCharacterInSightNetworkId (netId, false);
            m_networkService.SendServerCommand (SC_ApplyOtherCastSkillBegin.Instance (otherList, netId, skillId, parmNo));
        }
        /// <summary>
        /// client角色取消技能读条  
        /// 移除待结算技能计时  
        /// 通知视野内其他client播放动画  
        /// </summary>
        public void CommandApplyCastSkillSingCancel (int netId) {
            for (int i = 0; i < m_skillToCastList.Count; i++)
                if (m_skillToCastList[i].Item2.m_networkId == netId) {
                    m_skillToCastList.RemoveAt (i);
                    break;
                }
            var otherList = EM_Sight.s_instance.GetCharacterInSightNetworkId (netId, false);
            m_networkService.SendServerCommand (SC_ApplyOtherCastSkillSingCancel.Instance (otherList, netId));
        }
    }
}