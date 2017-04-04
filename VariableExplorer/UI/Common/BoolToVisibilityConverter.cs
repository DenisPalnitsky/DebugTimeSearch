using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SearchLocals.UI.Common
{
    // copy-paste from http://blogs.msdn.com/b/ukadc/archive/2009/11/17/five-minute-recipe-for-a-decent-booltovisibilityconverter.aspx


    //    <StackPanel VerticalAlignment="Bottom"> 
    //    <StackPanel.Resources> 
    //        <local:BoolToVisibilityConverter FalseValue="Collapsed" x:Key="btvc" /> 
    //    </StackPanel.Resources> 
    //    <CheckBox x:Name="HideOrShowCheck">Hide or show the text...</CheckBox> 
    //    <TextBlock Text="Hello World!" Visibility="{Binding ElementName=HideOrShowCheck, Path=IsChecked,Converter={StaticResource btvc}}" /> 
    //    </StackPanel> 


    //    <StackPanel VerticalAlignment="Bottom"> 
    //    <CheckBox x:Name="HideOrShowCheck">Hide or show the text...</CheckBox> 
    //    <TextBlock Text="Hello World!" Visibility="{Binding ElementName=HideOrShowCheck, Path=IsChecked,Converter={local:BoolToVisibilityConverter FalseValue=Collapsed}}" /> 
    //    </StackPanel> 

    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = System.Convert.ToBoolean(value);
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TrueValue.Equals(value) ? true : false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
