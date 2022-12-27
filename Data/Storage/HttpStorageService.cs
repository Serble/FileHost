using System.Net;
using System.Text.Encodings.Web;
using GeneralPurposeLib;

namespace FileHostingApi.Data.Storage; 

public class HttpStorageService : IStorageService {
    
    public async Task UploadFile(string fileName, Stream fileContent) {
        if (Program.Config == null) throw new Exception();
        Logger.Debug("Attempting to upload file " + fileName);
        // create a new HttpClient
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Program.Config["http_authorization_token"]);
        // create a new StreamContent
        StreamContent fileContentStreamContent = new StreamContent(fileContent);
        // send the request
        Logger.Debug("Sending request...");
        HttpResponseMessage response = await client.PutAsync(Program.Config["http_url"] + UrlEncoder.Create().Encode(fileName), fileContentStreamContent);
        Logger.Debug("Response received.");
        // get the response content
        
        string responseContent = response.Content.ReadAsStringAsync().Result;
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Error uploading file " + fileName + ": " + responseContent);
        }
    }
    
    // Get File Method
    public async Task<Stream?> GetFile(string fileName) {
        if (Program.Config == null) throw new Exception();
        Logger.Debug("Attempting to get file " + fileName);
        // create a new HttpClient
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Program.Config["http_authorization_token"]);
        // send the request
        Logger.Debug("Sending request...");
        HttpResponseMessage response = await client.GetAsync(Program.Config["http_url"] + UrlEncoder.Create().Encode(fileName));
        Logger.Debug("Response received.");
        // get the response content
        // string responseContent = response.Content.ReadAsStringAsync().Result;
        if (response.StatusCode == HttpStatusCode.NotFound) {
            return null;
        }
        if (!response.IsSuccessStatusCode) {
            string responseContent = response.Content.ReadAsStringAsync().Result;
            throw new Exception("Error getting file " + fileName + ": " + responseContent);
        }
        return await response.Content.ReadAsStreamAsync();
    }
    
    // Delete File Method
    public async Task DeleteFile(string fileName) {
        if (Program.Config == null) throw new Exception();
        Logger.Debug("Attempting to delete file " + UrlEncoder.Create().Encode(fileName));
        // create a new HttpClient
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Program.Config["http_authorization_token"]);
        // send the request
        Logger.Debug("Sending request...");
        HttpResponseMessage response = await client.DeleteAsync(Program.Config["http_url"] + fileName);
        Logger.Debug("Response received.");
        // get the response content
        string responseContent = response.Content.ReadAsStringAsync().Result;
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Error deleting file " + fileName + ": " + responseContent);
        }
    }

}