using System.Windows;
using VmrGenerator.Interfaces;

namespace VmrGenerator.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        private static readonly IMessageBoxService _instance = new MessageBoxService();
        /// <summary>
        /// Provides an instance of the MessageBoxService.
        /// </summary>
        public static IMessageBoxService Instance => _instance;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}