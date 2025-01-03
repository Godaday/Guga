using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guga.Collector.Interfaces;
using Guga.Collector.Models;
using Guga.Core.Interfaces;
using S7.Net;

namespace Guga.Collector
{

    public class S7Client : IDeviceClient
    {
        private Plc _plc;


        public S7Client(string ipAddress, CpuType cpuType, short rack, short slot)
        {
            _plc = new Plc(cpuType, ipAddress, rack, slot);

        }

        public async Task<Result> ConnectAsync()
        {
            if (IsConnected())
            {
                return Result.Failure("设备已经连接。");
            }

            try
            {
                await Task.Run(() => _plc.Open());

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"连接设备失败: {ex.Message}");
            }
        }

        public bool IsConnected()
        {
            return _plc != null && _plc.IsConnected;
        }

        public async Task<Result> DisconnectAsync()
        {
            if (!IsConnected())
            {
                return Result.Failure("设备未连接。");
            }

            try
            {
                await Task.Run(() => _plc.Close());

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"断开连接失败: {ex.Message}");
            }
        }

        public async Task<OperationResult<object>> ReadDataAsync(IPlcSignal plcSignal)
        {
            if (!IsConnected())
            {
                return OperationResult<object>.Failure("设备未连接，无法读取数据。");
            }

            try
            {
                var result = await Task.Run(() => _plc.Read(plcSignal.SignalCode));
                plcSignal.SetValue(result);
                return OperationResult<object>.Success(plcSignal);
            }
            catch (Exception ex)
            {
                return OperationResult<object>.Failure($"读取信号 '{plcSignal.SignalName}' 数据失败: {ex.Message}");
            }
        }

        public async Task<OperationResult<IEnumerable<IPlcSignal>>> ReadDataAsync(IEnumerable<IPlcSignal> signals)
        {
            if (!IsConnected())
            {
                return OperationResult<IEnumerable<IPlcSignal>>.Failure("设备未连接，无法读取数据。");
            }

            try
            {
                var result = new List<IPlcSignal>();
                foreach (var signal in signals)
                {
                    var value = await Task.Run(() => _plc.Read(signal.SignalCode));
                    signal.SetValue(value);
                }
                return OperationResult<IEnumerable<IPlcSignal>>.Success(signals);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<IPlcSignal>>.Failure($"读取数据失败: {ex.Message}");
            }
        }

        public async Task<Result> WriteDataAsync(IPlcSignal signal, object value)
        {
            if (!IsConnected())
            {
                return Result.Failure("设备未连接，无法写入数据。");
            }

            try
            {
                await Task.Run(() => _plc.Write(signal.SignalCode, value));
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"写入信号 '{signal}' 数据失败: {ex.Message}");
            }
        }

        public async Task<Result> WriteDataAsync(Dictionary<IPlcSignal, object> data)
        {
            if (!IsConnected())
            {
                return Result.Failure("设备未连接，无法写入数据。");
            }

            try
            {
                foreach (var kvp in data)
                {
                    await Task.Run(() => _plc.Write(kvp.Key.SignalCode, kvp.Value));
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"写入数据失败: {ex.Message}");
            }
        }

     
    }
}
    
