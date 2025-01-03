using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Models
{
    public class Result
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 操作结果的消息，若失败则为错误信息
        /// </summary>
        public string Message { get; set; }

  
        /// <summary>
        /// 错误的详细信息，如果有的话
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// 操作的时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        public Result()
        {
            Timestamp = DateTime.Now;
        }
        /// <summary>
        /// 设置成功的结果
        /// </summary>
        /// <param name="message">成功时的消息</param>
        /// <returns></returns>
        public static Result Success(string message = "Operation completed successfully.")
        {
            return new Result
            {
                IsSuccess = true,
                Message = message
            };
        }

        /// <summary>
        /// 设置失败的结果
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="exception">异常信息</param>
        /// <returns></returns>
        public static Result Failure(string message, Exception exception = null)
        {
            return new Result
            {
                IsSuccess = false,
                Message = message ?? "An error occurred during the operation.",
                Error = exception
            };
        }

    }
}
