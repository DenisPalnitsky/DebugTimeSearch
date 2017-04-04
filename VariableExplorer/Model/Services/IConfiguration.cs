using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.Model.Services
{
    interface IConfiguration
    {
        uint DefaultTimeoutForVSCalls { get; }
    }

}
