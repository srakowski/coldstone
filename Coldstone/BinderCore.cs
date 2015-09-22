using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coldstone
{
    class BinderCore
    {
        private ViewModelRepository _viewModelRepo = null;

        public BinderCore()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        public object Initialize(string assemblyPath)
        {
            var assembly = Assembly.LoadFile(assemblyPath);
            _viewModelRepo = new ViewModelRepository(assembly);
            return string.Empty;
        }

        public object CreateViewModel(string viewModelName)
        {
            var vm = _viewModelRepo.Create(viewModelName);
            return vm.Id;
        }

        public object GetPropertyValue(string id, string property)
        {
            var vm = _viewModelRepo.GetViewModel(id);
            return vm.GetPropertyValue(property).ToString();
        }

        public object GetPropertyAsViewModel(string id, string property)
        {
            var vm = _viewModelRepo.GetViewModel(id);
            var child = vm.GetPropertyAsViewModel(property);
            if (!_viewModelRepo.Contains(child.Id))
            {
                _viewModelRepo.Add(child);
            }
            return child.Id;
        }

        public object SetPropertyValue(string id, string property, string value)
        {            
            var vm = _viewModelRepo.GetViewModel(id);
            vm.SetPropertyValue(property, value);
            return null;
        }

        public object BindToProperty(string id, string property, Func<object, Task<object>> onChanged)
        {
            var vm = _viewModelRepo.GetViewModel(id);
            vm.Bind(property, onChanged);
            return null;
        }

        public object ExecuteCommand(string id, string command)
        {
            var vm = _viewModelRepo.GetViewModel(id);
            vm.ExecuteCommand(command);
            return null;
        }
    }
}
