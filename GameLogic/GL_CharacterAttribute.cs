using System;
using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理角色属性相关
    /// 升级, 属性点分配
    /// 装备相关属性
    /// </summary>
    class GL_CharacterAttribute : GameLogicBase {
        public static GL_CharacterAttribute s_instance;
        const int c_maxLevel = 100;
        public GL_CharacterAttribute (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandGainExperience (int netId, int exp) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyGainExperience (charObj, exp);
        }
        public void CommandApplyDistributePoints (int netId, short str, short intl, short agl, short spr) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            charObj.DistributePoints (str, intl, agl, spr);
            // 角色加点后, 具体属性变化
            this.MainPointToConAttr (charObj);
            // 战斗力变化
            NotifyCombatEffectivenessChange (charObj);
            // dds 与 client
            EM_Unit.s_instance.SaveCharacter (charObj);
            m_networkService.SendServerCommand (SC_ApplySelfMainAttribute.Instance (
                netId,
                charObj.m_Strength,
                charObj.m_Intelligence,
                charObj.m_Agility,
                charObj.m_Spirit));
        }
        public void CommandGainCurrency (int netId, CurrencyType type, long dC) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyUpdateCurrency (charObj, type, dC);
        }
        public void CommandGetCombatEffectivenessRank (int netId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            var topCombatEfctRnkCharIdList = EM_Rank.s_instance.GetTopCombatEfctRnkCharIdList (10);
            var topCombatEfctRnkList = new List<NO_FightCapacityRankInfo> (topCombatEfctRnkCharIdList.Count);
            for (int i = 0; i < topCombatEfctRnkCharIdList.Count; i++) {
                int topCharId = topCombatEfctRnkCharIdList[i];
                // TODO: 
                topCombatEfctRnkList.Add (new NO_FightCapacityRankInfo (topCharId, "nzy", 80, (short) i, 23333, "nihao", (byte) 0));
            }
            var myRank = EM_Rank.s_instance.GetCombatEfctRank (charObj.m_characterId);
            m_networkService.SendServerCommand (SC_SendFightCapacityRank.Instance (netId, topCombatEfctRnkList, charObj.m_combatEffectiveness, myRank));
        }
        public void NotifyCombatEffectivenessChange (E_Character unit) {
            double res = 0;
            switch (unit.m_Occupation) {
                case OccupationType.WARRIOR:
                    res = Math.Pow (unit.m_Attack, 1.5);
                    break;
                case OccupationType.ROGUE:
                    res = 2.5 * Math.Pow (unit.m_Attack, 1.5);
                    break;
                case OccupationType.MAGE:
                    res = 0.8 * Math.Pow (unit.m_Intelligence, 1.5);
                    break;
                case OccupationType.TAOIST:
                    res = 1.3 * Math.Pow (unit.m_Intelligence, 1.5);
                    break;
            }
            res = res + Math.Pow (unit.m_MaxHp, 0.5) * 0.5;
            res = res + Math.Pow (unit.m_MaxMp, 0.4) * 0.3;
            res = res + Math.Pow (unit.m_Defence * unit.m_Agility, 0.75);
            res = res * (1 + 0.72 * unit.m_CriticalRate * 0.01 * unit.m_CriticalBonus);
            res = res * unit.m_HitRate / (1 - unit.m_DodgeRate * 0.01f) * 0.01f;
            unit.m_combatEffectiveness = (int) (res);
        }
        public E_Character NotifyInitCharacter (int netId, int charId) {
            E_Character newChar = EM_Unit.s_instance.InitCharacter (netId, charId);
            MainPointToConAttr (newChar);
            NotifyCombatEffectivenessChange (newChar);
            // client
            m_networkService.SendServerCommand (SC_InitSelfAttribute.Instance (
                netId,
                newChar.m_Occupation,
                newChar.m_Level,
                newChar.m_experience,
                newChar.m_Strength,
                newChar.m_Intelligence,
                newChar.m_Agility,
                newChar.m_Spirit,
                newChar.m_TotalMainPoint,
                newChar.m_VirtualCurrency,
                newChar.m_ChargeCurrency));
            return newChar;
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Unit.s_instance.RemoveCharacter (netId);
        }
        public void NotifyGainExperience (E_Character charObj, int exp) {
            if (charObj.m_Level == c_maxLevel)
                return;
            charObj.m_experience += exp;
            var dLv = charObj.TryLevelUp ();
            if (dLv > 0)
                NotifyCombatEffectivenessChange (charObj);
            // dds 与 client
            EM_Unit.s_instance.SaveCharacter (charObj);
            m_networkService.SendServerCommand (SC_ApplySelfLevelAndExp.Instance (
                charObj.m_networkId, charObj.m_Level, charObj.m_experience, charObj.m_TotalMainPoint));
        }
        public void NotifyKillUnit (E_Character killer, E_Unit dead) {
            var expGain = 0;
            expGain += dead.m_Level * 30;
            expGain += dead.m_Attack * 2;
            expGain += dead.m_Magic * 2;
            expGain += dead.m_Defence * 3;
            expGain += dead.m_Resistance * 3;
            NotifyGainExperience (killer, expGain);
        }
        public void NotifyConcreteAttributeChange (E_Character charObj, List < (ActorUnitConcreteAttributeType, int) > attrList) {
            for (int i = 0; i < attrList.Count; i++)
                charObj.AddEquipConAttr (attrList[i].Item1, attrList[i].Item2);
        }
        public void NotifyUpdateCurrency (E_Character charObj, CurrencyType type, long dC) {
            // 实例 与 数据
            charObj.m_currencyDict[type] += dC;
            EM_Unit.s_instance.SaveCharacter (charObj);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                charObj.m_networkId, charObj.m_VirtualCurrency, charObj.m_ChargeCurrency));
        }
        private void MainPointToConAttr (E_Character charObj) {
            // charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, charObj.m_Strength * 233);
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, (int) (charObj.m_Strength * 0.55));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, (int) (charObj.m_Agility * 0.2));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.CRITICAL_BONUS, (int) (charObj.m_Agility * 0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.CRITICAL_BONUS, (int) (charObj.m_Agility * 0.0015));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAX_MP, (int) (charObj.m_Intelligence * 2));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, (int) (charObj.m_Spirit * 0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAGIC, (int) (charObj.m_Intelligence * 0.5));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND, (int) (charObj.m_Spirit * 0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAX_HP, (int) (charObj.m_Spirit * 2.5));
        }
    }
}