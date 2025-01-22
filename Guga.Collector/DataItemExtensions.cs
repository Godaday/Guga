using S7.Net;
using S7.Net.Types;



    namespace Guga.Collector
    {
        public static class DataItemExtensions
        {
            public static string ToAddressString(this DataItem dataItem)
            {
                return dataItem.DataType switch
                {
                    DataType.DataBlock => GetDbAddress(dataItem),
                    DataType.Input => GetIoAddress(dataItem, "I"),
                    DataType.Output => GetIoAddress(dataItem, "Q"),
                    DataType.Memory => GetIoAddress(dataItem, "M"),
                    DataType.Timer => $"T{dataItem.StartByteAdr}",
                    DataType.Counter => $"C{dataItem.StartByteAdr}",
                    _ => throw new InvalidOperationException($"Unsupported DataType: {dataItem.DataType}")
                };
            }

            private static string GetDbAddress(DataItem dataItem)
            {
                return dataItem.VarType switch
                {
                    VarType.Bit => $"DB{dataItem.DB}.DBX{dataItem.StartByteAdr}.{dataItem.BitAdr}",
                    VarType.Byte => $"DB{dataItem.DB}.DBB{dataItem.StartByteAdr}",
                    VarType.Word => $"DB{dataItem.DB}.DBW{dataItem.StartByteAdr}",
                    VarType.DWord => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}",
                    VarType.Int => $"DB{dataItem.DB}.DBW{dataItem.StartByteAdr}",
                    VarType.DInt => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}",
                    VarType.Real => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}",
                    VarType.LReal => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}", // Map LReal to DBD
                    VarType.String => $"DB{dataItem.DB}.DBB{dataItem.StartByteAdr}", // String stored as bytes
                    VarType.S7String => $"DB{dataItem.DB}.DBB{dataItem.StartByteAdr}", // S7 String stored as bytes
                    VarType.S7WString => $"DB{dataItem.DB}.DBW{dataItem.StartByteAdr}", // Wide String stored as words
                    VarType.Timer => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}", // Timer stored as DWord
                    VarType.Counter => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}", // Counter stored as DWord
                    VarType.DateTime => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}", // DateTime stored as DWord
                    //VarType.Date => $"DB{dataItem.DB}.DBW{dataItem.StartByteAdr}", // Date stored as Word
                    //VarType.Time => $"DB{dataItem.DB}.DBD{dataItem.StartByteAdr}", // Time stored as DWord
                    _ => throw new InvalidOperationException($"Unsupported VarType for DataBlock: {dataItem.VarType}")
                };
            }

            private static string GetIoAddress(DataItem dataItem, string prefix)
            {
                return dataItem.VarType switch
                {
                    VarType.Bit => $"{prefix}{dataItem.StartByteAdr}.{dataItem.BitAdr}",
                    VarType.Byte => $"{prefix}B{dataItem.StartByteAdr}",
                    VarType.Word => $"{prefix}W{dataItem.StartByteAdr}",
                    VarType.DWord => $"{prefix}D{dataItem.StartByteAdr}",
                    VarType.Int => $"{prefix}W{dataItem.StartByteAdr}",
                    VarType.DInt => $"{prefix}D{dataItem.StartByteAdr}",
                    VarType.Real => $"{prefix}D{dataItem.StartByteAdr}",
                    VarType.LReal => $"{prefix}D{dataItem.StartByteAdr}", // LReal stored as DWord
                    VarType.String => $"{prefix}B{dataItem.StartByteAdr}", // String stored as bytes
                    VarType.S7String => $"{prefix}B{dataItem.StartByteAdr}", // S7 String stored as bytes
                    VarType.S7WString => $"{prefix}W{dataItem.StartByteAdr}", // Wide String stored as words
                    VarType.Timer => $"{prefix}D{dataItem.StartByteAdr}", // Timer stored as DWord
                    VarType.Counter => $"{prefix}D{dataItem.StartByteAdr}", // Counter stored as DWord
                    VarType.DateTime => $"{prefix}D{dataItem.StartByteAdr}", // DateTime stored as DWord
                    //VarType.Date => $"{prefix}W{dataItem.StartByteAdr}", // Date stored as Word
                    //VarType.Time => $"{prefix}D{dataItem.StartByteAdr}", // Time stored as DWord
                    _ => throw new InvalidOperationException($"Unsupported VarType for {prefix}: {dataItem.VarType}")
                };
            }
        }
    }





