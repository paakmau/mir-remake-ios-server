using LiteNetLib.Utils;

namespace MirRemake {
    interface IFSMState {
        /// <summary>
        /// 状态的类别
        /// </summary>
        FSMStateType m_Type { get; }
        E_ActorUnit m_Self { get; set; }
        /// <summary>
        /// 用于判断能否转移到next
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        bool HasActiveTransitionTo (FSMStateType next);
        /// <summary>
        /// 用于处理主动转移时能否离开
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        bool CanActiveExit (FSMStateType next);
        /// <summary>
        /// 用于处理主动转移时能否进入
        /// </summary>
        /// <param name="prev"></param>
        /// <returns></returns>
        bool CanActiveEnter (FSMStateType prev);
        void OnEnter (FSMStateType prevType);
        /// <summary>
        /// 每帧调用, 可对unit施加影响
        /// </summary>
        /// <param name="self"></param>
        /// <param name="dT"></param>
        void OnTick (float dT);
        /// <summary>
        /// 每帧调用, 判断需要转移到的下一个状态  
        /// </summary>
        /// <returns>
        /// 若需要退出, 则返回下一个状态  
        /// 否则返回null  
        /// </returns>
        IFSMState GetNextState ();
        void OnExit (FSMStateType nextType);
    }
}