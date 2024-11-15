using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
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
            //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zoneefystorage-ee7b59340270.json");
            //Console.WriteLine("Using Service Account File: " + filePath);

            //// Load the service account credential
            //var credential = GoogleCredential
            //    .FromFile(filePath)
            //    .CreateScoped(DriveService.Scope.DriveFile);

            // Load the service account credential
            var credentialPath = _driveAccess.ServiceAccount;// Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            Console.WriteLine($"Credential Path: {credentialPath}");
            var credential = GoogleCredential
                .FromJson(credentialPath)
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
