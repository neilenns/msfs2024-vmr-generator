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

        public MainWindow()
        {
            InitializeComponent();

            _modelMatchingViewModel = new ModelMatchingViewModel
            {
                MessageBoxService = MessageBoxService.Instance,
                SaveDialogService = SaveDialogService.Instance
            };
            DataContext = _modelMatchingViewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            _hwndSource.AddHook(_modelMatchingViewModel.HandleWindowsEvent);
            _modelMatchingViewModel.WindowHandle = new WindowInteropHelper(this).Handle;
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
    }
}
