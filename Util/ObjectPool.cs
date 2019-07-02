using System;
using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.Util {
    interface ObjectPool {
        Object GetInstanceObj ();
        void RecycleInstance (Object obj);
    }
    /// <summary>
    /// 内存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ObjectPool<T> : ObjectPool where T : class, new () {
        private Queue<T> m_pool = new Queue<T> ();
        private int m_Size;
        public ObjectPool (int Size) {
            for (int i = 0; i < Size; i++)
                m_pool.Enqueue (new T ());
            m_Size = Size;
        }
        public Object GetInstanceObj () {
            return GetInstance ();
        }
        public T GetInstance () {
            T res;
            if (m_pool.TryDequeue (out res))
                return res;
            return new T ();
        }
        public void RecycleInstance (Object obj) {
            if (m_pool.Count < m_Size)
                m_pool.Enqueue ((T) obj);
        }
        public void RecycleInstance (T obj) {
            if (m_pool.Count < m_Size)
                m_pool.Enqueue (obj);
        }
    }
}