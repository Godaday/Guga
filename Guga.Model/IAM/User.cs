using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Models.IAM
{
    [SugarTable("Users")]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string Username { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string PasswordHash { get; set; } // 存储哈希密码

        [SugarColumn(Length = 50, IsNullable = false)]
        public string Salt { get; set; } // 存储密码盐

        [SugarColumn(IsNullable = false)]
        public int Status { get; set; } // 0=禁用, 1=启用

        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
