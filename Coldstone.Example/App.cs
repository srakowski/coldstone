using Coldstone.Example.ViewModels;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldstone.Example
{
    public class App : ViewModelBase
    {
        public AdderViewModel Adder { get; set; }

        public App()
        {
            Adder = new AdderViewModel();
        }
    }
}
