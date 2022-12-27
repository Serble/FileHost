namespace FileHostingApi.Data.Schemas; 

public enum BootstrapColor {
    Default,
    Primary,
    Secondary,
    Success,
    Info,
    Warning,
    Danger,
    Link
}

public enum AccountAccessLevel {
    Disabled = 0,
    Normal = 1,
    Admin = 2
}

public enum PageType {
    Account,
    NonAccount,
    Admin
}