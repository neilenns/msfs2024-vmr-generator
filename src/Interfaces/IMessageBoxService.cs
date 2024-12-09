namespace VmrGenerator.Interfaces
{
    /// <summary>
    /// Provides an interface to show a message to the user.
    /// </summary>
    public interface IMessageBoxService
    {
        public static readonly IMessageBoxService Instance;

        void ShowError(string message, string title);
        void ShowMessage(string message, string title);
    }
}