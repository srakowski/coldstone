var app = require('app');
var BrowserWindow = require('browser-window');

var mainWindow = null;

app.on('window-all-closed', function () {
    if (process.platform != 'darwin')
        app.quit();
});

app.on('ready', function () {
    mainWindow = new BrowserWindow({ width: 1024, height: 768 });
    mainWindow.setMenu(null);
    mainWindow.loadUrl('file://' + __dirname + '/app.html');
    mainWindow.openDevTools();
    mainWindow.on('closed', function () {
        mainWindow = null;
    });
});


