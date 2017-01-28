using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    internal class ThreadSafeActionBasedPropertyVisitor : IPropertyVisitor
    {
        List<IPropertyInfo> _propertyInfos = new List<IPropertyInfo>();
        const int ITEMS_TO_RELEASE_PER_EVENT = 100;

        readonly Action<IEnumerable<IExpandablePropertyInfo>> _expandablePropertyAttended;
        readonly Action<IEnumerable<IValuePropertyInfo>> _valuePropertyAttended;

        public ThreadSafeActionBasedPropertyVisitor(Action<IEnumerable<IExpandablePropertyInfo>> expandablePropertyAttended,
            Action<IEnumerable<IValuePropertyInfo>> valuePropertyAttended)
        {
            _expandablePropertyAttended = expandablePropertyAttended;
            _valuePropertyAttended = valuePropertyAttended;
        }

        public void ParentPropertyAttended(IExpandablePropertyInfo expandablePropertyInfo)
        {
            CheckAndReleasePropertiesInfoList();

            _propertyInfos.Add(expandablePropertyInfo);
        }

        public void ValuePropertyAttended(IValuePropertyInfo valuePropertyInfo)
        {
            CheckAndReleasePropertiesInfoList();
            _propertyInfos.Add(valuePropertyInfo);
        }

        public void Dispose()
        {
            if (_propertyInfos.Count > 0)
                ReleaseEventList();
        }

        private void CheckAndReleasePropertiesInfoList()
        {
            if (_propertyInfos.Count == ITEMS_TO_RELEASE_PER_EVENT)
                ReleaseEventList();
        }

        private void ReleaseEventList()
        {
            _valuePropertyAttended(_propertyInfos.OfType<IValuePropertyInfo>());
            _expandablePropertyAttended(_propertyInfos.OfType<IExpandablePropertyInfo>());
            _propertyInfos = new List<IPropertyInfo>();
        }
    }
}
