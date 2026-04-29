namespace onlineShop.Models
{
    public class AppSettings
    {
        public string AdminSecretCode { get; set; }
        public int SessionTimeoutMinutes { get; set; } = 30;
        public string DefaultAdminEmail { get; set; }
        public int MaxLoginAttempts { get; set; } = 5;
        public bool RequireEmailConfirmation { get; set; } = false;
    }
}