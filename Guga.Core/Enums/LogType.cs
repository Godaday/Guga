using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LogMetadataAttribute : Attribute
    {
        public string Description { get; }
        public string Color { get; }
        public string Icon { get; }

        public LogMetadataAttribute(string description, string color, string icon)
        {
            Description = description;
            Color = color;
            Icon = icon;
        }
    }
    public enum LogCategory
    {
        [Description("服务竞选")]
        ServiceElection,

        [Description("采集器")]
        Collector,

        [Description("设备连接")]
        DeviceConnection,

        [Description("写入器")]
        Writer,

        [Description("写入模拟器")]
        WriteSimulator
    }
    public enum LogLevel
    {
      
        [LogMetadata("信息", "#ecf0f1", "ℹ️")]
        Info,

        [LogMetadata("警告", "#f7b731", "⚠️")]
        Warning,

        [LogMetadata("错误", "#fc5c65", "❌")]
        Error
    }
    public static class EnumExtensions
    {
        // 获取枚举的 Description 特性值
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        // 获取 LogMetadata 特性值
        public static (string Description, string Color, string Icon) GetLogMetadata(this LogLevel level)
        {
            var field = level.GetType().GetField(level.ToString());
            var attribute = field?.GetCustomAttribute<LogMetadataAttribute>();
            return attribute != null
                ? (attribute.Description, attribute.Color, attribute.Icon)
                : (level.ToString(), "gray", "❓");
        }
    }
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public LogCategory Category { get; set; }
        public LogLevel Level { get; set; }

        // 格式化输出
        public override string ToString()
        {
            var categoryDesc = Category.GetDescription();
            var (levelDesc, color, icon) = Level.GetLogMetadata();

            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] " +
                   $"[{categoryDesc}] " +
                   $"{icon} {levelDesc} - {Message}";
        }
    }





}
