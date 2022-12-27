namespace FileHostingApi.Data.Storage; 

public class LocalStorageService : IStorageService {
    
    public async Task UploadFile(string fileName, Stream fileContent) {
        if (Program.Config == null) throw new Exception();
        
        string path = Path.Combine(Program.Config["local_store_path"], fileName);
        await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await fileContent.CopyToAsync(fileStream);
    }
    
    // Get File Method
    public Task<Stream?> GetFile(string fileName) {
        if (Program.Config == null) throw new Exception();
        
        string path = Path.Combine(Program.Config["local_store_path"], fileName);
        if (!File.Exists(path)) {
            return Task.FromResult<Stream?>(null);
        }
        FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(fs);
    }
    
    // Delete File Method
    public Task DeleteFile(string fileName) {
        if (Program.Config == null) throw new Exception();
        
        string path = Path.Combine(Program.Config["local_store_path"], fileName);
        File.Delete(path);
        return Task.CompletedTask;
    }

}