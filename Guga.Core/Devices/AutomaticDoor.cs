using Guga.Core.Enums;
using Guga.Core.Interfaces;
using Guga.Core.MediatR;
using Guga.Core.PlcSignals;
using Guga.Core.Rules;
using Guga.Transformer;
using Guga.Transformer.Interfaces;
using MediatR;

namespace Guga.Core.Devices
{
    /// <summary>
    /// 自动门
    /// </summary>
    public class AutomaticDoor : Device
    {
        /// <summary>
        ///默认构造函数
        /// </summary>
        public AutomaticDoor()
        {
            base.deviceInfo.DeviceType_ = DeviceType.AutomaticDoor;
        }

        public string DoorStatus { get; private set; } = null!; // 门的状态（开/关）

        public void SetDoorStatus(string value)
        {
            DoorStatus = value;
        }
        /// <summary>
        /// 定义信号到业务规则
        /// </summary>
        /// <returns></returns>
        public override List<IRule> GetSignalToBusinessRules()
        {
            var siganls = this.GetSubscribedSignals();
            List<IRule> rules = new List<IRule>();

            rules.AddRange(
                new List<Rule<IEnumerable<IPlcSignal>, AutomaticDoor>>()
                {
                    new Rule<IEnumerable<IPlcSignal>, AutomaticDoor>(siganls, this)
                    {
                        Condition = (signals) => signals.Any(s => s.Address=="0032"&&s.GetValue().ToString()=="1"),
                        Action = ( door) =>{
                        if(door.DoorStatus!="开") door.SetDoorStatus("开");
                        }
                    },
                    new Rule<IEnumerable<IPlcSignal>, AutomaticDoor>(siganls, this)
                    {
                        Condition = (signals) => signals.Any(s => s.Address=="0032"&&s.GetValue().ToString()!="1"),
                        Action = ( door) => {
                        if(door.DoorStatus!="关")door.SetDoorStatus("关");
                        }
                    },
                }
                );


            return rules;
        }
        /// <summary>
        /// 业务规则到信号的转换
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<IRule> GetBusinessToSignalRules()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 发布信号改变事件
        /// </summary>
        public override void SignalChangeEvent()
        {
            //if (_mediator != null)
            //{
            //    _mediator.Publish(new SignalsChangedEvent(this.GetSubscribedSignals(), this));
            //}
            //else
            //{
            //    throw new Exception("Device property _mediator is null");
            //}


        }
        public override string ToString()
        {
            return base.ToString() + " DoorStatus:" + DoorStatus;
        }
    }
}
