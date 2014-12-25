using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer
{
    public class VsCommandIdentifier : IEquatable<VsCommandIdentifier>
    {
        public string GUID { get; set; }

        public int Index { get; set; }

        public VsCommandIdentifier(string guid, int index)
        {
            this.GUID = guid;
            this.Index = index;
        }

        public bool Equals(VsCommandIdentifier other)
        {
            if (string.Equals(this.GUID, other.GUID))
                return this.Index == other.Index;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;
            else
                return this.Equals((VsCommandIdentifier)obj);
        }

        public override int GetHashCode()
        {
            return (this.GUID != null ? this.GUID.GetHashCode() : 0) * 397 ^ this.Index;
        }
    }
}
