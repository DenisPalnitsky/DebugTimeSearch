using SearchLocals.Model.VSPropertyModel;
using System;

namespace SearchLocals.Model
{
    interface IPropertyVisitor :IDisposable
    {
        void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo);
        void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo);
    }     

}
