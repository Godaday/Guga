﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guga.Collector.Interfaces
{
    public interface IMasterElectionService
    {
        void StartMasterElection(CancellationToken cancellationToken);
    }
}
