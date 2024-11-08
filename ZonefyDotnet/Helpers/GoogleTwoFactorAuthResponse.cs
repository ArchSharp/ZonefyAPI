namespace ZonefyDotnet.Helpers
{
    public class GoogleTwoFactorAuthResponse
    {
        public string QrCodeSetupImageUrl { get; set; }
        public string Account { get; set; }
        public string ManualEntryKey { get; set; }
    }    
}
