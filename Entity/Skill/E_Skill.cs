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
namespace MirRemake {
    class E_Skill {
        public short m_id;
        private byte m_type; //
        private string m_name;
        private string m_details;
        public int m_costHP;
        public int m_costMP;
        private short m_level;
        public short m_Level { get { return m_level; } set { this.m_level = value; } }
        // 技能熟练度
        private int m_masterly; //
        public int m_Masterly { get { return this.m_masterly; } set { this.m_masterly = value; } }
        // 升级技能所需金钱
        public long m_upgradeMoneyInNeed;
        // 咏唱时间
        public float m_singTime;
        // 是否需要咏唱
        public bool m_NeedSing { get { return m_singTime != 0.0f; } }
        // 前摇时间
        public float m_castFrontTime;
        // 后摇时间
        public float m_castBackTime;
        // 冷却时间
        public float m_coolDownTime;
        public E_Effect m_skillEffect;
        // TODO:是否能升级，（等级与熟练度是否都足够）
        public bool m_IsUpgradable {
            get { return false; }
        }
        public short m_FatherId {
            get { return 0; }
        }
        public List<short> m_ChildrenId {
            get { return null; }
        }
        public E_Skill (short id) {
            // TODO: 仅用于测试, 日后应当删除
            m_id = id;
            m_costHP = 0;
            m_costMP = 30;
            m_singTime = 0.0f;
            m_castFrontTime = 0.2f;
            m_castBackTime = 0.3f;
            m_skillEffect = new E_Effect ();
        }
        // TODO:技能升级
        public bool Upgrade () {
            if (this.m_IsUpgradable) {
                return true;
            }
            return false;
        }
        // TODO:遗忘技能
        public bool Forget () {
            return true;
        }
    }
}