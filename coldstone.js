var edge = require('electron-edge');
var path = require('path');

module.exports = function (pathToDll) {
    var self = this;
    var _dirName = path.dirname(pathToDll);

    //public Task<object> Initialize(dynamic obj);
    var _initialize = edge.func({
        assemblyFile: _dirName + "/Coldstone.dll",
        typeName: 'Coldstone.BindingEngine',
        methodName: "Initialize"
    });

    //public Task<object> CreateViewModel(dynamic obj);
    var _createViewModel = edge.func({
        assemblyFile: _dirName + "/Coldstone.dll",
        typeName: 'Coldstone.BindingEngine',
        methodName: "CreateViewModel"
    });

    //public Task<object> GetPropertyValue(dynamic obj);
    var _getPropertyValue = edge.func({
        assemblyFile: _dirName + "/Coldstone.dll",
        typeName: 'Coldstone.BindingEngine',
        methodName: "GetPropertyValue"
    });

    //public Task<object> SetPropertyValue(dynamic obj);
    var _setPropertyValue = edge.func({
        assemblyFile: _dirName + "/Coldstone.dll",
        typeName: 'Coldstone.BindingEngine',
        methodName: "SetPropertyValue"
    });

    //public Task<object> BindToProperty(dynamic obj);
    var _bindToProperty = edge.func({
        assemblyFile: _dirName + "/Coldstone.dll",
        typeName: 'Coldstone.BindingEngine',
        methodName: "BindToProperty"
    });

    //public Task<object> ExecuteCommand(dynamic obj);
    var _executeCommand = edge.func({
        assemblyFile: _dirName + "/Coldstone.dll",
        typeName: 'Coldstone.BindingEngine',
        methodName: "ExecuteCommand"
    });

    var result = _initialize({ path: pathToDll }, true);

    var ViewModel = function (name) {
        var self = this;
        var _id = _createViewModel({ name: name }, true);

        self.bindInput = function (propertyName, node) {
            _bindToProperty({
                id: _id,
                property: propertyName,
                onChanged: function (input, callback) {
                    node.value = input;
                    callback(null, null);
                }
            }, true)

            _getPropertyValue({ id: _id, property: propertyName }, function (err, result) {
                node.value = result;
            });

            node.addEventListener("input", function (e) {
                _setPropertyValue({ id: _id, property: propertyName, value: node.value },
                function (err, result) { });
            });
        };

        self.bindCommand = function (commandName, node) {
            node.addEventListener("click", function (e) {
                _executeCommand({ id: _id, command: commandName },
                function (err, result) { });
            });
        };

        self.bindText = function (propertyName, node) {
            _bindToProperty({
                id: _id,
                property: propertyName,
                onChanged: function (input, callback) {
                    node.innerText = input;
                    callback(null, null);
                }
            }, true);

            _getPropertyValue({ id: _id, property: propertyName }, function (err, result) {
                node.innerText = result;
            });
        };
    };

    self.bind = function (id, type) {
        var vm = new ViewModel(type);
        var root = document.getElementById(id);
        var bindings = root.querySelectorAll("[data-bind]");
        for (var i = 0; i < bindings.length; i++) {
            var node = bindings[i];
            var binding = node.dataset.bind;
            var bindingParts = binding.split(' ');
            var bindingType = bindingParts[0];
            var propertyName = bindingParts[1];
            if (bindingType === "value:") {
                vm.bindInput(propertyName, node);
            } else if (bindingType === "command:") {
                vm.bindCommand(propertyName, node);
            } else if (bindingType === "text:") {
                vm.bindText(propertyName, node);
            }
        }
    };
}











