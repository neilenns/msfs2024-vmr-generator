namespace VmrGenerator.Services
{
    using System;
    using Microsoft.Win32;
    using VmrGenerator.Interfaces;

    /// <summary>
    /// Provides a Win32 FileSave dialog via the ISaveDialogService interface.
    /// </summary>
    public class SaveDialogService : ISaveDialogService
    {
        private static readonly ISaveDialogService InstanceValue = new SaveDialogService();

        /// <summary>
        /// Gets an instance of the MessageBoxService.
        /// </summary>
        public static ISaveDialogService Instance => InstanceValue;

        /// <summary>
        /// Shows a Win32 save file dialog to the user and returns the file name.
        /// </summary>
        /// <returns>The file name if the dialog was closed successfully, or an empty string if canceled.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public string ShowDialog()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Model Matching Rule Sets (*.vmr)|*.vmr|All Files (*.*)|*.*",
                Title = "Save model matching file",
                FileName = "MSFS2024.vmr",
            };

            bool result = saveFileDialog.ShowDialog() ?? false;
            return result ? saveFileDialog.FileName : string.Empty;
        }
    }
}
