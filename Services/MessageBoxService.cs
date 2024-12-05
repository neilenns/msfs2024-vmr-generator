using System.Windows;
using vmr_generator.Interfaces;

namespace vmr_generator.Services
{
	public class MessageBoxService : IDialogService
	{
		private static readonly IDialogService _instance = new MessageBoxService();
		/// <summary>
		/// Provides an instance of the MessageBoxService.
		/// </summary>
		public static IDialogService Instance => _instance;

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