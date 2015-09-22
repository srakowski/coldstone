var edge = require('electron-edge');
var path = require('path');

module.exports = function (pathToDll) {
    var self = this;
    var _dirName = path.dirname(pathToDll);

    var _eb = (function () {

        //public Task<object> Initialize(dynamic obj);
        var _initialize = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "Initialize"
        });

        //public Task<object> CreateViewModel(dynamic obj);
        var _createViewModel = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "CreateViewModel"
        });

        //public Task<object> GetPropertyValue(dynamic obj);
        var _getPropertyValue = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "GetPropertyValue"
        });

        //public Task<object> GetPropertyAsViewModel(dynamic obj);
        var _getPropertyAsViewModel = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "GetPropertyAsViewModel"
        });

        //public Task<object> SetPropertyValue(dynamic obj);
        var _setPropertyValue = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "SetPropertyValue"
        });

        //public Task<object> BindToProperty(dynamic obj);
        var _bindToProperty = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "BindToProperty"
        });

        //public Task<object> ExecuteCommand(dynamic obj);
        var _executeCommand = edge.func({
            assemblyFile: _dirName + "/Coldstone.dll",
            typeName: 'Coldstone.EdgeBinder',
            methodName: "ExecuteCommand"
        });

        return {
            initialize: _initialize,
            createViewModel: _createViewModel,
            getPropertyValue: _getPropertyValue,
            setPropertyValue: _setPropertyValue,
            bindToProperty: _bindToProperty,
            executeCommand: _executeCommand,
            getPropertyAsViewModel: _getPropertyAsViewModel
        };
    })();

    function check(result) {
        if (!result.ok) {
            throw result.result;
        } else {
            return result.result;
        }
    };

    var result = check(_eb.initialize({ path: pathToDll }, true));

    var ViewModel = function (id) {
        var self = this;
        var _id = id;

        self.bindInput = function (propertyName, node) {
            check(_eb.bindToProperty({
                id: _id,
                property: propertyName,
                onChanged: function (input, callback) {
                    node.value = input;
                    callback(null, null);
                }
            }, true));

            _eb.getPropertyValue({ id: _id, property: propertyName }, function (err, result) {
                node.value = check(result);
            });

            node.addEventListener("input", function (e) {
                _eb.setPropertyValue({ id: _id, property: propertyName, value: node.value },
                function (err, result) { check(result); });
            });
        };

        self.bindCommand = function (commandName, node) {
            node.addEventListener("click", function (e) {
                _eb.executeCommand({ id: _id, command: commandName },
                function (err, result) { check(result); });
            });
        };

        self.bindText = function (propertyName, node) {
            _eb.bindToProperty({
                id: _id,
                property: propertyName,
                onChanged: function (input, callback) {
                    node.innerText = input;
                    callback(null, null);
                }
            }, true);

            _eb.getPropertyValue({ id: _id, property: propertyName }, function (err, result) {
                node.innerText = check(result);
            });
        };

        self.getChildAsViewModel = function (propertyName) {
            return new ViewModel(check(_eb.getPropertyAsViewModel({ id: _id, property: propertyName }, true)));
        }
    };

    function isDescendant(parent, child) {
        var node = child.parentNode;
        while (node != null) {
            if (node == parent) {
                return true;
            }
            node = node.parentNode;
        }
        return false;
    }

    self.bind = function (idOrNode, typeOrVM) {

        var vm = null;
        var root = null;
        if (typeof idOrNode === "string") {
            var vm = new ViewModel(check(_eb.createViewModel({ name: typeOrVM }, true)));
            var root = document.getElementById(idOrNode);
        } else {
            var vm = typeOrVM;
            root = idOrNode;
        }

        var skip_children = [];
        var bindings = root.querySelectorAll("[data-bind]");

        for (var i = 0; i < bindings.length; i++) {
            var node = bindings[i];

            var skip = false;
            for (var j = 0; j < skip_children.length; j++) {
                if (isDescendant(skip_children[j], node)) {
                    skip = true;
                }
            }

            if (skip)
                continue;

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
            } else if (bindingType === "with:") {
                var child = vm.getChildAsViewModel(propertyName);
                self.bind(node, child);
                skip_children.push(node);
            }
        }
    };
}











