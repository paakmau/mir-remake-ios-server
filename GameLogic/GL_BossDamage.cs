using System;
using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_BossDamage : GameLogicBase {
        public static GL_BossDamage s_instance;
        private const float c_bossSightTime = 2f;
        private float m_bossSightTimer;
        private const int c_bossDmgCharacterRankSize = 8;
        public GL_BossDamage (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) {
            // 发送Boss视野 (伤害量排行榜)
            m_bossSightTimer += dT;
            bool bossSight = false;
            while (m_bossSightTimer > c_bossSightTime) {
                bossSight = true;
                m_bossSightTimer -= c_bossSightTime;
            }
            if (bossSight) {
                var bossEn = EM_Monster.s_instance.GetBossEn ();
                while (bossEn.MoveNext ()) {
                    var dmgDict = EM_BossDamage.s_instance.GetBossDmgDict (bossEn.Current.Key);
                    // 若视野内无玩家
                    var toNetIdList = EM_Sight.s_instance.GetInSightCharacterNetworkId (bossEn.Current.Key, false);
                    if (toNetIdList.Count == 0) continue;
                    // 获取伤害列表并排序
                    var netIdAndNameDmgList = new List < KeyValuePair < int,
                        (string, int) >> (dmgDict.Count);
                    var dmgEn = dmgDict.GetEnumerator ();
                    while (dmgEn.MoveNext ())
                        netIdAndNameDmgList.Add (dmgEn.Current);
                    netIdAndNameDmgList.Sort ((KeyValuePair < int, (string, int) > a, KeyValuePair < int, (string, int) > b) => { return b.Value.Item2 - a.Value.Item2; });
                    var dmgCharList = new List<NO_DamageRankCharacter> (c_bossDmgCharacterRankSize);
                    for (int i = 0; i < Math.Min (c_bossDmgCharacterRankSize, netIdAndNameDmgList.Count); i++)
                        dmgCharList.Add (new NO_DamageRankCharacter(netIdAndNameDmgList[i].Key, netIdAndNameDmgList[i].Value.Item1, netIdAndNameDmgList[i].Value.Item2));
                    // client
                    for (int i = 0; i < toNetIdList.Count; i++) {
                        int toNetId = toNetIdList[i];
                        (string, int) nameDmg;
                        int rank;
                        if (!dmgDict.TryGetValue (toNetId, out nameDmg))
                            rank = 0;
                        else {
                            rank = 0;
                            for (int j = 0; j < netIdAndNameDmgList.Count; j++)
                                if (netIdAndNameDmgList[i].Key == toNetId) {
                                    rank = i + 1;
                                    break;
                                }
                        }
                        m_networkService.SendServerCommand (SC_SetAllBossDamageCharacterRank.Instance (toNetId, dmgCharList, nameDmg.Item2, (short) rank));
                    }
                }
            }
        }
        public override void NetworkTick () { }
        public void NotifyBossAttacked (int bossNetId, E_Character caster, int dmg) {
            var bossDmgDict = EM_BossDamage.s_instance.GetBossDmgDict (bossNetId);
            if (bossDmgDict == null) return;
            (string, int) oriNameDmg;
            if (!bossDmgDict.TryGetValue (caster.m_networkId, out oriNameDmg))
                oriNameDmg = (caster.m_name, 0);
            oriNameDmg.Item2 += dmg;
            bossDmgDict.Add (caster.m_networkId, oriNameDmg);
        }
    }
}