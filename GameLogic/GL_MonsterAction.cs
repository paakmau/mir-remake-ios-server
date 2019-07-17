using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 控制Monster的AI
    /// </summary>
    partial class GL_MonsterAction : GameLogicBase {
        public static GL_MonsterAction s_instance;
        private Dictionary<SkillAimType, SkillParamGeneratorBase> m_spgDict = new Dictionary<SkillAimType, SkillParamGeneratorBase> ();
        private Dictionary<int, MFSM> m_mfsmDict = new Dictionary<int, MFSM> ();
        private const float c_bossSightTime = 2;
        private float m_bossSightTimer;
        public GL_MonsterAction (INetworkService netService) : base (netService) {
            var monEn = EM_Unit.s_instance.GetMonsterEn ();
            while (monEn.MoveNext ()) {
                var mfsm = new MFSM (monEn.Current.Value);
                m_mfsmDict.Add (monEn.Current.Key, mfsm);
            }
            // Mfsm 的构造
            SkillParamGeneratorBase.Init ();
        }
        public override void Tick (float dT) {
            // fsm控制AI
            var mfsmEn = m_mfsmDict.GetEnumerator ();
            while (mfsmEn.MoveNext ())
                mfsmEn.Current.Value.Tick (dT);
            // 发送Boss视野
            var bossEn = EM_Unit.s_instance.GetBossEn ();
            while (bossEn.MoveNext ()) {
                var toNetIdList = EM_Sight.s_instance.GetInSightCharacterNetworkId (bossEn.Current.Key, false);
                var dmgList = new List<NO_DamageRankCharacter> (bossEn.Current.Value.m_netIdAndDamageDict.Count);
                var dmgEn = bossEn.Current.Value.m_netIdAndDamageDict.GetEnumerator ();
                while (dmgEn.MoveNext ()) {
                    var netId = dmgEn.Current.Key;
                    var dmg = dmgEn.Current.Value;
                    var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
                    if (charObj == null) continue;
                    charObj.GetDmgRnkNo ();
                }
                // TODO: boss伤害量统计
            }
        }
        public override void NetworkTick () { }
        public void MFSMCastSkillBegin (int netId, E_MonsterSkill skill, SkillParam sp) {
            // client
            m_networkService.SendServerCommand (SC_ApplyAllCastSkillBegin.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (netId, false),
                netId,
                skill.m_SkillId,
                sp.GetNo ()));
        }
        public void MFSMSkillSettle (E_Monster monster, E_MonsterSkill skill, SkillParam sp) {
            GL_BattleSettle.s_instance.NotifySkillSettle (monster, skill, sp);
        }
        public void MFSMRespawn (E_Monster monster) {
            // client
            m_networkService.SendServerCommand (SC_ApplyAllRespawn.Instance (
                EM_Sight.s_instance.GetInSightCharacterNetworkId (monster.m_networkId, false),
                monster.m_networkId,
                monster.m_position,
                monster.m_curHp,
                monster.m_MaxHp));
        }
    }
}