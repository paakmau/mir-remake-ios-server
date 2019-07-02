using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
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
            // dds 与 client
            EM_Unit.s_instance.SaveCharacter (charObj);
            m_networkService.SendServerCommand (SC_ApplySelfMainAttribute.Instance (
                netId,
                charObj.m_Strength,
                charObj.m_Intelligence,
                charObj.m_Agility,
                charObj.m_Spirit));
            // TODO: 属性加点
        }
        public void CommandGainCurrency (int netId, CurrencyType type, long dC) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyUpdateCurrency (charObj, type, dC);
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
    }
}