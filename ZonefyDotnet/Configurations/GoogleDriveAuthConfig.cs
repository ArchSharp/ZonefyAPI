using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ZonefyDotnet.Configurations
{
   public class GoogleDriveAuthConfig
    {
        public DriveService CreateDriveService()
        {
            // Load the service account credential
            var credential = GoogleCredential
                .FromFile("fueldeystorage-9c1a2af845cf.json")
                .CreateScoped(DriveService.Scope.DriveFile);

            // Initialize the DriveService with the service account credentials
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "FuelDeyImages",
            });
        }
    }
}
