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

namespace MirRemakeBackend {
    struct E_Effect {
        public int m_casterNetworkId;
        public short m_animId;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHp;
        public EffectDeltaHPType m_deltaHpType;
        public int m_deltaMp;
        public EffectDeltaMPType m_deltaMpType;
        public E_Status[] m_statusAttachArray;
        public int m_StatusAttachNum { get { return m_statusAttachArray.Length; } }

        public E_Effect (DO_Effect effectDo, int casterNetId) {
            m_casterNetworkId = casterNetId;
            m_animId = effectDo.m_animId;
            m_hitRate = effectDo.m_hitRate;
            m_criticalRate = effectDo.m_criticalRate;
            m_deltaHp = effectDo.m_deltaHP;
            m_deltaHpType = effectDo.m_deltaHPType;
            m_deltaMp = effectDo.m_deltaMP;
            m_deltaMpType = effectDo.m_deltaMPType;
            DO_Status[] statusDoArr = effectDo.m_statusAttachArray;
            m_statusAttachArray = new E_Status[statusDoArr.Length];
            for (int i=0; i<statusDoArr.Length; i++)
                m_statusAttachArray[i] = new E_Status (statusDoArr[i], casterNetId);
        }
    }
}