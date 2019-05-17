/**
 *Enity，技能实体
 *创建者 fn
 *时间 2019/4/1
 *最后修改者 yuk
 *时间 2019/4/3
 */

using System;
using System.Collections.Generic;
using UnityEngine;
namespace MirRemakeBackend {
    class E_Skill {
        public short m_id;
        private string m_name;
        private string m_details;
        public int m_costHP;
        public int m_costMP;
        private short m_level;
        public short m_Level { get { return m_level; } set { this.m_level = value; } }
        private short m_maxLevel;
        public short m_MaxLevel { get { return m_maxLevel; } }
        private short m_characterLevelInNeed;
        public short m_CharacterLevelInNeed { get { return m_characterLevelInNeed; } }
        // 技能熟练度
        private int m_masterly;
        public int m_Masterly { get { return this.m_masterly; } set { this.m_masterly = value; } }
        // 升级技能所需金钱
        private long m_upgradeMoneyInNeed;
        public long m_UpgradeMoneyInNeed { get { return m_upgradeMoneyInNeed; } }
        // 咏唱时间
        public float m_singTime;
        public bool m_NeedSing { get { return m_singTime != 0.0f; } }
        // 前摇时间
        public float m_castFrontTime;
        // 后摇时间
        public float m_castBackTime;
        // 冷却时间
        public float m_coolDownTime;
        private SkillTargetChooserBase m_targetChooser;
        public E_Effect m_skillEffect;
        public SkillAimType m_AimType { get { return m_targetChooser.m_TargetAimType; } }
        public short m_FatherId {
            get { return 0; }
        }
        public List<short> m_ChildrenId {
            get { return null; }
        }
        public E_Skill (short id) {
            // TODO: 仅用于测试, 日后应当删除
            m_id = id;
            m_level = 1;
            m_costHP = 0;
            m_costMP = 30;
            m_singTime = 0.0f;
            m_castFrontTime = 0.2f;
            m_castBackTime = 0.3f;
            m_coolDownTime = 3f;
            m_targetChooser = new STC_AimCircle ();
            m_skillEffect = new E_Effect ();
        }
        public List<E_ActorUnit> GetEffectTargets (E_ActorUnit self, SkillParam parm) {
            return m_targetChooser.GetEffectTargets (self, parm);
        }
        public SkillParam CompleteSkillParam (E_ActorUnit self, E_ActorUnit aimedTarget, SkillParam parm) {
            return m_targetChooser.CompleteSkillParam (self, aimedTarget, parm);
        }
        public bool InRange (Vector2 pos, SkillParam parm) {
            return m_targetChooser.InRange (pos, parm);
        }
    }
}