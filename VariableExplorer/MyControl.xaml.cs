using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyCompany.VariableExplorer
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        
        public MyControl()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (VariableExplorerPackage.ExpressionEvaluator != null)
            {                
                var propertyInfo = VariableExplorerPackage.ExpressionEvaluator.EvaluateExpression(this.ExpressionTextBox.Text);
                this.TextBlock.Text = Newtonsoft.Json.JsonConvert.SerializeObject(propertyInfo);
            }
            else
                this.TextBlock.Text = "ExpressionEvaluator is not initialized";
            
        }

        private void ExpressionTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExpressionTextBox.Text = ObjectDump.GetCurrentText();
        }
    }
}