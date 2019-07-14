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
        private IDDS_CombatEfct m_dds;
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
        public void LoadAllCharacter () {
            // TODO: 等yzj完成
            // var ddoList = m_dds.GetAllMixCombatEfct ();
        }
        public void UpdateCharCombatEfct (int charId, OccupationType ocp, int atk, int intl, int maxHp, int maxMp, int def, int agl, int criticalRate, int criticalBonus, int hitRate, int dodgeRate) {
            var combatEfct = AttrToCombatEfct (ocp, atk, intl, maxHp, maxMp, def, agl, criticalRate, criticalBonus, hitRate, dodgeRate);
            UpdateCharCombatEfct (charId, combatEfct);
        }
        private void UpdateCharCombatEfct (int charId, int combatEfct) {
            int oriCombatEfct;
            m_charIdAndOriCombatEfctDict.TryGetValue (charId, out oriCombatEfct);
            m_rankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct));
            m_rankTree.Insert (new CombatEffectiveItem (charId, combatEfct));
            m_charIdAndOriCombatEfctDict[charId] = combatEfct;
        }
        public List < (int, int) > GetTopCombatEfctRnkCharIdAndCombatEfctList (int num) {
            var res = new List < (int, int) > (num);
            for (int i = 0; i < num; i++) {
                var v = m_rankTree.GetKElementInOrder (i);
                if (v != null)
                    res.Add ((v.m_charId, v.m_combatEfct));
            }
            return res;
        }
        public (int, int) GetCombatEfctAndRank (int charId) {
            var combatEfct = m_charIdAndOriCombatEfctDict[charId];
            var rank = m_rankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct));
            return (combatEfct, rank);
        }
        private int AttrToCombatEfct (OccupationType ocp, int atk, int intl, int maxHp, int maxMp, int def, int agl, int criticalRate, int criticalBonus, int hitRate, int dodgeRate) {
            double res = 0;
            switch (ocp) {
                case OccupationType.WARRIOR:
                    res = Math.Pow (atk, 1.5);
                    break;
                case OccupationType.ROGUE:
                    res = 2.5 * Math.Pow (atk, 1.5);
                    break;
                case OccupationType.MAGE:
                    res = 0.8 * Math.Pow (intl, 1.5);
                    break;
                case OccupationType.TAOIST:
                    res = 1.3 * Math.Pow (intl, 1.5);
                    break;
            }
            res = res + Math.Pow (maxHp, 0.5) * 0.5;
            res = res + Math.Pow (maxMp, 0.4) * 0.3;
            res = res + Math.Pow (def * agl, 0.75);
            res = res * (1 + 0.72 * criticalRate * 0.01 * criticalBonus);
            res = res * hitRate / (1 - dodgeRate * 0.01f) * 0.01f;
            return (int) res;
        }
    }
}