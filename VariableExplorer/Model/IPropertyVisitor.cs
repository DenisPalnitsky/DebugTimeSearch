using MyCompany.VariableExplorer.Model.VSPropertyModel;
using System;

namespace MyCompany.VariableExplorer.Model
{
    interface IPropertyVisitor :IDisposable
    {
        void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo);
        void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo);
    }     

}
