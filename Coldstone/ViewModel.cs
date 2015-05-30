using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldstone
{
    class ViewModel
    {
        public string Id { get { return this.GetHashCode().ToString(); } }

        private object _instance;

        private Type _instanceType;

        private bool _capturePropertyChangedEvents;

        private Dictionary<string, Func<object, Task<object>>> _bindings;

        public ViewModel(object instance)
        {
            this._instance = instance;
            this._instanceType = instance.GetType();
            this._capturePropertyChangedEvents = false;
            this._bindings = new Dictionary<string, Func<object, Task<object>>>();
        }
        
        public object GetPropertyValue(string propertyName)
        {
            var propInfo = _instanceType.GetProperty(propertyName);
            return propInfo.GetValue(_instance);
        }

        public void SetPropertyValue(string propertyName, string value)
        {
            var propInfo = _instanceType.GetProperty(propertyName);            
            
            object convertedValue = value;

            if (propInfo.PropertyType == typeof(Int32))
            {
                int val;
                if (!Int32.TryParse(value, out val))
                    val = 0;
                convertedValue = val;
            }

            propInfo.SetValue(_instance, convertedValue);
        }

        public void ExecuteCommand(string commandName)
        {
            var propInfo = _instanceType.GetProperty(commandName);
            var command = propInfo.GetValue(_instance);
            var commandType = command.GetType();
            var methodInfo = commandType.GetMethod("Execute");
            methodInfo.Invoke(command, new object[] { null });
        }

        public void Bind(string propertName, Func<object, Task<object>> callback)
        {
            if (!_capturePropertyChangedEvents)
            {
                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(_instanceType))
                    return;

                var notify = _instance as INotifyPropertyChanged;
                notify.PropertyChanged += notify_PropertyChanged;
                _capturePropertyChangedEvents = true;
            }

            _bindings[propertName] = callback;
        }

        private async void notify_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_bindings.ContainsKey(e.PropertyName))
                return;

            var callback = _bindings[e.PropertyName];
            var propInfo = _instanceType.GetProperty(e.PropertyName);
            await callback(propInfo.GetValue(_instance).ToString());                       
        }
    }
}
