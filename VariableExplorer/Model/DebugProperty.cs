using Microsoft.VisualStudio.Debugger.Interop;
using System.Collections.Generic;
using System.Linq;

namespace MyCompany.VariableExplorer.Model
{    
    class DebugProperty : MyCompany.VariableExplorer.Model.IDebugProperty
    {        
        IDebugPropertyInfo _debugPropertyInfo;
        IConfiguration _configuration = IocContainer.Resolve<IConfiguration>();
        IEnumerable<IDebugPropertyInfo> _children;
        
        private DebugProperty (IDebugProperty2 debugProperty)
        {            
        }

        public static IDebugProperty Create(IDebugProperty2 debugProperty)
        {
            var result =  new DebugProperty(debugProperty);
            result._children = GetChildren(debugProperty);
            result._debugPropertyInfo = GetPropertyInfo(debugProperty);
            return result;
        }

        public IEnumerable<IDebugPropertyInfo> Children 
        { 
            get 
            {
                return _children; 
            } 
        }

        public IDebugPropertyInfo PropertyInfo
        {
            get
            {                
                return _debugPropertyInfo;
            }
        }

        private static IDebugPropertyInfo GetPropertyInfo(IDebugProperty2 debugProperty)
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
            return new DebugPropertyInfo(propertyInfo[0], true);
        }

        private static List<IDebugPropertyInfo> GetChildren(IDebugProperty2 debugProperty)
        {
            IEnumDebugPropertyInfo2 debugPropertyEnum;

            debugProperty.EnumChildren(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL,
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
            
            return  debugPropInfos.Select(d => new DebugPropertyInfo(d, false)).Cast<IDebugPropertyInfo>().ToList();            
        }
    }
}
