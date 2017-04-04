using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.Model.Services 
{
    class Configuration : IConfiguration
    {
        public uint DefaultTimeoutForVSCalls
        {
            get { return uint.MaxValue; }
        }
    }
}
