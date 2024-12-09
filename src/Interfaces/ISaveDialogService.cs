namespace vmr_generator.Interfaces
{
    /// <summary>
    /// Provides an interface to show file save dialog to the user.
    /// </summary>
    public interface ISaveDialogService
    {
        public static readonly ISaveDialogService Instance;

        string ShowDialog();
    }
}
