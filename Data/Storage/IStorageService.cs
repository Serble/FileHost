namespace FileHostingApi.Data.Storage; 

public interface IStorageService {
    Task UploadFile(string fileName, Stream fileContent);
    Task<Stream?> GetFile(string fileName);
    Task DeleteFile(string fileName);
}