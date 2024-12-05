using System.Windows.Input;

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