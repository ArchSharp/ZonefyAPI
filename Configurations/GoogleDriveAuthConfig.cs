using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ZonefyDotnet.Helpers;

namespace ZonefyDotnet.Configurations
{
   public class GoogleDriveAuthConfig
    {
        public DriveService DriveService { get; private set; }
        private readonly DriveAccess _driveAccess;

        public GoogleDriveAuthConfig(
            IOptions<DriveAccess> driveAccess
            )
        {
            _driveAccess = driveAccess.Value;
            DriveService = CreateDriveService();

            // Call the test method to verify the setup
            TestGoogleDriveService();
        }

        public DriveService CreateDriveService()
        {
            //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service-account.json");
            //Console.WriteLine("Using Service Account File: " + filePath);

            //// Load the service account credential
            //var credential = GoogleCredential
            //    .FromFile(filePath)
            //    .CreateScoped(DriveService.Scope.DriveFile);

            // Reconstructed service account object
            var serviceAccount = new
            {
                type = "service_account",
                project_id = "zoneefystorage",
                private_key_id = "fcfc46be0a9138502e5eae7ea5a6b8b358b28ac2",
                private_key = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC/62aRICiBb89w\nTAV4Exp7VGIkCHTsBFtf8t4FtHny0T5ZxCWjZbFNiLYCD8ntWHZ3Xb1YbRT8PVz4\no7NMTjww1IRZW2mHc4L11rECgzEnrGmCypKDSYFNyJHg+2zIK83DFEw7mqcWzXie\nEfNmQLatDafL/UdD739K7F36Qq2tJKGEGdmZnGIYy/x0hupMD1GEtVOEfXVqmkdp\nZTdHugYT/04RhRWAGxRjDa3cVhLLM2CKPsNa0KegQZ49HytX1mWKggQ5Ij7bhjfy\nJvrPfrhsfmMvDP1eLXqhb6tzlOXO6dR18LVLX+CIYSTblW8V2mvL6DhUgMttxiqJ\n7lqW1mmtAgMBAAECggEAWvkBTfKh8LUo7fDDOOdJSsO1n6H4lnX8QE+WCbtCgSTJ\nevCc7MbEFF4k9HY6gAzKXwuquNNAOs71lpC2aYm+Q2FBeT/5FnFdVx4VGv1IHCfA\nffR7HcuGhPH5oF1d88a7yuiAuFI3OPpd3gPhTguH7CEIdIwTL3ND8iZ356yaufLL\nsiPuZoZBB64QTkfUeW5dpRAHVde7w2dhZyL2uiwxaPYsTyxRGNqYXlNmL5REjDdf\nlcJWXIFRvI7j3HeGwSmNIaruWsAufKpQlytFxqtYTGms7O84AqlB019dTpX3gWk/\nSeT9ZGLbOM1q+F2elHAmv5n8fQjRJV1VlBcoppX6uwKBgQDysKK1m4ppNhFW1kRI\ny9JxhvFs9i6DCKFl3tDLW1FF+xgqgf/X7OTYH8e96zdfuGElR0upKCx3YjS5Mmk6\ng9oECQNYm4aw5ijWZ+TGvxWbrid6MEpyjiSSgWc4id9RPHEr1EadPxIBWA9Nmpmw\nWCM5GxHHYl5NK30Wo4wCviocFwKBgQDKcfLWKfqwE29IPIVZZa3yziY5jG7duOXE\ntyNZPJZXUwgh2ygA6Pqe5VPZ/SwZBuoEAcEcAxZRyoT9kkIM9LtQe6OpmF7StRXP\nEAawmOp/wLmyQGhsQ/l1scFJBaBaCBRwORrPtFMumkTDyX5AtDVHSURnWoIh+oWb\nFZfII8Xu2wKBgBdGI0SBVyPB4KXQRJoqmdLzWwHVFh7Q3BGV7bYi2sd4Thf9gZim\nikfDvHcVMKlIWGCn1rSxOpp0W5QLJpZLAMR0TNLZj3A1kmMFcaYxuxPD/tsz+F/G\nC/HU4R5F8pmqzzuhzufTnZltZYJOukB0I2SdmPXjFCIYcpWSN6IntIgfAoGBALzJ\n3qeRplIlDZXTupdPlxPi4wqvx4PFNaBpGPHS6nCs4dyQv5F5sp4TRkr3KfR82ia6\nbBdvWZUUw0ik6Cuk3jmD03AIxATDEn9ydoqhT62rUIDGR5sPGMeE6LbsJElRV53/\n4tznUNsYyh+6+53jb/v2e2/Wm5yMP6QMUus1URQZAoGBAImFSJL+ed46/UwkXICt\nE28IrYgVwqIe+bz8WCPH0vLFLp7ssObpjE3x5LJZEtKvYRPNfObD85dx0w/D9XxL\n9p4dWGEmpc3jKHhs4EjN3I4PD7pcPhUaTK+osH/7dPo3e6UGzO2pgOZa2QZ40Fd+\np35s2tCqC/FggIQMLG7xJORi\n-----END PRIVATE KEY-----\n",
                client_email = "zonefystorage@zoneefystorage.iam.gserviceaccount.com",
                client_id = "116471458008477900798",
                auth_uri = "https://accounts.google.com/o/oauth2/auth",
                token_uri = "https://oauth2.googleapis.com/token",
                auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
                client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/zonefystorage%40zoneefystorage.iam.gserviceaccount.com",
                universe_domain = "googleapis.com"
            };

            // Serialize to JSON
            string json = JsonSerializer.Serialize(serviceAccount, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Write to temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), "service-account.json");
            File.WriteAllText(tempFilePath, json);

            // Log file location
            Console.WriteLine($"Service account file written to: {tempFilePath}");

            // Initialize Google Credential
            var credential = GoogleCredential
                .FromFile(tempFilePath)
                .CreateScoped(DriveService.Scope.DriveFile);

            // Initialize the DriveService with the service account credentials
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "ZonefyImages",
            });
        }

        public void TestGoogleDriveService()
        {
            try
            {
                var request = DriveService.Files.List();
                var files = request.Execute().Files;

                Console.WriteLine("Google Drive API call successful. Files:");
                foreach (var file in files)
                {
                    Console.WriteLine($"- {file.Name} (ID: {file.Id})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing Google Drive: {ex.Message}");
            }
        }
    }
}
