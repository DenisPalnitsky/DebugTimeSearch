﻿using System;

namespace MyCompany.VariableExplorer.Model.VSPropertyModel
{
    interface IPropertyInfo
    {
        string FullName { get; }
        string Name { get; }
        string ValueType { get; }
    }
}