using System.Collections.Generic;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_CharacterAttribute {
        private class CombatEffectivenessRank {
            private struct CombatEffectiveItem {
                int m_charId;
                int m_combatEfct;
                public CombatEffectiveItem (int charId, int combatEfct) {
                    m_charId = charId;
                    m_combatEfct = combatEfct;
                }
            }
            private SortedSet<CombatEffectiveItem> m_rankSet = new SortedSet<CombatEffectiveItem> ();
            public List<NO_FightCapacityRankInfo> GetTopCombatEfctRnkList () {
                return new List<NO_FightCapacityRankInfo> ();
            }
            public int GetRank (int charId) {
                // TODO: 
                return 100;
            }
        }
    }
}