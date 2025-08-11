using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SalesAnalysis.Classes
{
    public abstract class GeneralMethods
    {
        public static void ShowNotificationMessageBox(string text)
        {
            MessageBox.Show(text, 
                            "Внимание", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Information);
        }
    }
}
