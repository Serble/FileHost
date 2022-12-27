using FileHostingApi.Data.Schemas;

namespace FileHostingApi.Data; 

public static class UploadLimitManager {

    public static int GetUploadLimit(User? user) {
        if (user == null) {
            return TextMbToBytes(Program.Config!["not_logged_in_upload_limit"]);
        }
        
        switch (user.PermLevel) {
            case 2:  // Admin
                return int.MaxValue;
            case 0:  // Disabled
                return 0;
        }

        if (user.PremiumLevel == 10) {
            return TextMbToBytes(Program.Config!["premium_upload_limit"]);
        }

        return TextMbToBytes(Program.Config!["logged_in_upload_limit"]);
    }
    
    private static int TextMbToBytes(string text) {
        return int.Parse(text)*1000*1000;
    }
    
}