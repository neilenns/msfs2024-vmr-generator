namespace VmrGenerator.Interfaces
{
    /// <summary>
    /// Provides an interface to show file save dialog to the user.
    /// </summary>
    public interface ISaveDialogService
    {
        /// <summary>
        /// Returns an instance of a save dialog service.
        /// </summary>
        public static readonly ISaveDialogService Instance;

        /// <summary>
        /// Shows a save dialog to the user.
        /// </summary>
        /// <returns>The filename to save to, or null if no filename specified.</returns>
        string ShowDialog();
    }
}
