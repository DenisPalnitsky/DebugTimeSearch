using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCompany.VariableExplorer.Model
{
    interface IPropertyVisitor :IDisposable
    {
        void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo);
        void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo);
    }

  

   

}
