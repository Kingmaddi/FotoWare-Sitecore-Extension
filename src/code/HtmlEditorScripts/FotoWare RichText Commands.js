var scEditor = null;

Telerik.Web.UI.Editor.CommandList["ImportFotoWareImage"] = function (commandName, editor, args) {
  scEditor = editor;

  editor.showExternalDialog(
    "/sitecore/shell/default.aspx?xmlcontrol=SelectImageFromFotoWareDialog&currentItemId=" + scItemID + "&fieldtype=RichText",
    null, //argument
    1200,
    700,
    scInsertFotoWareImage,
    null,
    "Bild von FotoWare auswählen",
    true, //modal
    Telerik.Web.UI.WindowBehaviors.Close, // behaviors
    false, //showStatusBar
    false //showTitleBar
  );
};


// CallBack function for Select FotoWare Image Dialog
function scInsertFotoWareImage(sender, dialogInfo) {
  if (!dialogInfo) {
    return;
  }

  var htmlCode = "<img alt='" + dialogInfo.alt + "' src='" + dialogInfo.url + "' />";

  scEditor.pasteHtml(htmlCode, "DocumentManager");
}