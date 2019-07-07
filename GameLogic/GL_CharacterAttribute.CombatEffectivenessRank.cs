using System;
using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_CharacterAttribute {
        private class CombatEffectivenessRank {
            private class CombatEffectiveItem : IComparable {
                public int m_charId;
                public int m_combatEfct;
                public CombatEffectiveItem (int charId, int combatEfct) {
                    m_charId = charId;
                    m_combatEfct = combatEfct;
                }
                public int CompareTo (Object bObj) {
                    var b = bObj as CombatEffectiveItem;
                    if (m_combatEfct != b.m_combatEfct)
                        return m_combatEfct - b.m_combatEfct;
                    return m_charId - b.m_charId;
                }
            }
            // TODO: 需要自行实现平衡树
            private SortedList<CombatEffectiveItem, bool> m_rank = new SortedList<CombatEffectiveItem, bool> ();
            private Dictionary<int, int> m_charIdAndOriCombatEfctDict = new Dictionary<int, int> ();
            private Dictionary<int, int> m_charIdAndCombatEfctToUpdateDict = new Dictionary<int, int> ();
            public void Flush () {
                var en = m_charIdAndCombatEfctToUpdateDict.GetEnumerator ();
                while (en.MoveNext ()) {
                    // m_rankSet.Remove (new CombatEffectiveItem (en.Current.Key, m_charIdAndOriCombatEfctDict[en.Current.Key]));
                    // m_rankSet.Add (new CombatEffectiveItem (en.Current.Key, en.Current.Value));
                    m_charIdAndOriCombatEfctDict[en.Current.Key] = en.Current.Value;
                }
                m_charIdAndCombatEfctToUpdateDict.Clear ();
            }
            public void UpdateCharCombatEfct (int charId, int combatEfct) {
                m_charIdAndCombatEfctToUpdateDict[charId] = combatEfct;
            }
            public List<NO_FightCapacityRankInfo> GetTopCombatEfctRnkList (int num) {
                var res = new List<NO_FightCapacityRankInfo> ();
                // var en = m_rankSet.GetEnumerator ();
                // int i = 0;
                // while (en.MoveNext () && i < num) {
                //     // res.Add (new NO_FightCapacityRankInfo (en.Current.m_charId, ))
                //     i++;
                // }
                return res;
            }
            public int GetRank (int charId) {
                var combatEfct = m_charIdAndOriCombatEfctDict[charId];
                // m_rankSet;
                return 100;
            }
        }
    }
}