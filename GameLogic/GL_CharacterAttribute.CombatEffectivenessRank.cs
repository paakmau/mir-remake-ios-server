using System;
using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_CharacterAttribute {
        private class CombatEffectivenessRank {
            private struct CombatEffectiveItem : IComparable {
                public int m_charId;
                public int m_combatEfct;
                public CombatEffectiveItem (int charId, int combatEfct) {
                    m_charId = charId;
                    m_combatEfct = combatEfct;
                }
                public int CompareTo (Object bObj) {
                    var b = (CombatEffectiveItem) bObj;
                    if (m_combatEfct != b.m_combatEfct)
                        return m_combatEfct - b.m_combatEfct;
                    return m_charId - b.m_charId;
                }
            }
            private SortedSet<CombatEffectiveItem> m_rankSet = new SortedSet<CombatEffectiveItem> ();
            // private Dictionary<int, CombatEffectiveItem> m_
            public List<NO_FightCapacityRankInfo> GetTopCombatEfctRnkList (int num) {
                var res = new List<NO_FightCapacityRankInfo> ();
                var en = m_rankSet.GetEnumerator ();
                int i = 0;
                while (en.MoveNext () && i < num) {
                    // res.Add (new NO_FightCapacityRankInfo (en.Current.m_charId, ))
                    i++;
                }
                return res;
            }
            public int GetRank (int charId) {
                return 100;
            }
        }
    }
}