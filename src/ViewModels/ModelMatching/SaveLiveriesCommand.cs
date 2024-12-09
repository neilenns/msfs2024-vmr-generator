using System.Windows.Input;
using vmr_generator.Helpers;

namespace vmr_generator.ViewModels.ModelMatching
{
    public partial class ModelMatchingViewModel
    {
        RelayCommand _saveLiveriesCommand;

        public ICommand SaveLiveriesCommand => _saveLiveriesCommand ??= new RelayCommand(param => SaveLiveries(), param => CanSaveLiveries());

        public void SaveLiveries()
        {
            var fileName = SaveDialogService.ShowDialog();

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            ToXml(fileName);
        }

        public bool CanSaveLiveries()
        {
            return Liveries.Count > 0;
        }
    }
}


