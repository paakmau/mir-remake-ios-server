using System;
using System.Collections.Generic;
using Algorithms.DataStructures.Tree;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 处理排行榜
    /// </summary>
    class EM_Rank : EntityManagerBase {
        public static EM_Rank s_instance;
        private class CombatEffectiveItem : IComparable<CombatEffectiveItem> {
            public int m_charId;
            public int m_combatEfct;
            public CombatEffectiveItem (int charId, int combatEfct) {
                m_charId = charId;
                m_combatEfct = combatEfct;
            }
            public int CompareTo (CombatEffectiveItem b) {
                if (m_combatEfct != b.m_combatEfct)
                    return m_combatEfct - b.m_combatEfct;
                return m_charId - b.m_charId;
            }
        }
        private AVLTree<CombatEffectiveItem> m_rankTree = new AVLTree<CombatEffectiveItem> ();
        private Dictionary<int, int> m_charIdAndOriCombatEfctDict = new Dictionary<int, int> ();
        public void InitAllCharacter (List < (int, int) > charIdAndCombatEfctList) {
            for (int i = 0; i < charIdAndCombatEfctList.Count; i++)
                UpdateCharCombatEfct (charIdAndCombatEfctList[i].Item1, charIdAndCombatEfctList[i].Item2);
        }
        public void UpdateCharCombatEfct (int charId, int combatEfct) {
            int oriCombatEfct;
            m_charIdAndOriCombatEfctDict.TryGetValue (charId, out oriCombatEfct);
            m_rankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct));
            m_rankTree.Insert (new CombatEffectiveItem (charId, combatEfct));
            m_charIdAndOriCombatEfctDict[charId] = combatEfct;
        }
        public List < (int, int) > GetTopCombatEfctRnkCharIdAndCombatEfctList (int num) {
            var res = new List < (int, int) > ();
            for (int i = 0; i < num; i++) {
                var v = m_rankTree.GetKElementInOrder (i);
                res.Add ((v.m_charId, v.m_combatEfct));
            }
            return res;
        }
        public int GetCombatEfctRank (int charId) {
            var combatEfct = m_charIdAndOriCombatEfctDict[charId];
            return m_rankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct));
        }
    }
}