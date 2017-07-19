using Microsoft.VisualStudio.Debugger.Interop;
using SearchLocals.Model.Services;
using System.Collections.Generic;
using System.Linq;

namespace SearchLocals.Model.VSPropertyModel
{    
    class VSDebugPropertyProxy : IVSDebugPropertyProxy
    {        
        IPropertyInfo _propertyInfo;
        IConfiguration _configuration = ServiceLocator.Resolve<IConfiguration>();
        IPropertyInfo[] _children;
        IDebugProperty2 _vsDebugProperty;
        ILog _logger = ServiceLocator.Resolve<ILog>();
        private const uint ITEMS_TO_FETCH = 1000;
        PropertyInfoFactory _propertyInfoFactory = new PropertyInfoFactory();

        private VSDebugPropertyProxy() { }

        private VSDebugPropertyProxy (IDebugProperty2 debugProperty)
        {
            _vsDebugProperty = debugProperty;
        }

        public static IVSDebugPropertyProxy Create(IDebugProperty2 vsDebugProperty)
        {
            var result =  new VSDebugPropertyProxy(vsDebugProperty);

            result._logger.Info("Start GetPropertyInfo()");
            result._propertyInfo = GetPropertyInfo(vsDebugProperty, result._propertyInfoFactory);
            result._logger.Info("Finish GetPropertyInfo()");
            return result;
        }

        public IEnumerable<IPropertyInfo> Children 
        { 
            get 
            {
                _logger.Info("Start getting children for {0}", this.PropertyInfo.FullName);
                if(_children == null)
                    _children = GetChildren(_vsDebugProperty, _propertyInfo as IExpandablePropertyInfo);

                _logger.Info("Finish getting children for {0}", this.PropertyInfo.FullName);
                return _children; 
            } 
        }

        public IPropertyInfo PropertyInfo
        {
            get
            {                
                return _propertyInfo;
            }
        }

        private static IPropertyInfo GetPropertyInfo(IDebugProperty2 debugProperty, PropertyInfoFactory propertyInfoFactory)
        {
            IDebugReference2[] reference = new IDebugReference2[1];
            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];

            debugProperty.GetPropertyInfo(
                    enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE,
                    10,
                    ServiceLocator.Resolve<IConfiguration>().DefaultTimeoutForVSCalls,
                    reference,
                    0,
                    propertyInfo).ThrowOnFailure();
            return propertyInfoFactory.Create(propertyInfo[0], null);
        }

        private IPropertyInfo[] GetChildren(IDebugProperty2 debugProperty, IExpandablePropertyInfo parent)
        {
            var logger = ServiceLocator.Resolve<ILog>();
            logger.Info("EnumChildren");

            IEnumDebugPropertyInfo2 debugPropertyEnum;

            debugProperty.EnumChildren(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE,
                10,
                dbgGuids.guidFilterLocals,
                enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ACCESS_ALL,
                "",
                ServiceLocator.Resolve<IConfiguration>().DefaultTimeoutForVSCalls,
                out debugPropertyEnum).ThrowOnFailure();

            uint count;
            debugPropertyEnum.GetCount(out count).ThrowOnFailure();
            DEBUG_PROPERTY_INFO[] debugPropInfos = new DEBUG_PROPERTY_INFO[ITEMS_TO_FETCH];
            uint fetched;

            logger.Info("Fetch children");

            List<IPropertyInfo> result = new List<IPropertyInfo>();
            do
            {
                // TODO: Performance bottlneck
                debugPropertyEnum.Next(ITEMS_TO_FETCH, debugPropInfos, out fetched).ThrowOnFailure();

                logger.Info("Received next {0} of children", ITEMS_TO_FETCH );
                foreach (var p in debugPropInfos.Take((int)fetched).Select(d => _propertyInfoFactory.Create(d, parent)).Cast<IPropertyInfo>())
                {
                    logger.Info("Returning property: '{0}'", p.Name);
                    result.Add(p);
                }
            } while (fetched >= ITEMS_TO_FETCH );

            logger.Info("All children returned");
            return result.ToArray();
        }
    }
}
