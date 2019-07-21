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
                    var netIdAndDmgList = new List<KeyValuePair<int, int>> (dmgDict.Count);
                    var dmgEn = dmgDict.GetEnumerator ();
                    while (dmgEn.MoveNext ())
                        netIdAndDmgList.Add (dmgEn.Current);
                    netIdAndDmgList.Sort ((KeyValuePair<int, int> a, KeyValuePair<int, int> b) => { return b.Value - a.Value; });
                    var dmgCharList = new List<NO_DamageRankCharacter> (c_bossDmgCharacterRankSize);
                    for (int i = 0; i < Math.Min (c_bossDmgCharacterRankSize, netIdAndDmgList.Count); i++) {
                        var charObj = EM_Character.s_instance.GetCharacterByNetworkId (netIdAndDmgList[i].Key);
                        if (charObj == null) continue;
                        dmgCharList.Add (charObj.GetDmgRnkNo (netIdAndDmgList[i].Value));
                    }
                    // client
                    for (int i = 0; i < toNetIdList.Count; i++) {
                        int toNetId = toNetIdList[i];
                        int dmg;
                        dmgDict.TryGetValue (toNetId, out dmg);
                        int rank;
                        if (dmg == 0)
                            rank = 0;
                        else {
                            rank = 0;
                            for (int j = 0; j < netIdAndDmgList.Count; j++)
                                if (netIdAndDmgList[i].Key == toNetId) {
                                    rank = i + 1;
                                    break;
                                }
                        }
                        m_networkService.SendServerCommand (SC_SetAllBossDamageCharacterRank.Instance (toNetId, dmgCharList, dmg, (short) rank));
                    }
                }
            }
        }
        public override void NetworkTick () { }
        public void NotifyBossAttacked (E_Monster boss, int dmg) { }
    }
}