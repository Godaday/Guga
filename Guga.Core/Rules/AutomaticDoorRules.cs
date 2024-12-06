using Guga.Core.Devices;
using Guga.Core.Interfaces;
using Guga.Core.PlcSignals;
using Guga.Transformer;
using Guga.Transformer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class AutomaticDoorRules
    {
        public AutomaticDoorRules() { }

        /// <summary>
        /// 业务对象转信号
        /// </summary>
        /// <param name="u"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<Rule<AutomaticDoor, IEnumerable<IPlcSignal>>> GetBusinessToSignalRules(AutomaticDoor u, IEnumerable<IPlcSignal> t)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 信号转业务对象
        /// </summary>
        /// <param name="plcSignals"></param>
        /// <param name="automaticDoor"></param>
        /// <returns></returns>
        public List<Rule<IEnumerable<IPlcSignal>, AutomaticDoor>> GetSignalToBusinessRules(IEnumerable<IPlcSignal> plcSignals, AutomaticDoor automaticDoor)
        {
            List<Rule<IEnumerable<IPlcSignal>, AutomaticDoor>> rules = new List<Rule<IEnumerable<IPlcSignal>, AutomaticDoor>>();

            rules.AddRange(
                new List<Rule<IEnumerable<IPlcSignal>, AutomaticDoor>>()
                {
                    new Rule<IEnumerable<IPlcSignal>, AutomaticDoor>(plcSignals, automaticDoor)
                    {
                        Condition = (signals) => signals.Any(s => s.SignalCode=="0032"&&s.GetValue().ToString()=="1"),
                        Action = ( door) =>{
                        if(door.DoorStatus!="开") door.SetDoorStatus("开");
                        }
                    },
                    new Rule<IEnumerable<IPlcSignal>, AutomaticDoor>(plcSignals, automaticDoor)
                    {
                        Condition = (signals) => signals.Any(s => s.SignalCode=="0032"&&s.GetValue().ToString()!="1"),
                        Action = ( door) => {
                        if(door.DoorStatus!="关")door.SetDoorStatus("关");
                        }
                    },
                }
                );


            return rules;
        }
    }
}
