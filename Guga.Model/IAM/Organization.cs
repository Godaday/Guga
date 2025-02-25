using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Models.IAM
{
    [SugarTable("Organizations")]
    public class Organization
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Name { get; set; } // 组织名称

        [SugarColumn(IsNullable = true)]
        public int? ParentId { get; set; } // 父级组织

        [SugarColumn(IsNullable = false)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
