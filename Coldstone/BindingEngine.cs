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
    public class BindingEngine
    {
        private static ViewModelRepository _viewModelRepo = null;

        public Task<object> Initialize(dynamic obj)
        {
            Debugger.Launch();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
            return SyncTask(() =>
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(obj.path);
                        _viewModelRepo = new ViewModelRepository(assembly);
                        return string.Empty;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                });
        }

        public Task<object> CreateViewModel(dynamic obj)
        {            
            return SyncTask(() =>
            {
                var vm = _viewModelRepo.Create(obj.name);
                return vm.Id;
            });
        }

        public Task<object> GetPropertyValue(dynamic obj)
        {
            return AsyncTask(() =>
            {
                var vm = _viewModelRepo.GetViewModel(obj.id);
                return vm.GetPropertyValue(obj.property).ToString();
            });
        }

        public Task<object> SetPropertyValue(dynamic obj)
        {
            return AsyncTask(() =>
            {
                var vm = _viewModelRepo.GetViewModel(obj.id);
                vm.SetPropertyValue(obj.property, obj.value);
                return null;
            });
        }

        public Task<object> BindToProperty(dynamic obj)
        {
            return SyncTask(() =>
            {
                var vm = _viewModelRepo.GetViewModel(obj.id);
                vm.Bind(obj.property, obj.onChanged);
                return null;
            });
        }

        public Task<object> ExecuteCommand(dynamic obj)
        {
            return AsyncTask(() =>
            {
                var vm = _viewModelRepo.GetViewModel(obj.id);
                vm.ExecuteCommand(obj.command);
                return null;
            });
        }

        static Task<object> AsyncTask(Func<object> function)
        {
            return Task.Factory.StartNew(function);
        }

        static Task<object> SyncTask(Func<object> function)
        {
            var task = AsyncTask(function);
            task.Wait();
            return task;
        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
