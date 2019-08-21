using System;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace MirRemakeBackend.Data {
    interface IDS_Item {
        (DO_Item, DO_Equipment) [] GetAllEquipment (); //done
        (DO_Item, DO_Consumable) [] GetAllConsumable ();
        (DO_Item, DO_Gem) [] GetAllGem ();
        DO_Item[] GetAllMaterial ();
        DO_Item[] GetAllEnchantment ();
    }
    class DS_ItemImpl : IDS_Item {
        private DO_Item[] items = null;
        /* private DO_Equipment[] equipments;
        private DO_Consumable[] consumables;
        private DO_Gem[] gems;*/
        private JsonData s_equipmentDatas;
        private JsonData s_consumableDatas;
        private JsonData s_gemDatas;
        private JsonData s_enchantmentDatas;


        public (DO_Item, DO_Equipment) [] GetAllEquipment () {
            string jsonFile = File.ReadAllText ("Data/D_Equipment.json");
            s_equipmentDatas = JsonMapper.ToObject (jsonFile);
            (DO_Item, DO_Equipment) [] res = new (DO_Item, DO_Equipment) [s_equipmentDatas.Count];
            for (int i = 0; i < s_equipmentDatas.Count; i++) {
                DO_Equipment equipment = new DO_Equipment ();
                DO_Item item = new DO_Item ();
                item.m_itemId = short.Parse (s_equipmentDatas[i]["EquipmentID"].ToString ());
                item.m_quality = (ItemQuality) Enum.Parse (
                    typeof (ItemQuality), s_equipmentDatas[i]["Quality"].ToString ());
                item.m_maxNum = 1;
                item.m_sellPrice = 450 * (int.Parse (s_equipmentDatas[i]["LevelInNeed"].ToString ())/10);
                item.m_buyPrice = 25*item.m_sellPrice;
                item.m_type = ItemType.EQUIPMENT;
                equipment.m_itemId = short.Parse (s_equipmentDatas[i]["EquipmentID"].ToString ());
                equipment.m_equipPosition = (EquipmentPosition) Enum.Parse (typeof (EquipmentPosition), s_equipmentDatas[i]["EquipmentPosition"].ToString ());
                equipment.m_attrWave = float.Parse (s_equipmentDatas[i]["Wave"].ToString ());
                equipment.m_validOccupation = (OccupationType) Enum.Parse (typeof (OccupationType), s_equipmentDatas[i]["Occupation"].ToString ());
                equipment.m_equipLevelInNeed = short.Parse (s_equipmentDatas[i]["LevelInNeed"].ToString ());
                equipment.m_quality = (ItemQuality) Enum.Parse (
                    typeof (ItemQuality), s_equipmentDatas[i]["Quality"].ToString ());
                equipment.m_equipmentAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_equipmentDatas[i]["AttributeAttachArray"].Count];
                for (int m = 0; m < s_equipmentDatas[i]["AttributeAttachArray"].Count; m++) {
                    equipment.m_equipmentAttributeArr[m] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType) Enum.Parse (typeof (ActorUnitConcreteAttributeType), s_equipmentDatas[i]["AttributeAttachArray"][m].ToString ().Split (' ') [0]),
                            int.Parse (s_equipmentDatas[i]["AttributeAttachArray"][m].ToString ().Split (' ') [1]));
                }
                res[i] = new ValueTuple<DO_Item, DO_Equipment> (item, equipment);

            }
            return res;
        }
        public (DO_Item, DO_Consumable) [] GetAllConsumable () {
            string jsonFile = File.ReadAllText ("Data/D_Consumable.json");
            s_consumableDatas = JsonMapper.ToObject (jsonFile);
            (DO_Item, DO_Consumable) [] res = new (DO_Item, DO_Consumable) [s_consumableDatas.Count];
            for (int i = 0; i < s_consumableDatas.Count; i++) {
                DO_Consumable consumable = new DO_Consumable ();
                DO_Item item = new DO_Item ();
                item.m_itemId = short.Parse (s_consumableDatas[i]["ID"].ToString ());
                item.m_maxNum = short.Parse (s_consumableDatas[i]["MaxNum"].ToString ());
                item.m_sellPrice = long.Parse (s_consumableDatas[i]["Price"].ToString ());
                item.m_buyPrice = item.m_sellPrice*3;
                item.m_type = ItemType.CONSUMABLE;
                item.m_quality = (ItemQuality) Enum.Parse (
                    typeof (ItemQuality), s_consumableDatas[i]["Quality"].ToString ());
                consumable.m_itemId = short.Parse (s_consumableDatas[i]["ID"].ToString ());
                DO_Effect effect = new DO_Effect ();
                effect.m_type = EffectType.CONSUMABLE;
                effect.m_deltaHp = int.Parse (s_consumableDatas[i]["EffectDeltaHP"].ToString ());
                effect.m_deltaMp = int.Parse (s_consumableDatas[i]["EffectDeltaMP"].ToString ());
                effect.m_statusIdAndValueAndTimeArr = new ValueTuple<short, float, float>[s_consumableDatas[i]["StatusAttachArray"].Count];
                for (int mm = 0; mm < s_consumableDatas[i]["StatusAttachArray"].Count; mm++) {
                    effect.m_statusIdAndValueAndTimeArr[mm] = new ValueTuple<short, float, float>
                        (short.Parse (s_consumableDatas[i]["StatusAttachArray"][mm]["StatusID"].ToString ()),
                            float.Parse (s_consumableDatas[i]["StatusAttachArray"][mm]["Value"].ToString ()),
                            float.Parse (s_consumableDatas[i]["StatusAttachArray"][mm]["LastingTime"].ToString ()));
                }
                consumable.m_effect = effect;
                res[i] = new ValueTuple<DO_Item, DO_Consumable> (item, consumable);

            }
            return res;
        }
        public (DO_Item, DO_Gem) [] GetAllGem () {
            string jsonFile = File.ReadAllText ("Data/D_Gem.json");
            s_gemDatas = JsonMapper.ToObject (jsonFile);
            (DO_Item, DO_Gem) [] res = new (DO_Item, DO_Gem) [s_gemDatas.Count];
            for (short i = 0; i < s_gemDatas.Count; i++) {
                DO_Gem gem = new DO_Gem ();
                DO_Item item = new DO_Item ();
                item.m_itemId = short.Parse (s_gemDatas[i]["ID"].ToString ());
                item.m_maxNum = 10;
                item.m_sellPrice = 10000L;
                item.m_buyPrice = 50000L;
                item.m_type = ItemType.GEM;
                item.m_quality = (ItemQuality) Enum.Parse (
                    typeof (ItemQuality), s_gemDatas[i]["Quality"].ToString ());
                gem.m_itemId = (short) (short.Parse (s_gemDatas[i]["ID"].ToString ()));
                gem.m_equipmentAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_gemDatas[i]["ConcreteAttributionTable"].Count];
                for (int x = 0; x < s_gemDatas[i]["ConcreteAttributionTable"].Count; x++) {
                    gem.m_equipmentAttributeArr[x] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType) Enum.Parse (typeof (ActorUnitConcreteAttributeType), s_gemDatas[i]["ConcreteAttributionTable"][x].ToString ().Split (' ') [0]), int.Parse (s_gemDatas[i]["ConcreteAttributionTable"][x].ToString ().Split (' ') [1]));
                }
                res[i] = (item, gem);
            }
            return res;
        }
        public DO_Item[] GetAllMaterial () {
            string jsonFile = File.ReadAllText ("Data/D_Material.json");
            JsonData s_itemDatas = JsonMapper.ToObject (jsonFile);
            DO_Item[] res=new DO_Item[s_itemDatas.Count];
            for (int i=0;i<s_itemDatas.Count;i++){
                res[i].m_itemId=short.Parse(s_itemDatas[i]["ID"].ToString());
                res[i].m_sellPrice=150;
                res[i].m_buyPrice =100;
                res[i].m_maxNum=short.Parse(s_itemDatas[i]["MaxNum"].ToString());
                res[i].m_quality=ItemQuality.COMMON;
                res[i].m_type=ItemType.MATERIAL;
            }
            return res;
        }

        public DO_Item[] GetAllEnchantment(){
            string jsonFile = File.ReadAllText("Data/D_EnchantMent.json");
            s_enchantmentDatas=JsonMapper.ToObject(jsonFile);
            DO_Item[] result=new DO_Item[s_enchantmentDatas.Count];
            for(int i=0;i<s_enchantmentDatas.Count;i++){
                result[i].m_itemId=short.Parse(s_enchantmentDatas[i]["ID"].ToString());
                result[i].m_quality=(ItemQuality)Enum.Parse(typeof(ItemQuality),s_enchantmentDatas[i]["Quality"].ToString());
                result[i].m_sellPrice = 600*(2*(int)(result[i].m_quality)+1);
                result[i].m_buyPrice = 3*result[i].m_sellPrice;
                result[i].m_type = ItemType.ENCHANTMENT;
                result[i].m_maxNum = 1;
            }
            return result;
        }
    }
}