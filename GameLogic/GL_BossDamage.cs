using System;
using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_BossDamage : GameLogicBase {
        public static GL_BossDamage s_instance;
        private const float c_bossSightSendTime = 2f;
        private float m_bossSightSendTimer;
        private const int c_bossDmgCharacterRankSize = 8;
        public GL_BossDamage (INetworkService ns) : base (ns) { }
        public override void Tick (float dT) {
            // 发送Boss视野 (伤害量排行榜)
            m_bossSightSendTimer += dT;
            bool sightSend = false;
            while (m_bossSightSendTimer > c_bossSightSendTime) {
                sightSend = true;
                m_bossSightSendTimer -= c_bossSightSendTime;
            }
            if (sightSend) {
                // 判断是否需要清空排行榜
                EM_BossDamage.s_instance.RefreshBossDmg ();

                var bossDmgList = EM_BossDamage.s_instance.GetBossDmgList ();
                for (int i = 0; i < bossDmgList.Count; i++) {
                    // 排行榜
                    var dmgDict = bossDmgList[i].Item2;
                    // 若视野内无玩家
                    var toNetIdList = EM_Sight.s_instance.GetInSightCharacterNetworkId (bossDmgList[i].Item1, false);
                    if (toNetIdList.Count == 0) continue;
                    // 获取伤害列表并排序
                    var netIdAndNameDmgList = new List < KeyValuePair < int,
                        (string, int) >> (dmgDict.Count);
                    var dmgEn = dmgDict.GetEnumerator ();
                    while (dmgEn.MoveNext ())
                        netIdAndNameDmgList.Add (dmgEn.Current);
                    netIdAndNameDmgList.Sort ((KeyValuePair < int, (string, int) > a, KeyValuePair < int, (string, int) > b) => { return b.Value.Item2 - a.Value.Item2; });
                    var dmgCharList = new List<NO_DamageRankCharacter> (c_bossDmgCharacterRankSize);
                    for (int j = 0; j < Math.Min (c_bossDmgCharacterRankSize, netIdAndNameDmgList.Count); j++)
                        dmgCharList.Add (new NO_DamageRankCharacter (netIdAndNameDmgList[j].Key, netIdAndNameDmgList[j].Value.Item1, netIdAndNameDmgList[j].Value.Item2));
                    // client
                    for (int j = 0; j < toNetIdList.Count; j++) {
                        int toNetId = toNetIdList[j];
                        // 获取视野内玩家的排行 TODO: 使用二分
                        (string, int) nameDmg;
                        int rank;
                        if (!dmgDict.TryGetValue (toNetId, out nameDmg))
                            rank = 0;
                        else {
                            rank = 0;
                            for (int k = 0; k < netIdAndNameDmgList.Count; k++)
                                if (netIdAndNameDmgList[k].Key == toNetId) {
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
            // bossDmg清空延后
            EM_BossDamage.s_instance.UpdateBossDmg (bossNetId);
            // 更新伤害
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