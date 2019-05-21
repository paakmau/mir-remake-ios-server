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
        // TODO: 有缘改成命令模式
        public int m_casterNetworkId;
        public EffectType m_type;
        public float m_hitRate;
        public float m_criticalRate;
        public int m_deltaHp;
        public int m_deltaMp;
        public KeyValuePair<short, KeyValuePair<float, float>>[] m_statusIdAndValueAndTimeArray;
        public int m_StatusAttachNum { get { return m_statusIdAndValueAndTimeArray.Length; } }

        public E_Effect (DE_Effect effectDe, int casterNetId) {
            m_casterNetworkId = casterNetId;
            m_type = effectDe.m_type;
            m_hitRate = effectDe.m_hitRate;
            m_criticalRate = effectDe.m_criticalRate;
            m_deltaHp = effectDe.m_deltaHP;
            m_deltaMp = effectDe.m_deltaMP;
            List<int> res = new List<int> ();
            m_statusIdAndValueAndTimeArray = new KeyValuePair<short, KeyValuePair<float, float>>[effectDe.m_statusIdAndValueAndTimeList.Count];
            for (int i=0; i<effectDe.m_statusIdAndValueAndTimeList.Count; i++)
                m_statusIdAndValueAndTimeArray[i] = effectDe.m_statusIdAndValueAndTimeList[i];
        }
    }
}