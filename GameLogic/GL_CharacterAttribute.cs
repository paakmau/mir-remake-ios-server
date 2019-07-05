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
        public E_Character NotifyInitCharacter (int netId, int charId) {
            E_Character newChar = EM_Unit.s_instance.InitCharacter (netId, charId);
            MainPointToConAttr(newChar);
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
            charObj.TryLevelUp ();
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
            // TODO: 根据角色属性点 直接计算 具体属性
            // charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, charObj.m_Strength * 233);
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, (int)(charObj.m_Strength*0.55));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, (int)(charObj.m_Agility*0.2));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.CRITICAL_BONUS, (int)(charObj.m_Agility*0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.CRITICAL_BONUS, (int)(charObj.m_Agility*0.0015));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAX_MP, (int)(charObj.m_Intelligence*2));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, (int)(charObj.m_Spirit*0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAGIC, (int)(charObj.m_Intelligence*0.5));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND, (int)(charObj.m_Spirit*0.05));
        }
    }
}