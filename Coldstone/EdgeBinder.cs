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
    public class EdgeBinder
    {
        private static BinderCore _binder = null;

        public Task<object> Initialize(dynamic obj)
        {
            _binder = new BinderCore();
            return SyncTask(() =>
                {
                    var result = _binder.Initialize(obj.path);
                    return EdgeResult.Ok(result);
                });
        }

        public Task<object> CreateViewModel(dynamic obj)
        {
            return SyncTask(() =>
                {
                    return _binder.CreateViewModel(obj.name);
                });
        }

        public Task<object> GetPropertyValue(dynamic obj)
        {
            return AsyncTask(() =>
            {
                return _binder.GetPropertyValue(obj.id, obj.property);
            });
        }

        public Task<object> SetPropertyValue(dynamic obj)
        {
            return AsyncTask(() =>
            {
                return _binder.SetPropertyValue(obj.id, obj.property, obj.value);
            });
        }

        public Task<object> GetPropertyAsViewModel(dynamic obj)
        {
            return SyncTask(() =>
            {
                return _binder.GetPropertyAsViewModel(obj.id, obj.property);
            });
        }

        public Task<object> BindToProperty(dynamic obj)
        {
            return SyncTask(() =>
            {
                return _binder.BindToProperty(obj.id, obj.property, obj.onChanged);
            });
        }

        public Task<object> ExecuteCommand(dynamic obj)
        {
            return AsyncTask(() =>
            {
                return _binder.ExecuteCommand(obj.id, obj.command);
            });
        }

        static Task<object> SyncTask(Func<object> function)
        {
            var task = AsyncTask(function);
            task.Wait();
            return task;
        }

        static Task<object> AsyncTask(Func<object> function)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var result = function.Invoke();
                    return EdgeResult.Ok(result);
                }
                catch (Exception ex)
                {
                    return EdgeResult.NotOk(ex.Message);
                }
            });
            
        }        
    }
}
