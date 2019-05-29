using System.Collections.Generic;
using System.IO;
using LitJson;
using System;
namespace MirRemakeBackend.Data {
    interface IDS_Monster {
        DO_Monster[] GetAllMonster ();//done
    }
    class IDS_MonsterImpl : IDS_Monster {
        private static JsonData s_monsterDatas;
        private static DO_Monster[] res;
        public DO_Monster[] GetAllMonster() {
            string jsonFile = File.ReadAllText("Data/D_Monster.json");
            s_monsterDatas = JsonMapper.ToObject(jsonFile);
            res = new DO_Monster[s_monsterDatas.Count];
            for (int durex = 0; durex < s_monsterDatas.Count; durex++)
            {
                DO_Monster monster = new DO_Monster();
                monster.m_monsterId = short.Parse(s_monsterDatas[durex]["MonsterID"].ToString());
                monster.m_level = short.Parse(s_monsterDatas[durex]["Level"].ToString());
                monster.m_dropItemIdArr = new short[s_monsterDatas[durex]["DropList"].Count];
                for (int i = 0; i < s_monsterDatas[durex]["DropList"].Count; i++)
                {
                    monster.m_dropItemIdArr[i] = short.Parse(s_monsterDatas[durex]["DropList"][i].ToString());
                }
                monster.m_skillIdAndLevelArr = new ValueTuple<short, short>[s_monsterDatas[durex]["SkillList"].Count];
                for (int i = 0; i < monster.m_skillIdAndLevelArr.Length; i++)
                {
                    monster.m_skillIdAndLevelArr[i] = new ValueTuple<short, short>
                        (short.Parse(s_monsterDatas[durex]["SkillList"].ToString().Split(' ')[0]), short.Parse(s_monsterDatas[durex]["SkillList"].ToString().Split(' ')[1]));
                }
                monster.m_attrArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_monsterDatas[durex]["ConcreteAttributionTable"].Count];
                for (int x = 0; x < s_monsterDatas[durex]["ConcreteAttributionTable"].Count; x++)
                {
                    monster.m_attrArr[x] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                        ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType), s_monsterDatas[durex]["ConcreteAttributionTable"][x].ToString().Split(' ')[0]), int.Parse(s_characterDatas[durex]["ConcreteAttributionTable"][x].ToString().Split(' ')[1]));
                }
                res[durex] = monster;
            }
            return res;
        }
    }
}