namespace VmrGenerator.Services
{
    using System.Windows;
    using VmrGenerator.Interfaces;

    /// <summary>
    /// Provides a Win32 MessageBox via the IMessageBoxService interface.
    /// </summary>
    public class MessageBoxService : IMessageBoxService
    {
        private static readonly IMessageBoxService InstanceValue = new MessageBoxService();

        /// <summary>
        /// Gets an instance of the MessageBoxService.
        /// </summary>
        public static IMessageBoxService Instance => InstanceValue;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}