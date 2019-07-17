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
        public class CombatEffectiveItem : IComparable<CombatEffectiveItem> {
            public int m_charId;
            public int m_combatEfct;
            public string m_name;
            public OccupationType m_ocp;
            public short m_level;
            public CombatEffectiveItem (int charId, int combatEfct, string name, OccupationType ocp, short lv) {
                m_charId = charId;
                m_combatEfct = combatEfct;
                m_name = name;
                m_ocp = ocp;
                m_level = lv;
            }
            public int CompareTo (CombatEffectiveItem b) {
                if (m_combatEfct != b.m_combatEfct)
                    return b.m_combatEfct - m_combatEfct;
                return m_charId - b.m_charId;
            }
        }
        private AVLTree<CombatEffectiveItem> m_allRankTree = new AVLTree<CombatEffectiveItem> ();
        private AVLTree<CombatEffectiveItem> m_warriorRankTree = new AVLTree<CombatEffectiveItem> ();
        private AVLTree<CombatEffectiveItem> m_rogueRankTree = new AVLTree<CombatEffectiveItem> ();
        private AVLTree<CombatEffectiveItem> m_mageRankTree = new AVLTree<CombatEffectiveItem> ();
        private AVLTree<CombatEffectiveItem> m_taoistRankTree = new AVLTree<CombatEffectiveItem> ();
        private Dictionary<int, int> m_charIdAndOriCombatEfctDict = new Dictionary<int, int> ();
        public EM_Rank (IDDS_CombatEfct dds) {
            m_dds = dds;
            LoadAllCharacter ();
        }
        public void LoadAllCharacter () {
            var ddoArr = m_dds.GetAllMixCombatEfct ();
            foreach (var ddo in ddoArr)
                UpdateCharCombatEfct (ddo.m_charId, ddo.m_combatEfct, ddo.m_ocp, ddo.m_name, ddo.m_level);
        }
        public void RemoveCharacter (E_Character charObj) {
            int combatEfct;
            if (!m_charIdAndOriCombatEfctDict.TryGetValue (charObj.m_characterId, out combatEfct)) return;
            m_dds.UpdateMixCombatEfct (new DDO_CombatEfct (charObj.m_characterId, combatEfct, charObj.m_Occupation, charObj.m_name, charObj.m_Level));
        }
        public void UpdateCharCombatEfct (int charId, OccupationType ocp, string name, short lv, int atk, int intl, int maxHp, int maxMp, int def, int agl, int criticalRate, int criticalBonus, int hitRate, int dodgeRate) {
            var combatEfct = AttrToCombatEfct (ocp, atk, intl, maxHp, maxMp, def, agl, criticalRate, criticalBonus, hitRate, dodgeRate);
            UpdateCharCombatEfct (charId, combatEfct, ocp, name, lv);
        }
        private void UpdateCharCombatEfct (int charId, int combatEfct, OccupationType ocp, string name, short lv) {
            int oriCombatEfct;
            m_charIdAndOriCombatEfctDict.TryGetValue (charId, out oriCombatEfct);
            m_allRankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct, null, 0, 0));
            m_allRankTree.Insert (new CombatEffectiveItem (charId, combatEfct, name, ocp, lv));
            switch (ocp) {
                case OccupationType.WARRIOR:
                    m_warriorRankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct, null, 0, 0));
                    m_warriorRankTree.Insert (new CombatEffectiveItem (charId, combatEfct, name, ocp, lv));
                    break;
                case OccupationType.ROGUE:
                    m_rogueRankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct, null, 0, 0));
                    m_rogueRankTree.Insert (new CombatEffectiveItem (charId, combatEfct, name, ocp, lv));
                    break;
                case OccupationType.MAGE:
                    m_mageRankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct, null, 0, 0));
                    m_mageRankTree.Insert (new CombatEffectiveItem (charId, combatEfct, name, ocp, lv));
                    break;
                case OccupationType.TAOIST:
                    m_taoistRankTree.Remove (new CombatEffectiveItem (charId, oriCombatEfct, null, 0, 0));
                    m_taoistRankTree.Insert (new CombatEffectiveItem (charId, combatEfct, name, ocp, lv));
                    break;
            }
            m_charIdAndOriCombatEfctDict[charId] = combatEfct;
        }
        public List<CombatEffectiveItem> GetTopCombatEfctRnkCharIdAndCombatEfctList (OccupationType ocp, int num) {
            var res = new List<CombatEffectiveItem> (num);
            for (int i = 0; i < num; i++) {
                CombatEffectiveItem v = null;
                switch (ocp) {
                    case OccupationType.ALL:
                        v = m_allRankTree.GetKElementInOrder (i);
                        break;
                    case OccupationType.WARRIOR:
                        v = m_warriorRankTree.GetKElementInOrder (i);
                        break;
                    case OccupationType.ROGUE:
                        v = m_rogueRankTree.GetKElementInOrder (i);
                        break;
                    case OccupationType.MAGE:
                        v = m_mageRankTree.GetKElementInOrder (i);
                        break;
                    case OccupationType.TAOIST:
                        v = m_taoistRankTree.GetKElementInOrder (i);
                        break;
                }
                if (v != null)
                    res.Add (v);
            }
            return res;
        }
        public (int, int) GetCombatEfctAndRank (OccupationType ocp, int charId, OccupationType myOcp) {
            var combatEfct = m_charIdAndOriCombatEfctDict[charId];
            if (myOcp != ocp) return (combatEfct, -1);
            int rank = -1;
            switch (ocp) {
                case OccupationType.ALL:
                    rank = m_allRankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct, null, 0, 0));
                    break;
                case OccupationType.WARRIOR:
                    rank = m_warriorRankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct, null, 0, 0));
                    break;
                case OccupationType.ROGUE:
                    rank = m_rogueRankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct, null, 0, 0));
                    break;
                case OccupationType.MAGE:
                    rank = m_mageRankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct, null, 0, 0));
                    break;
                case OccupationType.TAOIST:
                    rank = m_taoistRankTree.GetIndexOfElement (new CombatEffectiveItem (charId, combatEfct, null, 0, 0));
                    break;
            }
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