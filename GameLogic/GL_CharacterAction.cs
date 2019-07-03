using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色行为
    /// 技能释放, 移动等
    /// </summary>
    class GL_CharacterAction : GameLogicBase {
        public static GL_CharacterAction s_instance;
        public GL_CharacterAction (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () {
            var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
            while (charEn.MoveNext ()) {
                // 为角色发送他视野内的其他单位位置
                var charObj = charEn.Current.Value;
                var sightUnits = EM_Sight.s_instance.GetCharacterRawSight (charObj.m_networkId);
                if (sightUnits == null) continue;
                List < (int, Vector2) > sightNetIdPositionList = new List < (int, Vector2) > (sightUnits.Count);
                for (int i = 0; i < sightUnits.Count; i++)
                    sightNetIdPositionList.Add ((sightUnits[i].m_networkId, sightUnits[i].m_position));
                if (sightNetIdPositionList.Count != 0)
                    m_networkService.SendServerCommand (SC_SetOtherPosition.Instance (charObj.m_networkId, sightNetIdPositionList));
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
        public void CommandApplyCastSkillBegin (int netId, short skillId, NO_SkillParam parmNo, int[] hitTargetNetIdArr) {
            // 获取释放者, 技能, 技能参数, 作用目标列表
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            E_Skill skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            E_Unit targetObj = EM_Sight.s_instance.GetUnitVisibleByNetworkId (parmNo.m_targetNetworkId);
            if (charObj == null || skill == null || targetObj == null) return;
            if (skill.m_skillLevel == 0) return;
            List<E_Unit> targetList = new List<E_Unit> ();
            for (int i = 0; i < hitTargetNetIdArr.Length; i++) {
                var target = EM_Sight.s_instance.GetUnitVisibleByNetworkId (hitTargetNetIdArr[i]);
                if (target == null) continue;
                targetList.Add (target);
            }
            // 通知战斗结算
            GL_BattleSettle.s_instance.NotifySkillSettle (charObj, skill, targetList);
            // 向Client发送CastBegin事件
            var otherList = EM_Sight.s_instance.GetInSightCharacterNetworkId (netId, true);
            m_networkService.SendServerCommand (SC_ApplyAllCastSkillBegin.Instance (otherList, netId, skillId, parmNo));
        }
    }
}