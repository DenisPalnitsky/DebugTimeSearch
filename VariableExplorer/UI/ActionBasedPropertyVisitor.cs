using MyCompany.VariableExplorer.Model;
using MyCompany.VariableExplorer.Model.VSPropertyModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCompany.VariableExplorer.UI
{

    /// <summary>
    /// This property visitor buffers events and then releases the list of events in buffer.
    /// This is to prevent often calls to DataGrid view as it lags. 
    /// </summary>
    internal class ActionBasedPropertyVisitor : IPropertyVisitor
    {
        List<IPropertyInfo> _propertyInfos = new List<IPropertyInfo>();
        const int ITEMS_TO_RELEASE_PER_EVENT = 100;

        readonly Action<IEnumerable<IExpandablePropertyInfo>> _expandablePropertyAttended;
        readonly Action<IEnumerable<IValuePropertyInfo>> _valuePropertyAttended;

        public ActionBasedPropertyVisitor(Action<IEnumerable<IExpandablePropertyInfo>> expandablePropertyAttended,
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
