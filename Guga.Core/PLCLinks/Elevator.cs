namespace Guga.Core.PLCLinks
{
    /// <summary>
    /// 电梯
    /// </summary>
    public class Elevator : PLCLink
    {
        public int CurrentFloor { get; private set; } // 当前楼层
        public bool IsMoving { get; private set; } // 电梯是否在运动
        public bool IsOpenDoor { get; private set; } //是否开门

       

        public override void SignalChangeEvent()
        {
            throw new NotImplementedException();
        }

       
    }
}
