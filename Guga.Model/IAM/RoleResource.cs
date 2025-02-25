using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Models.IAM
{
    [SugarTable("RoleResources")]
    public class RoleResource
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false)]
        public int RoleId { get; set; } // 角色ID

        [SugarColumn(IsNullable = false)]
        public int ResourceId { get; set; } // 资源ID
    }

}
