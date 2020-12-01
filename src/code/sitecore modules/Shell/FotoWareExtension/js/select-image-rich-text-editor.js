function GetDialogArguments() {
  return getRadWindow().ClientParameters;
}

function getRadWindow() {
  if (window.radWindow) {
    return window.radWindow;
  }

  if (window.frameElement && window.frameElement.radWindow) {
    return window.frameElement.radWindow;
  }

  return null;
}

var isRadWindow = true;

var radWindow = getRadWindow();

if (radWindow) {
  if (window.dialogArguments) {
    radWindow.Window = window;
  }
}

function scClose(selectedImageResponse) {
  // we're passing back an object holding data needed for inserting our special html into the RTE
  var response = JSON.parse(selectedImageResponse);

  var dialogInfo = response;

  getRadWindow().close(dialogInfo);
}

function scCancel() {
  getRadWindow().close();
}

if (window.focus && Prototype.Browser.Gecko) {
  window.focus();
}