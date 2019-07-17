using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 处理角色排行榜逻辑
    /// </summary>
    class GL_CharacterCombatEfct : GameLogicBase {
        public static GL_CharacterCombatEfct s_instance;
        public GL_CharacterCombatEfct (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandGetCombatEffectivenessRank (int netId, OccupationType ocp) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var topCombatEfctRnkCharIdAndCombatEfctList = EM_Rank.s_instance.GetTopCombatEfctRnkCharIdAndCombatEfctList (ocp, 15);
            var topCombatEfctRnkList = new List<NO_FightCapacityRankInfo> (topCombatEfctRnkCharIdAndCombatEfctList.Count);
            for (int i = 0; i < topCombatEfctRnkCharIdAndCombatEfctList.Count; i++) {
                var topCharId = topCombatEfctRnkCharIdAndCombatEfctList[i].Item1;
                var topCharCombatEfct = topCombatEfctRnkCharIdAndCombatEfctList[i].Item2;
                topCombatEfctRnkList.Add (new NO_FightCapacityRankInfo (topCharId, "匿名", 14, (short) i, topCharCombatEfct, "无", (byte) 0));
            }
            var myCombatEfctAndRank = EM_Rank.s_instance.GetCombatEfctAndRank (ocp, charObj.m_characterId, charObj.m_Occupation);
            m_networkService.SendServerCommand (SC_SendFightCapacityRank.Instance (netId, topCombatEfctRnkList, myCombatEfctAndRank.Item1, (short) myCombatEfctAndRank.Item2));
        }
        public void NotifyCombatEffectivenessChange (E_Character charObj) {
            EM_Rank.s_instance.UpdateCharCombatEfct (charObj.m_characterId, charObj.m_Occupation, charObj.m_Attack, charObj.m_Intelligence, charObj.m_MaxHp, charObj.m_MaxMp, charObj.m_Defence, charObj.m_Agility, charObj.m_CriticalRate, charObj.m_CriticalBonus, charObj.m_HitRate, charObj.m_DodgeRate);
        }
        public void NotifyInitCharacter (E_Character charObj) {
            NotifyCombatEffectivenessChange (charObj);
        }
        public void NotifyRemoveCharacter (E_Character charObj) {
            EM_Rank.s_instance.RemoveCharacter (charObj);
        }
    }
}