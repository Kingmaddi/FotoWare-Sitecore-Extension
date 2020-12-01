//config
var accessToken = "";
var tenantUrl = "";

//state
var selectedImageUrl = "";
var exportedImageUrl = "";
var responseJson = {}

//helper
function getSelectionWidgetUrl() {
  if (accessToken != "" && tenantUrl != "") {
    return tenantUrl + "/fotoweb/widgets/selection#access_token=" + accessToken;
  }
  else {
    return "";
  }
}

function getExportWidgetUrl(imageUrl) {
  return tenantUrl + "/fotoweb/widgets/publish?access_token=" + accessToken + "&i=" + imageUrl;
}

//program
$(document).ready(function () {
  $("#OK").prop('disabled', true);

  accessToken = $("#AccessToken").val();
  tenantUrl = $("#TenantUrl").val();

  //listener
  function listener(event) {
    if (event.origin != tenantUrl) {
      return;
    }

    if (event.data.event === "assetSelected") {
      //image selected
      selectedImageUrl = event.data.asset.href;

      //set data
      responseJson.lastModification = event.data.asset.modified;
      responseJson.alt = responseJson.description = event.data.asset.builtinFields[1].value;
      responseJson.title = event.data.asset.builtinFields[0].value;
      responseJson.description = event.data.asset.builtinFields[1].value;
      responseJson.keywords = event.data.asset.builtinFields[2].value;
      responseJson.metaDataUrl = event.data.asset.href;
      $("#RawValue").val(JSON.stringify(responseJson));

      //load export
      $("#fotowarecontainer").append("<iframe id='exportWidget' height='600' width='100%'' src='" + getExportWidgetUrl(selectedImageUrl) + "'></iframe>");
      $("#selectionWidget").hide();
    }

    if (event.data.event == "assetExported") {
      //image exported
      var imageURL = event.data.export.export.image.normal;
      exportedImageUrl = imageURL;

      //set data
      responseJson.height = event.data.export.export.size.h;
      responseJson.width = event.data.export.export.size.w;
      responseJson.imageUrl = exportedImageUrl;
      $("#RawValue").val(JSON.stringify(responseJson));

      $("#exportWidget").hide();
      $("#OK").prop('disabled', false);
      $("#Cancel").prop('disabled', true);
      $("#OK").click();
      $("#OK").prop('disabled', true);
    }
  }

  addEventListener("message", listener, false);

  //code
  if (getSelectionWidgetUrl() != "") {
    $("#fotowarecontainer").append("<iframe id='selectionWidget' height='600' width='100%'' src='" + getSelectionWidgetUrl() + "'></iframe>");
  }
  else {
    $("#errorMessage").show();
  }
});