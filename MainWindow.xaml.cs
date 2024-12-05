using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Interop;
using vmr_generator.Services;
using vmr_generator.ViewModels.ModelMatching;

namespace vmr_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ModelMatchingViewModel _modelMatchingViewModel;
        private HwndSource _hwndSource;
        private IntPtr _hwnd;

        public MainWindow()
        {
            InitializeComponent();

            _modelMatchingViewModel = new ModelMatchingViewModel
            {
                MessageBoxService = MessageBoxService.Instance
            };
            DataContext = _modelMatchingViewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            _hwndSource.AddHook(_modelMatchingViewModel.HandleWindowsEvent);
            _hwnd = new WindowInteropHelper(this).Handle;

        }

        protected override void OnClosed(EventArgs e)
        {
            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(_modelMatchingViewModel.HandleWindowsEvent);
                _hwndSource = null;
            }

            base.OnClosed(e);
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            _modelMatchingViewModel.ConnectToSim(_hwnd);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Model Matching Rule Sets (*.vmr)|*.vmr|All Files (*.*)|*.*",
                Title = "Save model matching file",
                FileName = "MSFS2024.vmr"
            };

            bool result = saveFileDialog.ShowDialog() ?? false;
            if (!result)
            {
                return;
            }

            _modelMatchingViewModel.ToXml(saveFileDialog.FileName);
        }

        private void btnGetAircraft_Click(object sender, RoutedEventArgs e)
        {
            _modelMatchingViewModel.GetLiveries();
        }
    }
}
