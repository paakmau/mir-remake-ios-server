using System.Collections.Generic;
using LitJson;
using System;
using System.IO;

namespace MirRemakeBackend.Data {
    interface IDS_Item {
        DO_Item[] GetAllItem();
        DO_Equipment[] GetAllEquipment ();//done
        DO_Consumable[] GetAllConsumable ();
        DO_Gem[] GetAllGem ();
    }
    class DS_ItemImpl : IDS_Item {
        private DO_Item[] items=null;
        private DO_Equipment[] equipments;
        private DO_Consumable[] consumables;
        private DO_Gem[] gems;
        private JsonData s_equipmentDatas;
        private JsonData s_consumableDatas;
        private JsonData s_gemDatas;
        public DO_Item[] GetAllItem() {
            return items;
        }
        public DO_Equipment[] GetAllEquipment() {
            string jsonFile= File.ReadAllText("Data/D_Equipment.json");
            s_equipmentDatas = JsonMapper.ToObject(jsonFile);
            equipments = new DO_Equipment[s_equipmentDatas.Count];
            for(int i = 0; i < s_equipmentDatas.Count; i++) {
                DO_Equipment equipment = new DO_Equipment();
                equipment.m_itemId = short.Parse(s_equipmentDatas[i]["EquipmentID"].ToString());
                equipment.m_equipPosition = (EquipmentPosition)Enum.Parse
                    (typeof(EquipmentPosition),s_equipmentDatas[i]["EquipmentPosition"].ToString());
                equipment.m_attrWave = float.Parse(s_equipmentDatas[i]["Wave"].ToString());
                equipment.m_validOccupation = (OccupationType)Enum.Parse
                    (typeof(OccupationType),s_equipmentDatas[i]["Occupation"].ToString());
                equipment.m_equipLevelInNeed = short.Parse(s_equipmentDatas[i]["LevelInNeed"].ToString());
                equipment.m_quality = (ItemQuality)Enum.Parse(
                    typeof(ItemQuality),s_equipmentDatas[i]["Quality"].ToString());
                equipment.m_equipmentAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_equipmentDatas[i]["AttributeAttachArray"].Count];
                for(int m = 0; m < s_equipmentDatas[i]["AttributeAttachArray"].Count; m++) {
                    equipment.m_equipmentAttributeArr[m] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType),s_equipmentDatas[i]["AttributeAttachArray"][m].ToString().Split(' ')[0]),
                        int.Parse(s_equipmentDatas[i]["AttributeAttachArray"][m].ToString().Split(' ')[1]));
                }
                equipments[i] = equipment;
                

            }
            return equipments;
        }
        public DO_Consumable[] GetAllConsumable() {
            string jsonFile = File.ReadAllText("Data/D_Consumable.json");
            s_consumableDatas = JsonMapper.ToObject(jsonFile);
            consumables = new DO_Consumable[s_consumableDatas.Count];
            for(int i = 0; i < s_consumableDatas.Count; i++) {
                DO_Consumable consumable = new DO_Consumable();
                consumable.m_itemId = short.Parse(s_consumableDatas[i]["ID"].ToString());
                DO_Effect effect = new DO_Effect();
                effect.m_type = EffectType.CONSUMABLE;
                effect.m_deltaHp = int.Parse(s_consumableDatas[i]["EffectDeltaHP"].ToString());
                effect.m_deltaMp = int.Parse(s_consumableDatas[i]["EffectDeltaMP"].ToString());
                effect.m_statusIdAndValueAndTimeArr = new ValueTuple<short,float,float>[s_consumableDatas[i]["StatusAttachArray"].Count];
                for(int mm = 0; mm < s_consumableDatas[i]["StatusAttachArray"].Count; mm++) {
                    effect.m_statusIdAndValueAndTimeArr[mm] = new ValueTuple<short, float, float>
                        (short.Parse(s_consumableDatas[i]["StatusAttachArray"][mm]["StatusID"].ToString()),
                        float.Parse(s_consumableDatas[i]["StatusAttachArray"][mm]["Value"].ToString()),
                        float.Parse(s_consumableDatas[i]["StatusAttachArray"][mm]["LastingTime"].ToString()));
                }
                consumable.m_effect = effect;
                consumables[i] = consumable;
            }
            return consumables;
        }
        public DO_Gem[] GetAllGem() {
            string jsonFile = File.ReadAllText("Data/D_Gem.json");
            s_gemDatas = JsonMapper.ToObject(jsonFile);
            gems = new DO_Gem[s_gemDatas.Count];
            for (short i = 0; i < s_gemDatas.Count; i++)
            {
                gems[i] = new DO_Gem();
                gems[i].m_itemId = (short)(short.Parse(s_gemDatas[i]["ID"].ToString()));
                gems[i].m_equipmentAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_gemDatas[i]["ConcreteAttributionTable"].Count];
                for (int x = 0; x < s_gemDatas[i]["ConcreteAttributionTable"].Count; x++)
                {
                    gems[i].m_equipmentAttributeArr[x] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType), s_gemDatas[i]["ConcreteAttributionTable"][x].ToString().Split(' ')[0]), int.Parse(s_gemDatas[i]["ConcreteAttributionTable"][x].ToString().Split(' ')[1]));
                }
            }
            return gems;
        }

    }
}