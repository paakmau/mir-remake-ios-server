using System.Collections.Generic;
using LitJson;
using System;
using System.IO;
namespace MirRemakeBackend.Data
{
    interface IDS_Character
    {
        /// <summary>
        /// 返回所有角色  
        /// 第一维是不同的角色  
        /// 第二维是不同的等级, 等级有序递增  
        /// </summary>
        /// <returns></returns>
        DO_Character[][] GetAllCharacter();//done
    }
    class DS_CharacterImpl : IDS_Character
    {
        private JsonData s_characterDatas;
        private DO_Character[][] res;
        public DO_Character[][] GetAllCharacter()
        {
            string jsonFile = File.ReadAllText("Data/D_Character.json");
            s_characterDatas = JsonMapper.ToObject(jsonFile);
            res = new DO_Character[4][];
            for (int i = 0; i < 4; i++)
            {
                res[i] = new DO_Character[100];
            }
            for (int occupation = 0; occupation < 4; occupation++)
            {
                for (int level = 1; level <= 100; level++)
                {
                    DO_Character character = new DO_Character();
                    int i = occupation * 100 + level - 1;
                    character.m_occupation =
                        (OccupationType)Enum.Parse(typeof(OccupationType), s_characterDatas[i]["Occupation"].ToString());
                    character.m_level = (short)level;
                    character.m_upgradeExperienceInNeed =
                        int.Parse(s_characterDatas[i]["ExperienceInNeed"].ToString());
                    character.m_mainAttributeArr = new ValueTuple<ActorUnitMainAttributeType, int>[s_characterDatas[i]["MainAttributionTable"].Count];
                    for (int durex = 0; durex < s_characterDatas[i]["MainAttributionTable"].Count; durex++)
                    {
                        character.m_mainAttributeArr[durex] = new ValueTuple<ActorUnitMainAttributeType, int>
                            ((ActorUnitMainAttributeType)Enum.Parse(typeof(ActorUnitMainAttributeType), s_characterDatas[i]["MainAttributionTable"][durex].ToString().Split(' ')[0]), int.Parse(s_characterDatas[i]["MainAttributionTable"][durex].ToString().Split(' ')[1]));
                    }
                    character.m_concreteAttributeArr = new ValueTuple<ActorUnitConcreteAttributeType, int>[s_characterDatas[i]["ConcreteAttributionTable"].Count];
                    for (int durex = 0; durex < s_characterDatas[i]["ConcreteAttributionTable"].Count; durex++)
                    {   ActorUnitConcreteAttributeType t=(ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType), s_characterDatas[i]["ConcreteAttributionTable"][durex].ToString().Split(' ')[0]);
                        if(character.m_occupation==OccupationType.ROGUE && t==ActorUnitConcreteAttributeType.ATTACK)
                            character.m_concreteAttributeArr[durex] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                                (t, 2*(int)float.Parse(s_characterDatas[i]["ConcreteAttributionTable"][durex].ToString().Split(' ')[1]));
                        else
                            character.m_concreteAttributeArr[durex] = new ValueTuple<ActorUnitConcreteAttributeType, int>
                                (t, (int)float.Parse(s_characterDatas[i]["ConcreteAttributionTable"][durex].ToString().Split(' ')[1]));
                            
                    }
                    character.m_mainAttrPointNumber = short.Parse(s_characterDatas[i]["GiftPointNumber"].ToString());
                    // Console.WriteLine(occupation+" "+level);
                    res[occupation][level-1] = character;
                }
            }

            return res;
        }

    }
}