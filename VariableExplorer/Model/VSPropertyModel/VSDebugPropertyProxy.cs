﻿using Microsoft.VisualStudio.Debugger.Interop;
using SearchLocals.Model.Services;
using System.Collections.Generic;
using System.Linq;

namespace SearchLocals.Model.VSPropertyModel
{    
    class VSDebugPropertyProxy : IVSDebugPropertyProxy
    {        
        IPropertyInfo _propertyInfo;        
        IPropertyInfo[] _children;
        IDebugProperty2 _vsDebugProperty;
        ILog _logger = Logger.GetLogger();
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

        public override string ToString()
        {
            if (PropertyInfo != null)
                return $"{PropertyInfo?.ToString()};Childern count: {_children?.Length }";
            else
                return base.ToString();
        }

        private static IPropertyInfo GetPropertyInfo(IDebugProperty2 debugProperty, PropertyInfoFactory propertyInfoFactory)
        {
            IDebugReference2[] reference = new IDebugReference2[1];
            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];

            debugProperty.GetPropertyInfo(
                    enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE,
                    10,
                    Configuration.DefaultTimeoutForVSCalls,
                    reference,
                    0,
                    propertyInfo).ThrowOnFailure();
            return propertyInfoFactory.Create(propertyInfo[0], null);
        }

        private IPropertyInfo[] GetChildren(IDebugProperty2 debugProperty, IExpandablePropertyInfo parent)
        {
            var logger = Logger.GetLogger();
            logger.Info("EnumChildren");

            IEnumDebugPropertyInfo2 debugPropertyEnum;

            debugProperty.EnumChildren(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_STANDARD | enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_FULLNAME,
                10,
                dbgGuids.guidFilterLocals,
                enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_ALL,
                "",
                Configuration.DefaultTimeoutForVSCalls,
                out debugPropertyEnum).ThrowOnFailure();

            uint count;
            debugPropertyEnum.GetCount(out count).ThrowOnFailure();
            DEBUG_PROPERTY_INFO[] debugPropInfos = new DEBUG_PROPERTY_INFO[ITEMS_TO_FETCH];
            uint fetched;

            logger.Info($"Fetch children");

            List<IPropertyInfo> result = new List<IPropertyInfo>();
            do
            {
                // TODO: Performance bottlneck
                debugPropertyEnum.Next(ITEMS_TO_FETCH, debugPropInfos, out fetched).ThrowOnFailure();

                logger.Info("Received next {0} of children", ITEMS_TO_FETCH );
                foreach (var p in debugPropInfos.Take((int)fetched))
                {
                    // this properties are not evaluated
                   var child = _propertyInfoFactory.Create(p, parent);
                    logger.Info("Returning property: '{0}'", child.Name);
                    result.Add(child);
                }
            } while (fetched >= ITEMS_TO_FETCH );

            logger.Info("All children returned");
            return result.ToArray();
        }
    }
}
