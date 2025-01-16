using Guga.Collector.Models;
using Guga.Core.Interfaces;

namespace Guga.Collector.Interfaces
{
    /// <summary>
    /// 链路客户端接口
    /// </summary>
    public interface IPLCLinkClient
    {

      
        /// <summary>
        /// 连接链路
        /// </summary>
        /// <returns></returns>
        Task<Result> ConnectAsync(int retryCount = 3, int delayMilliseconds = 1000);
        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        Task<Result> DisconnectAsync();
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signal"></param>
        /// <returns></returns>
        Task<OperationResult<object>> ReadDataAsync(IPlcSignal plcSignal);

        /// <summary>
        /// 读取多个信号的数据
        /// </summary>
        /// <param name="signals">信号标识集合</param>
        /// <returns>信号和值的字典</returns>
        Task<OperationResult<IEnumerable<IPlcSignal>>> ReadDataAsync(IEnumerable<IPlcSignal> plcSignals);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="signal"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<Result> WriteDataAsync(IPlcSignal  plcSignal, object data);

        /// <summary>
        /// 写入多个信号的数据
        /// </summary>
        /// <param name="data">信号和值的字典</param>
        /// <returns></returns>
        Task <Result> WriteDataAsync(Dictionary<IPlcSignal, object> data);



       
    }
}
