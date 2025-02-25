namespace Guga.Models.Enums
{
    public enum ModbusRegisterType
    {
        /// <summary>
        /// 线圈 可读可写，表示单个布尔值
        /// </summary>
        Coil,
        /// <summary>
        /// 离散输入Discrete Input = Input Status 只读，表示单个布尔值
        /// </summary>
        DiscreteInput,
        /// <summary>
        /// 保持寄存器 可读可写
        /// </summary>
        HoldingRegister,
        /// <summary>
        /// 输入寄存器 只读
        /// </summary>
        InputRegister
    }
}
