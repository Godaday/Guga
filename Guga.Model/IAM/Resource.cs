using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Models.IAM
{
    [SugarTable("Resources")]
    public class Resource
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Name { get; set; } // 资源名称，如 "用户管理"

        [SugarColumn(Length = 255, IsNullable = true)]
        public string Path { get; set; } // 菜单路径或 API 路径

        [SugarColumn(IsNullable = false)]
        public int Type { get; set; } // 1=菜单, 2=按钮, 3=API

        [SugarColumn(Length = 255, IsNullable = true)]
        public string Icon { get; set; } // 菜单图标

        [SugarColumn(IsNullable = true)]
        public int? ParentId { get; set; } // 父级ID（用于菜单结构）

        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
