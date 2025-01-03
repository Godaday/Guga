using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Models
{

    /// <summary>
    /// 操作结果类，用于表示读写操作的结果。
    /// </summary>
    public class OperationResult<T>: Result
    {
        /// </summary>
        public T Data { get; set; }

      
        public OperationResult()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 设置成功的结果
        /// </summary>
        /// <param name="data">返回的数据</param>
        /// <returns></returns>
        public static OperationResult<T> Success(T data)
        {
            return new OperationResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = "Operation completed successfully."
            };
        }

        /// <summary>
        /// 设置失败的结果
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="error">异常信息</param>
        /// <returns></returns>
        public static OperationResult<T> Failure(string message, Exception error = null)
        {
            return new OperationResult<T>
            {
                IsSuccess = false,
                Message = message,
                Error = error
            };
        }
    }
}
