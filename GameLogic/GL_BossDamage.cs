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

                // 处理各个boss的伤害排行
                var charIdBossDmgList = EM_BossDamage.s_instance.GetBossDmgList ();
                for (int i = 0; i < charIdBossDmgList.Count; i++) {
                    var toNetIdList = EM_Sight.s_instance.GetInSightCharacterNetworkId (charIdBossDmgList[i].Item1, false);
                    // 若视野内无玩家
                    if (toNetIdList.Count == 0) continue;
                    var dmgDict = charIdBossDmgList[i].Item2;
                    // 获取伤害列表并排序
                    var charIdDmgNameList = new List < (int, int, string) > (dmgDict.Count);
                    var dmgEn = dmgDict.GetEnumerator ();
                    while (dmgEn.MoveNext ())
                        charIdDmgNameList.Add ((dmgEn.Current.Key, dmgEn.Current.Value.Item1, dmgEn.Current.Value.Item2));
                    charIdDmgNameList.Sort (((int, int, string) a, (int, int, string) b) => { return b.Item2 - a.Item2; });
                    var dmgCharList = new List<NO_DamageRankCharacter> (c_bossDmgCharacterRankSize);
                    for (int j = 0; j < Math.Min (c_bossDmgCharacterRankSize, charIdDmgNameList.Count); j++) {
                        var item = charIdDmgNameList[j];
                        int rnkCharNetId = EM_Character.s_instance.GetNetIdByCharId (item.Item1);
                        dmgCharList.Add (new NO_DamageRankCharacter (rnkCharNetId, item.Item3, item.Item2));
                    }
                    // client
                    for (int j = 0; j < toNetIdList.Count; j++) {
                        int toNetId = toNetIdList[j];
                        int toCharId = EM_Character.s_instance.GetCharIdByNetId (toNetId);
                        // 获取视野内玩家的排行  
                        (int, string) dmgName;
                        int dmg;
                        short rank;
                        if (!dmgDict.TryGetValue (toCharId, out dmgName)) {
                            dmg = 0;
                            rank = 0;
                        } else {
                            dmg = dmgName.Item1;
                            int l = 0, r = charIdDmgNameList.Count - 1;
                            while (l < r) {
                                int mid = l + r >> 1;
                                int midDmg = charIdDmgNameList[mid].Item2;
                                if (midDmg < dmg)
                                    l = mid + 1;
                                else if (midDmg > dmg)
                                    r = mid - 1;
                                else
                                    l = r = mid;
                            }
                            rank = (short) l;
                        }
                        m_networkService.SendServerCommand (SC_SetAllBossDamageCharacterRank.Instance (toNetId, dmgCharList, dmg, rank));
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
            (int, string) oriDmgName;
            if (!bossDmgDict.TryGetValue (caster.m_characterId, out oriDmgName))
                oriDmgName = (0, caster.m_name);
            oriDmgName.Item2 += dmg;
            bossDmgDict[caster.m_characterId] = oriDmgName;
        }
    }
}