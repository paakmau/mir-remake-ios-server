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
        private const int c_maxLevel = 100;
        private const float c_syncPosTime = 5;
        private float m_syncPosTimer = 0;
        public GL_CharacterAttribute (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            m_syncPosTimer += dT;
            bool needSyncPos = false;
            while (m_syncPosTimer >= c_syncPosTime) {
                m_syncPosTimer -= c_syncPosTime;
                needSyncPos = true;
            }
            if (needSyncPos) {
                var charEn = EM_Unit.s_instance.GetCharacterEnumerator ();
                while (charEn.MoveNext ())
                    EM_Unit.s_instance.SaveCharacterPosition (charEn.Current.Value);
            }
        }
        public override void NetworkTick () { }
        public void CommandGainExperience (int netId, int exp) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyGainExperience (charObj, exp);
        }
        public void CommandRequireCharacterAttribute (int netId, int tarNetId) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (tarNetId);
            if (charObj == null) return;
            m_networkService.SendServerCommand (SC_SendAllCharacterAttribute.Instance (netId, charObj.GetAttrNo ()));
        }
        public void CommandApplyDistributePoints (int netId, short str, short intl, short agl, short spr) {
            E_Character charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            charObj.DistributePoints (str, intl, agl, spr);
            // 角色加点后, 具体属性变化
            this.MainPointToConAttr (charObj);
            // 战斗力变化
            GL_CharacterCombatEfct.s_instance.NotifyCombatEffectivenessChange (charObj);
            // dds 与 client
            EM_Unit.s_instance.SaveCharacterAttribute (charObj);
            m_networkService.SendServerCommand (SC_ApplySelfMainAttribute.Instance (
                netId,
                charObj.m_strength,
                charObj.m_intelligence,
                charObj.m_agility,
                charObj.m_spirit));
        }
        public void CommandGainCurrency (int netId, CurrencyType type, long dC) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            NotifyUpdateCurrency (charObj, type, dC);
        }
        public E_Character NotifyInitCharacter (int netId, int charId) {
            E_Character newChar = EM_Unit.s_instance.InitCharacter (netId, charId);
            if (newChar == null)
                return null;
            MainPointToConAttr (newChar);
            // client
            m_networkService.SendServerCommand (SC_InitSelfAttribute.Instance (
                netId,
                newChar.m_Occupation,
                newChar.m_name,
                newChar.m_Level,
                newChar.m_experience,
                newChar.m_strength,
                newChar.m_intelligence,
                newChar.m_agility,
                newChar.m_spirit,
                newChar.m_TotalMainPoint,
                newChar.m_virtualCurrency,
                newChar.m_chargeCurrency,
                newChar.m_position));
            return newChar;
        }
        public void NotifyRemoveCharacter (E_Character charObj) {
            EM_Unit.s_instance.RemoveCharacter (charObj.m_networkId);
        }
        public void NotifyGainExperience (E_Character charObj, int exp) {
            if (charObj.m_Level == c_maxLevel)
                return;
            charObj.m_experience += exp;
            var dLv = charObj.TryLevelUp ();
            if (dLv > 0)
                GL_CharacterCombatEfct.s_instance.NotifyCombatEffectivenessChange (charObj);
            // dds 与 client
            EM_Unit.s_instance.SaveCharacterAttribute (charObj);
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
            if (type == CurrencyType.CHARGE)
                charObj.m_chargeCurrency += dC;
            else
                charObj.m_virtualCurrency += dC;
            EM_Unit.s_instance.SaveCharacterWallet (charObj);
            // client
            m_networkService.SendServerCommand (SC_ApplySelfCurrency.Instance (
                charObj.m_networkId, charObj.m_virtualCurrency, charObj.m_chargeCurrency));
        }
        private void MainPointToConAttr (E_Character charObj) {
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, (int) (charObj.m_strength * 0.55));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.ATTACK, (int) (charObj.m_agility * 0.2));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.CRITICAL_BONUS, (int) (charObj.m_agility * 0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.CRITICAL_BONUS, (int) (charObj.m_agility * 0.0015));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAX_MP, (int) (charObj.m_intelligence * 2));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, (int) (charObj.m_spirit * 0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAGIC, (int) (charObj.m_intelligence * 0.5));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND, (int) (charObj.m_spirit * 0.05));
            charObj.SetMainPointConAttr (ActorUnitConcreteAttributeType.MAX_HP, (int) (charObj.m_spirit * 2.5));
        }
    }
}