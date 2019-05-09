/**
 * Enity，使用物品或技能等对角色属性造成变化的实体
 * 创建者 fn
 * 时间 2019/4/1
 * 最后修改者 yuk
 * 时间 2019/4/3
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class E_Effect {
        public short m_animId;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHP;
        public EffectDeltaHPType m_deltaHPType;
        public int m_deltaMP;
        public EffectDeltaMPType m_deltaMPType;
        public E_Status[] m_statusAttachArray;
        public int m_StatusAttachNum { get { return m_statusAttachArray == null? 0 : m_statusAttachArray.Length; } }
        public HPStrategy m_selfHPStrategy;
        public HPStrategy m_targetHPStrategy;

        public E_Effect () {
            // TODO: 仅用于测试, 日后应当删除
            m_hitRate = 10000.0f;
            m_criticalRate = 0.0f;
            m_deltaHP = -500;
            m_deltaHPType = EffectDeltaHPType.MAGIC;
            m_deltaMP = 0;
            m_deltaMPType = EffectDeltaMPType.MAGIC;
            m_statusAttachArray = new E_Status[1] { new E_Status(1, 10, 2f) };
            m_selfHPStrategy = null;
            m_targetHPStrategy = null;
        }
        public E_Effect GetClone () {
            E_Effect res = new E_Effect ();
            res.m_hitRate = m_hitRate;
            res.m_deltaHP = m_deltaHP;
            res.m_deltaHPType = m_deltaHPType;
            res.m_deltaMP = m_deltaMP;
            res.m_deltaMPType = m_deltaMPType;
            if (m_statusAttachArray != null) {
                res.m_statusAttachArray = new E_Status[m_statusAttachArray.Length];
                Array.Copy (m_statusAttachArray, res.m_statusAttachArray, m_statusAttachArray.Length);
            } else
                res.m_statusAttachArray = null;
            res.m_selfHPStrategy = m_selfHPStrategy;
            res.m_targetHPStrategy = m_targetHPStrategy;
            return res;
        }
    }
}