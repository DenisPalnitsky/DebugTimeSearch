using Microsoft.VisualStudio.Debugger.Interop;
using System.Collections.Generic;
using System.Linq;

namespace MyCompany.VariableExplorer.Model
{    
    class DebugProperty : MyCompany.VariableExplorer.Model.IDebugProperty
    {        
        IPropertyInfo _debugPropertyInfo;
        IConfiguration _configuration = IocContainer.Resolve<IConfiguration>();
        IEnumerable<IPropertyInfo> _children;
        IDebugProperty2 _vsDebugProperty;

        private DebugProperty() { }

        private DebugProperty (IDebugProperty2 debugProperty)
        {
            _vsDebugProperty = debugProperty;
        }

        public static IDebugProperty Create(IDebugProperty2 vsDebugProperty)
        {
            var result =  new DebugProperty(vsDebugProperty);            
            result._debugPropertyInfo = GetPropertyInfo(vsDebugProperty);
            return result;
        }

        public IEnumerable<IPropertyInfo> Children 
        { 
            get 
            {
                if(_children == null)
                    _children = GetChildren(_vsDebugProperty);
                return _children; 
            } 
        }

        public IPropertyInfo PropertyInfo
        {
            get
            {                
                return _debugPropertyInfo;
            }
        }

        private static IPropertyInfo GetPropertyInfo(IDebugProperty2 debugProperty)
        {
            IDebugReference2[] reference = new IDebugReference2[1];
            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];

            debugProperty.GetPropertyInfo(
                    enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE,
                    10,
                    IocContainer.Resolve<IConfiguration>().DefaultTimeoutForVSCalls,
                    reference,
                    0,
                    propertyInfo).ThrowOnFailure();
            return PropertyInfoFactory.Create(propertyInfo[0]);
        }

        private static List<IPropertyInfo> GetChildren(IDebugProperty2 debugProperty)
        {
            IEnumDebugPropertyInfo2 debugPropertyEnum;

            debugProperty.EnumChildren(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE,
                10,
                dbgGuids.guidFilterLocals,
                enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_ALL,
                "",
                IocContainer.Resolve<IConfiguration>().DefaultTimeoutForVSCalls,
                out debugPropertyEnum).ThrowOnFailure();

            uint count;
            debugPropertyEnum.GetCount(out count).ThrowOnFailure();
            DEBUG_PROPERTY_INFO[] debugPropInfos = new DEBUG_PROPERTY_INFO[count];
            uint fetched;
            debugPropertyEnum.Next(count, debugPropInfos, out fetched).ThrowOnFailure();


            return debugPropInfos.Select(d => PropertyInfoFactory.Create(d)).Cast<IPropertyInfo>().ToList();
        }
    }
}
