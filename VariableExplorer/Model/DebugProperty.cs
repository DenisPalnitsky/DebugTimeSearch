using Microsoft.VisualStudio.Debugger.Interop;
using MyCompany.VariableExplorer.Model.Services;
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
        ILogger _logger = IocContainer.Resolve<ILogger>();
        private const uint ITEMS_TO_FETCH = 10;

        private DebugProperty() { }

        private DebugProperty (IDebugProperty2 debugProperty)
        {
            _vsDebugProperty = debugProperty;
        }

        public static IDebugProperty Create(IDebugProperty2 vsDebugProperty)
        {
            var result =  new DebugProperty(vsDebugProperty);

            result._logger.Info("Start GetPropertyInfo()");
            result._debugPropertyInfo = GetPropertyInfo(vsDebugProperty);
            result._logger.Info("Finish GetPropertyInfo()");
            return result;
        }

        public IEnumerable<IPropertyInfo> Children 
        { 
            get 
            {
                _logger.Info("Start getting children for {0}", this.PropertyInfo.Name );
                if(_children == null)
                    _children = GetChildren(_vsDebugProperty);

                _logger.Info("Finish getting children for {0}", this.PropertyInfo.Name);
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

        private static IEnumerable<IPropertyInfo> GetChildren(IDebugProperty2 debugProperty)
        {
            var logger = IocContainer.Resolve<ILogger>();
            logger.Info("EnumChildren");

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
            DEBUG_PROPERTY_INFO[] debugPropInfos = new DEBUG_PROPERTY_INFO[ITEMS_TO_FETCH];
            uint fetched;

            logger.Info("Fetch children");            
            do
            {
                debugPropertyEnum.Next(ITEMS_TO_FETCH, debugPropInfos, out fetched).ThrowOnFailure();                
                foreach (var p in debugPropInfos.Select(d => PropertyInfoFactory.Create(d)).Cast<IPropertyInfo>())
                    yield return p;
            } while (fetched >= ITEMS_TO_FETCH );

            logger.Info("Return children");            
        }
    }
}
