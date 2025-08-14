using System.Windows;

namespace SalesAnalysis.Classes
{
    /// <summary>
    /// Общие методы 
    /// </summary>
    public abstract class GeneralMethods
    {
        /// <summary>
        /// Отображение уведомления в MessageBox
        /// </summary>
        /// <param name="text"></param>
        public static void ShowNotificationMessageBox(string text)
        {
            MessageBox.Show(text, 
                            "Внимание", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Information);
        }
    }
}
