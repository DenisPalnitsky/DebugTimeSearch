namespace SearchLocals.Model.VSPropertyModel
{
    interface IValuePropertyInfo : IPropertyInfo
    {
        bool IsEvaluated { get; }
        string Value { get; }        
    }
}
