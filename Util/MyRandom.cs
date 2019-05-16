using System;

namespace MirRemakeBackend {
    static class MyRandom {
        private static Random s_randomObj = new Random (DateTime.Now.Millisecond);
        /// <summary>
        /// 获取[left, right)中的一个整数
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int NextInt (int left, int right) {
            int num = s_randomObj.Next();
            return left + num % (right-left);
        }
        /// <summary>
        /// 获取[left, right)中的一个浮点数
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static float NextFloat (float left, float right) {
            int num = s_randomObj.Next();
            return left + num % 1000 / 1000f * (right - left);
        }
    }
}