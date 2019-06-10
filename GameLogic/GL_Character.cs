using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色
    /// 技能释放, 移动等
    /// 属性点分配
    /// </summary>
    /// </summary>
    class GL_Character : GameLogicBase {
        public static GL_Character s_instance;
        private IDDS_Character m_charDds;
        public GL_Character (IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_charDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () {
            var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
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
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
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
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_Skill skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            E_Unit targetObj = EM_Sight.s_instance.GetUnitVisibleByNetworkId (parmNo.m_targetNetworkId);
            if (charObj == null || skill == null || targetObj == null) return;
            SkillParam skillParam = new SkillParam (skill.m_AimType, targetObj, parmNo.m_direction, parmNo.m_position);
            // 通知战斗结算
            GL_BattleSettle.s_instance.NotifySkillSettle (charObj, skill, skillParam);
            // 向Client发送CastBegin事件
            var otherList = EM_Sight.s_instance.GetCharacterInSightNetworkId (netId, false);
            m_networkService.SendServerCommand (SC_ApplyOtherCastSkillBegin.Instance (otherList, netId, skillId, parmNo));
        }
        public void CommandApplyDistributePoints (int netId, short str, short intl, short agl, short spr) {
            // TODO: 
        }
    }
}