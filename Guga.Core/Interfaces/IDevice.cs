﻿using Guga.Core.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Interfaces
{
    /// <summary>
    /// 设备接口
    /// </summary>
   public interface IDevice
    {

        /// <summary>
        /// 设备ID
        /// </summary>
        string DeviceId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        string DeviceName { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        string DeviceCode { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        DeviceType DeviceType_ { get;  }

        /// <summary>
        /// 设备型号 如西门子 S7-300，不需要的可为空
        /// </summary>
        S7CPUType? S7CPUType_ { get; }
        /// <summary>
        /// 通信协议
        /// </summary>
        ProtocolType ProtocolType_ { get;  }

        /// <summary>
        /// 读取周期，以毫秒为单位，例如：1000ms
        /// </summary>
        int ReadCycle { get;  } 





        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip { get; set; } 

        /// <summary>
        /// 端口
        /// </summary>
        public int? Port { get; set; }



        /// <summary>
        /// 订阅多个信号
        /// </summary>
        /// <param name="signals"></param>
        public  void SubscribeToSignals(IEnumerable<IPlcSignal> signals);

        /// <summary>
        /// 取消订阅多个信号
        /// </summary>
        /// <param name="signals"></param>
        void UnsubscribeFromSignals(IEnumerable<IPlcSignal> signals);

        /// <summary>
        /// 更新多个信号
        /// </summary>
        /// <param name="updatedSignals"></param>
        void UpdateSignals(IEnumerable<IPlcSignal> updatedSignals);

        /// <summary>
        /// 获取当前设备订阅的所有信号
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlcSignal> GetSubscribedSignals();



    }
}
