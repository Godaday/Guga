using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Models.IAM
{
    [SugarTable("Roles")]
    public class Role
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 50, IsNullable = false)]
        public string Name { get; set; }

        [SugarColumn(Length = 50)]
        public string Description { get; set; } // 角色描述

        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }


}
