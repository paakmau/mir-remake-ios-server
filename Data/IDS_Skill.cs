using System.Collections.Generic;
using System;
using System.IO;
using LitJson;
namespace MirRemakeBackend.Data {
    interface IDS_Skill {
        /// <summary>
        /// 获取所有技能
        /// </summary>
        /// <returns></returns>
        DO_Skill[] GetAllSkill ();
        DO_Skill[] GetSkillsByOccupation(OccupationType occupation);
    }
    class DS_SkillImpl:IDS_Skill{
        private static JsonData s_skillDatas;
        private DO_Skill[] res;
        public DO_Skill[] GetAllSkill() {
            string jsonFile = File.ReadAllText("Data/D_Skill.json");
            s_skillDatas = JsonMapper.ToObject(jsonFile);
            res = new DO_Skill[s_skillDatas.Count];
            for (int i = 0; i < s_skillDatas.Count; i++)
            {
                DO_Skill skill = new DO_Skill();
                skill.m_skillId = short.Parse(s_skillDatas[i]["SkillID"].ToString());
                skill.m_skillMaxLevel = (short)s_skillDatas[i]["ManaCost"].Count;
                skill.m_fatherIdArr = new short[s_skillDatas[i]["FatherID"].Count];
                for (int j = 0; j < s_skillDatas[i]["FatherID"].Count; j++)
                {
                    skill.m_fatherIdArr[j] = short.Parse(s_skillDatas[i]["FatherID"][j].ToString());
                }
                skill.m_childrenIdArr = new short[s_skillDatas[i]["FatherID"].Count];
                for (int j = 0; j < s_skillDatas[i]["ChildrenIDList"].Count; j++)
                {
                    skill.m_childrenIdArr[j] = short.Parse(s_skillDatas[i]["ChildrenIDList"][j].ToString());
                }
                skill.m_skillAimType = (SkillAimType)Enum.Parse(typeof(SkillAimType), s_skillDatas[i]["SkillAimType"].ToString());
                skill.m_targetCamp = (CampType)Enum.Parse(typeof(CampType), s_skillDatas[i]["CampType"].ToString());
                skill.m_skillDataAllLevel = new DO_SkillData[skill.m_skillMaxLevel];
                for (int j = 0; j < skill.m_skillMaxLevel; j++)
                {
                    DO_SkillData skillData = new DO_SkillData();
                    skillData.m_skillLevel = (short)(j + 1);
                    skillData.m_upgradeCharacterLevelInNeed = (short)(10 * j + 1);
                    if(i==40){
                        skillData.m_upgradeMasterlyInNeed=0;
                    }
                    skillData.m_upgradeMoneyInNeed = GetMoney(j);
                    skillData.m_upgradeMasterlyInNeed = 100 * (j + 1);
                    skillData.m_mpCost = int.Parse(s_skillDatas[i]["ManaCost"][j].ToString());
                    skillData.m_singTime = float.Parse(s_skillDatas[i]["SingTime"].ToString());
                    skillData.m_castFrontTime = float.Parse(s_skillDatas[i]["CastFrontTime"].ToString());
                    skillData.m_castBackTime = float.Parse(s_skillDatas[i]["CastBackTime"].ToString());
                    skillData.m_coolDownTime = float.Parse(s_skillDatas[i]["CoolDownTime"].ToString());
                    skillData.m_targetNumber = byte.Parse(s_skillDatas[i]["TargetNumber"].ToString());
                    skillData.m_castRange = float.Parse(s_skillDatas[i]["CastRange"].ToString());                        skillData.m_damageParamArr = new ValueTuple<SkillAimParamType, float>[2];
                    float parameter1 = float.Parse(s_skillDatas[i]["DamageRadian"].ToString());
                    float parameter2 = float.Parse(s_skillDatas[i]["SecondParameter"].ToString());
                    SkillAimType x = skill.m_skillAimType;
                    if (x == SkillAimType.AIM_CIRCLE || x == SkillAimType.NOT_AIM_CIRCLE || x == SkillAimType.NOT_AIM_SELF_SECTOR
                        || x == SkillAimType.AIM_SELF_SECTOR || x == SkillAimType.NOT_AIM_SELF_SECTOR)
                    {
                        skillData.m_damageParamArr[0] = new ValueTuple<SkillAimParamType, float>
                            (SkillAimParamType.RADIUS, parameter1);
                        skillData.m_damageParamArr[1] = new ValueTuple<SkillAimParamType, float>
                                (SkillAimParamType.RADIAN, ((int)parameter2 == 0) ? 360 : parameter2);
                    }
                    else
                    {
                            skillData.m_damageParamArr[0] = new ValueTuple<SkillAimParamType, float>
                                (SkillAimParamType.LENGTH, parameter1);
                            skillData.m_damageParamArr[1] = new ValueTuple<SkillAimParamType, float>
                                (SkillAimParamType.WIDTH, parameter2);
                        }
                        DO_Effect effect = new DO_Effect();
                        effect.m_type = (EffectType)Enum.Parse(typeof(EffectType), s_skillDatas[i]["Effect"]["EffectDeltaHPType"].ToString());
                        effect.m_hitRate = float.Parse(s_skillDatas[i]["Effect"]["HitRate"][j].ToString());
                        effect.m_criticalRate = float.Parse(s_skillDatas[i]["Effect"]["CriticalRate"][j].ToString());
                        effect.m_deltaMp = int.Parse(s_skillDatas[i]["Effect"]["EffectDeltaMP"][j].ToString());
                        effect.m_deltaHp = int.Parse(s_skillDatas[i]["Effect"]["EffectDeltaHP"][j].ToString());
                        if(s_skillDatas[i]["Effect"]["SelfHPStrategy"].Count!=0){
                            effect.m_attributeArr = new ValueTuple<ActorUnitConcreteAttributeType, float>[s_skillDatas[i]["Effect"]["SelfHPStrategy"][j].Count];
                            for (int k = 0; k < s_skillDatas[i]["Effect"]["SelfHPStrategy"][j].Count; k++)
                            {
                                effect.m_attributeArr[k] = new ValueTuple<ActorUnitConcreteAttributeType, float>
                                ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType), s_skillDatas[i]["Effect"]["SelfHPStrategy"][j][k].ToString().Split(' ')[0]),
                                float.Parse(s_skillDatas[i]["Effect"]["SelfHPStrategy"][j][k].ToString().Split(' ')[1]));
                            }
                        }
                        effect.m_statusIdAndValueAndTimeArr = new ValueTuple<short, float, float>[s_skillDatas[i]["Effect"]["StatusAttachArray"].Count];
                        for (int wym = 0; wym < s_skillDatas[i]["Effect"]["StatusAttachArray"].Count; wym++)
                        {
                            effect.m_statusIdAndValueAndTimeArr[wym] = new ValueTuple<short, float, float>
                                (short.Parse(s_skillDatas[i]["Effect"]["StatusAttachArray"][wym]["StatusID"].ToString()),
                                float.Parse(s_skillDatas[i]["Effect"]["StatusAttachArray"][wym]["Value"][j].ToString()),
                                float.Parse(s_skillDatas[i]["Effect"]["StatusAttachArray"][wym]["LastingTime"][j].ToString()));
                        }
                        skillData.m_skillEffect = effect;
                        skill.m_skillDataAllLevel[j] = skillData;
                    }
                    
                res[i] = skill;
            }
            return res;
        }
        public DO_Skill[] GetSkillsByOccupation(OccupationType occupation) {
            string jsonFile = File.ReadAllText("Data/D_Skill.json");
            s_skillDatas = JsonMapper.ToObject(jsonFile);
            res = new DO_Skill[(s_skillDatas.Count-1)/4];
            int faker=(s_skillDatas.Count-1)/4;
            int ssm=-1;
            switch (occupation){
                case(OccupationType.WARRIOR):
                    ssm=0;
                    break;
                case(OccupationType.ROGUE):
                    ssm=10;
                    break;
                case(OccupationType.MAGE):
                    ssm=20;
                    break;
                case(OccupationType.TAOIST):
                    ssm=30;
                    break;
            }
            for (int i = ssm; i < faker+ssm; i++)
            {
                DO_Skill skill = new DO_Skill();
                skill.m_skillId = short.Parse(s_skillDatas[i]["SkillID"].ToString());
                skill.m_skillMaxLevel = (short)s_skillDatas[i]["ManaCost"].Count;
                skill.m_fatherIdArr = new short[s_skillDatas[i]["FatherID"].Count];
                for (int j = 0; j < s_skillDatas[i]["FatherID"].Count; j++)
                {
                    skill.m_fatherIdArr[j] = short.Parse(s_skillDatas[i]["FatherID"][j].ToString());
                }
                skill.m_childrenIdArr = new short[s_skillDatas[i]["FatherID"].Count];
                for (int j = 0; j < s_skillDatas[i]["ChildrenIDList"].Count; j++)
                {
                    skill.m_childrenIdArr[j] = short.Parse(s_skillDatas[i]["ChildrenIDList"][j].ToString());
                }
                skill.m_skillAimType = (SkillAimType)Enum.Parse(typeof(SkillAimType), s_skillDatas[i]["SkillAimType"].ToString());
                skill.m_targetCamp = (CampType)Enum.Parse(typeof(CampType), s_skillDatas[i]["CampType"].ToString());
                skill.m_skillDataAllLevel = new DO_SkillData[skill.m_skillMaxLevel];
                for (int j = 0; j < skill.m_skillMaxLevel; j++)
                {
                    DO_SkillData skillData = new DO_SkillData();
                    skillData.m_skillLevel = (short)(j + 1);
                    skillData.m_upgradeCharacterLevelInNeed = (short)(10 * j + 1);
                    if(i==40){
                        skillData.m_upgradeMasterlyInNeed=0;
                    }
                    skillData.m_upgradeMoneyInNeed = GetMoney(j);
                    skillData.m_upgradeMasterlyInNeed = 100 * (j + 1);
                    skillData.m_mpCost = int.Parse(s_skillDatas[i]["ManaCost"][j].ToString());
                    skillData.m_singTime = float.Parse(s_skillDatas[i]["SingTime"].ToString());
                    skillData.m_castFrontTime = float.Parse(s_skillDatas[i]["CastFrontTime"].ToString());
                    skillData.m_castBackTime = float.Parse(s_skillDatas[i]["CastBackTime"].ToString());
                    skillData.m_coolDownTime = float.Parse(s_skillDatas[i]["CoolDownTime"].ToString());
                    skillData.m_targetNumber = byte.Parse(s_skillDatas[i]["TargetNumber"].ToString());
                    skillData.m_castRange = float.Parse(s_skillDatas[i]["CastRange"].ToString());                        skillData.m_damageParamArr = new ValueTuple<SkillAimParamType, float>[2];
                    float parameter1 = float.Parse(s_skillDatas[i]["DamageRadian"].ToString());
                    float parameter2 = float.Parse(s_skillDatas[i]["SecondParameter"].ToString());
                    SkillAimType x = skill.m_skillAimType;
                    if (x == SkillAimType.AIM_CIRCLE || x == SkillAimType.NOT_AIM_CIRCLE || x == SkillAimType.NOT_AIM_SELF_SECTOR
                        || x == SkillAimType.AIM_SELF_SECTOR || x == SkillAimType.NOT_AIM_SELF_SECTOR)
                    {
                        skillData.m_damageParamArr[0] = new ValueTuple<SkillAimParamType, float>
                            (SkillAimParamType.RADIUS, parameter1);
                        skillData.m_damageParamArr[1] = new ValueTuple<SkillAimParamType, float>
                                (SkillAimParamType.RADIAN, ((int)parameter2 == 0) ? 360 : parameter2);
                    }
                    else
                    {
                            skillData.m_damageParamArr[0] = new ValueTuple<SkillAimParamType, float>
                                (SkillAimParamType.LENGTH, parameter1);
                            skillData.m_damageParamArr[1] = new ValueTuple<SkillAimParamType, float>
                                (SkillAimParamType.WIDTH, parameter2);
                        }
                        DO_Effect effect = new DO_Effect();
                        effect.m_type = (EffectType)Enum.Parse(typeof(EffectType), s_skillDatas[i]["Effect"]["EffectDeltaHPType"].ToString());
                        effect.m_hitRate = float.Parse(s_skillDatas[i]["Effect"]["HitRate"][j].ToString());
                        effect.m_criticalRate = float.Parse(s_skillDatas[i]["Effect"]["CriticalRate"][j].ToString());
                        effect.m_deltaMp = int.Parse(s_skillDatas[i]["Effect"]["EffectDeltaMP"][j].ToString());
                        effect.m_deltaHp = int.Parse(s_skillDatas[i]["Effect"]["EffectDeltaHP"][j].ToString());
                        if(s_skillDatas[i]["Effect"]["SelfHPStrategy"].Count!=0){
                            effect.m_attributeArr = new ValueTuple<ActorUnitConcreteAttributeType, float>[s_skillDatas[i]["Effect"]["SelfHPStrategy"][j].Count];
                            for (int k = 0; k < s_skillDatas[i]["Effect"]["SelfHPStrategy"][j].Count; k++)
                            {
                                effect.m_attributeArr[k] = new ValueTuple<ActorUnitConcreteAttributeType, float>
                                ((ActorUnitConcreteAttributeType)Enum.Parse(typeof(ActorUnitConcreteAttributeType), s_skillDatas[i]["Effect"]["SelfHPStrategy"][j][k].ToString().Split(' ')[0]),
                                float.Parse(s_skillDatas[i]["Effect"]["SelfHPStrategy"][j][k].ToString().Split(' ')[1]));
                            }
                        }
                        effect.m_statusIdAndValueAndTimeArr = new ValueTuple<short, float, float>[s_skillDatas[i]["Effect"]["StatusAttachArray"].Count];
                        for (int wym = 0; wym < s_skillDatas[i]["Effect"]["StatusAttachArray"].Count; wym++)
                        {
                            effect.m_statusIdAndValueAndTimeArr[wym] = new ValueTuple<short, float, float>
                                (short.Parse(s_skillDatas[i]["Effect"]["StatusAttachArray"][wym]["StatusID"].ToString()),
                                float.Parse(s_skillDatas[i]["Effect"]["StatusAttachArray"][wym]["Value"][j].ToString()),
                                float.Parse(s_skillDatas[i]["Effect"]["StatusAttachArray"][wym]["LastingTime"][j].ToString()));
                        }
                        skillData.m_skillEffect = effect;
                        skill.m_skillDataAllLevel[j] = skillData;
                    }
                    
                res[i - ssm] = skill;
            }
            return res;
        }
        
        private long GetMoney(int x) {
            if (x == 0) {
                return 125;
            }
            return GetMoney(x - 1) * 2;
        }
    }
}