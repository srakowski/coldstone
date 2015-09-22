using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Coldstone.Example.ViewModels
{
    public class AdderViewModel : ViewModelBase
    {
        private int _number1 = 0;

        public int InNumber1
        {
            get { return _number1; }
            set
            {
                _number1 = value;
                RaisePropertyChanged();
            }
        }

        private int _number2 = 0;

        public int InNumber2
        {
            get { return _number2; }
            set
            {
                _number2 = value;
                RaisePropertyChanged();
            }
        }

        private int _outNumber = 0;

        public int OutNumber
        {
            get { return _outNumber; }
            set
            {
                _outNumber = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand AddCommand { get; set; }

        public AdderViewModel()
        {
            AddCommand = new RelayCommand(Add);
        }

        private void Add()
        {
            OutNumber = InNumber1 + InNumber2;
        }
    }
}
