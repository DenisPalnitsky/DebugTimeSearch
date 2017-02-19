using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariableExplorer_UnitTests
{

    public class Program
    {
        public static void Main()
        {
            var t = new PropertyInfoVisitorTest();
            t.SetupContainer();
            t.Enumerate_when_called_do_not_evaluates_properties_in_square_brackets();
            t.Enumerate_when_called_do_not_evaluates_properties_containing_round_brackets();
        }    
    }
}
