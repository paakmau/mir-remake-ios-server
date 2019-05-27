using System.Collections.Generic;
using LitJson;
using System;
using System.IO;

namespace MirRemakeBackend.Data {
    interface IDS_Item {
        DO_Item[] GetAllItem();
        DO_Equipment[] GetAllEquipment ();
        DO_Consumable[] GetAllConsumable ();
        DO_Gem[] GetAllGem ();
    }
    class IDS_ItemImpl {
        private DO_Item[] items;
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
                        int.Parse(s_equipmentDatas[i]["AttributeAttachArray"][m].ToString().Split(' ').ToString()));
                }
                

            }
            return equipments;
        }
        public DO_Consumable[] GetAllConsumables() {
            return consumables;
        }
        public DO_Gem[] GetAllGem() {
            return gems;
        }
    }
}