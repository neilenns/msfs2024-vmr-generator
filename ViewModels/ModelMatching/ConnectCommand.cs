using System.Windows.Input;

// This method of adding commands to view models comes from
// https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090051
namespace vmr_generator.ViewModels.ModelMatching
{
	public partial class ModelMatchingViewModel
	{
		RelayCommand _connectCommand;

		public ICommand ConnectCommand
		{
			get
			{
				{
					_connectCommand ??= new RelayCommand(param => ConnectToSim(),
								param => CanConnectToSim());
					return _connectCommand;
				}
			}
		}


	}
}