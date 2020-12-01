using System;
using System.Net;
using Kingmaddi.Foundation.FotoWareExtension.Logging;
using Kingmaddi.Foundation.FotoWareExtension.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Kingmaddi.Foundation.FotoWareExtension.Repositories
{
  public class FotoWareRepository : IFotoWareRepository
  {
    /// <inheritdoc />  
    public string GetAccessToken(string tenantUrl, string clientId, string clientSecret)
    {
      //try to get access-token
      try
      {
        var client = new RestClient(tenantUrl + "/fotoweb/oauth2/token");
        client.Timeout = -1;
        var request = new RestRequest(Method.POST);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        IRestResponse response = client.Execute(request);

        if (!response.IsSuccessful)
        {
          FotoWareFieldsLog.WriteToLog("--- FotoWare Repository: Authentication exception while calling FotoWare-API to get Access-Token: ---", response.ErrorException);
          return string.Empty;
        }
        else
        {
          var auth = JsonConvert.DeserializeObject<AuthResponseModel>(response.Content);
          return auth.AccessToken;
        }
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- FotoWare Repository: Authentication exception while calling FotoWare-API to get Access-Token: ---", e);
        return string.Empty;
      }
    }

    /// <inheritdoc /> 
    public MetaDataResponse GetMetaData(string imageInfoUrl, string accessToken)
    {
      try
      {
        var client = new RestClient(imageInfoUrl);
        client.Timeout = -1;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Accept", "application/vnd.fotoware.asset+json");
        request.AddHeader("Authorization", "Bearer " + accessToken);
        IRestResponse response = client.Execute(request);

        if (response.IsSuccessful)
        {
          var metaData = JsonConvert.DeserializeObject<FotoWareImageMetaDataModel>(response.Content);
          return new MetaDataResponse()
          {
            StatusCode = HttpStatusCode.OK,
            MetaData = metaData
          };
        }
        else
        {
          FotoWareFieldsLog.WriteToLog($"--- FotoWare Repository: Could not get MetaData : { response.StatusDescription } ---");
          return new MetaDataResponse()
          {
            StatusCode = response.StatusCode
          };
        }
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog("--- FotoWare Repository: Could not get MetaData: ---", e);
        return null;
      }
    }

    /// <inheritdoc />
    public bool IsImageExists(string imageUrl)
    {
      try
      {
        var client = new RestClient(imageUrl);
        client.Timeout = -1;
        var request = new RestRequest(Method.GET);
        IRestResponse response = client.Execute(request);

        if (response.IsSuccessful)
        {
          return true;
        }
        else
        {
          FotoWareFieldsLog.WriteToLog($"--- FotoWare Repository: Could not get Image with Url { imageUrl } : { response.StatusDescription } ---");
          if (response.StatusCode == HttpStatusCode.NotFound)
          {
            return false;
          }
          
          //unknown error
          return true;
        }
      }
      catch (Exception e)
      {
        FotoWareFieldsLog.WriteToLog($"--- FotoWare Repository: Could not get Image with Url { imageUrl } ---", e);
        return true;
      }
    }
  }
}
