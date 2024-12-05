namespace vmr_generator.Interfaces
{
	/// <summary>
	/// Provides an interface to show a message to the user.
	/// </summary>
	public interface IDialogService
	{
		public static readonly IDialogService Instance;

		void ShowError(string message, string title);
		void ShowMessage(string message, string title);
	}
}