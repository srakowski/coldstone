using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coldstone
{
    class ViewModelRepository
    {
        private static Dictionary<string, ViewModel> _viewModels = new Dictionary<string, ViewModel>();

        private Assembly _assembly;

        public ViewModelRepository(Assembly assembly)
        {
            this._assembly = assembly;
        }

        public ViewModel Create(string name)
        {
            var type = _assembly.GetType(name);
            var instance = Activator.CreateInstance(type);
            var vm = new ViewModel(instance);
            _viewModels[vm.GetHashCode().ToString()] = vm;
            return vm;
        }

        public bool Contains(string id)
        {
            return GetViewModel(id) != null;
        }

        public void Add(ViewModel vm)
        {
            _viewModels[vm.GetHashCode().ToString()] = vm;
        }

        public ViewModel GetViewModel(string id)
        {
            if (!_viewModels.ContainsKey(id))
                return null;

            return _viewModels[id];
        }
    }
}
