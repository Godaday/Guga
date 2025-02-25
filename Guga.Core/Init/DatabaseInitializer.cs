using Guga.Models.IAM;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Core.Init
{
    public class DatabaseInitializer
    {
        private readonly ISqlSugarClient _db;

        public DatabaseInitializer(ISqlSugarClient db)
        {
            _db = db;
        }

        public void Init()
        {
            _db.CodeFirst.SetStringDefaultLength(200)
                .InitTables(
                    typeof(User),
                    typeof(Role),
                    typeof(Resource),
                    typeof(RoleResource),
                    typeof(UserRole),
                    typeof(Organization),
                    typeof(UserOrganization)
                );
        }
    }

}
