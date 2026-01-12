using ProjectApp.Wpf.Infrastructure;

namespace ProjectApp.Desktop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set => Set(ref _currentViewModel, value);
        }

        public MainViewModel(BaseViewModel startowyEkran)
        {
            _currentViewModel = startowyEkran;
        }
    }
}