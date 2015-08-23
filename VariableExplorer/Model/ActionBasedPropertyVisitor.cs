using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    internal class ActionBasedPropertyVisitor : IPropertyVisitor
    {
        private Action<IExpandablePropertyInfo> _expandablePropertyAttended;
        private Action<IValuePropertyInfo> _valuePropertyAttended;

        public ActionBasedPropertyVisitor(Action<IExpandablePropertyInfo> expandablePropertyAttended,
            Action<IValuePropertyInfo> valuePropertyAttended)
        {
            _expandablePropertyAttended = expandablePropertyAttended;
            _valuePropertyAttended = valuePropertyAttended;
        }

        public virtual void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
        {
            _expandablePropertyAttended(expandablePropertyInfo);
        }

        public virtual void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
        {
            _valuePropertyAttended(valuePropertyInfo);
        }

        public void Dispose()
        {
            // no need to do anything
        }
    }
}
