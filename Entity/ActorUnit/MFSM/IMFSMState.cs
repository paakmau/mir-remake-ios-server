using LiteNetLib.Utils;

namespace MirRemakeBackend {
    interface IMFSMState {
        /// <summary>
        /// 状态的类别
        /// </summary>
        MFSMStateType m_Type { get; }
        E_Monster m_Self { get; set; }
        void OnEnter (MFSMStateType prevType);
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
        IMFSMState GetNextState ();
        void OnExit (MFSMStateType nextType);
    }
}