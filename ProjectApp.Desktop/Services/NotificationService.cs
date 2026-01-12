using System.Windows;

namespace ProjectApp.Desktop.Services
{
    public interface INotificationService
    {
        void Info(string message, string title);
        void Error(string message, string title);
    }

    public class NotificationService : INotificationService
    {
        public void Info(string message, string title)
            => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public void Error(string message, string title)
            => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}