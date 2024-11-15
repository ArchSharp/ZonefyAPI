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
                  type= "service_account",
                  project_id= "zoneefystorage",
                  private_key_id= "a95dfddf7c15eba2de0354a240404b4d3f1721c6",
                  private_key= "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCx7yLo53T4ffoU\nKYHTjTbvnv6ksuaPVyvsfIlNMlwY7eMpM0xhPgwgb0OBaSd0XsrSD/hCYkKPg9/O\nlcrMTJ0JmeFS2E4g+sbXt5UTpqvQ5Gqv+5jBItekOoO2lt74+dSpT7rGsCyfKbXJ\n3tkz/wKJsX1vDumNoch/KvEuXMMPUIK8gs8zaF26OyCLEH/SSZ9wxAEosl/j2sN6\nICwSYO2CHeRYliZtm7/vAvp9Trvllr7TjVoBeHHDSDn8mTmqsc+sDFA5bIvmPNWz\n3n0tQ4kCBNSnGeE8dLNfhgvOdsJK9wg/5v8VAOVixRMIO2lFJiGUVIYikt8y7ybH\niKrjmfK/AgMBAAECggEAT1wnJ9XD1+joFwIEQ4SiUfGKBY89QgVRM+K/okypwvym\n6oNUt7Ji2jyF0TxDOqXktBgnVARbR4M72Pn6P3CUQen+E2MiRZdUBHZ+6QrXaw3V\nf7Qph0qw/zCR+mgD/uv+q51B4shEvqBt0iGwon5EdYFSPqxaAq97qyxN8a2x2iwC\nE7vdS0K0WR4HMSZkGW2z/yINjKC3GH/VvMCHgnSyFtZJGjVB17JODNB5Gt6HFc38\nIYAFeZbqt15K5d3rrEvt9ZP0nKumY75394IKKfxzOz32w6I8mIVD3VmZ/ODSZnRa\ndbkWLWZj2Ijjz7DsvtmHiRe6sMXfCkLbYzsltP2MUQKBgQDah/+cO4XFgnZMJafG\nOZ0TDvG+HXwEe6H52i26u/K8Ro7QK4NdvF4Eq840vKXiCkTA7iuUPe0rwGYcY4Ar\nhCIPq/sipPIG486QFQXqTn9UR8WBYbdXj6sQECceEj1v3QTl0aJw1x9dQaC7d8U8\nMbjbHskpGLgnGkMPPVwxQ2r5CwKBgQDQcTTx660jcEXi/o6r+GbtInJ1pGy2YDns\nkIfIszZUBiYIzb8trsUiY250RdJCg26PtK93T90Aaqz8iyFYMJ6MirV/D2NIouO9\nC+bwne7z1R/UvLbgZCqfPaqjwopJ1dh7kfzGYIXtqb6oPTNQDszXLEfFJpN7b7qZ\nNjayRaYFnQKBgQCl4Glj28KUw4ysDEEns//IwaU53AORG489qiDgB14fD6fD1+Ol\nOSBch1TErxVzWLwLfj3SDpeCiE556gUWAvwfzTmZeH7GQBFBSNPuoJsCDGCc7uFO\nM51zWe93Yf0Edk4LbG7THFeQYApzglxSbqOUn++tpDfUZpjUbo0WT8hZNQKBgQCI\nhV0UBjREPcjIP8naQLvJHQKyoprBaI0HsDH+9cHjjpNiuL86gHmFaHJznTd1cPf9\nD27NSnWJTFU4ozeXt2Bg1IjDS6TPckCPFTiQ6CAVe8V5ZJCyr2hrG8yjrUs9yN4d\nMSTKGXYej0rcjhkJmmf3lnz7V6TI+AjTSHrSkGOI0QKBgQCrGy7H5/fhzctB6m88\nEvTO0BJCZc61hWZh0CYicA41EO3kEBcOM0v+QYhgA1tl5+HbbPO9BHHIiUHwXJcl\nshx469IcgVfSs13uu+w9OrMLSxzOKGx99e8Tz3JB7SWqF2g56Sx/CMMdx/eFqpc0\nRlssK7VkT6PLgwzN6/yr4XFOKg==\n-----END PRIVATE KEY-----\n",
                  client_email= "zonefystorage@zoneefystorage.iam.gserviceaccount.com",
                  client_id= "116471458008477900798",
                  auth_uri= "https://accounts.google.com/o/oauth2/auth",
                  token_uri= "https://oauth2.googleapis.com/token",
                  auth_provider_x509_cert_url= "https://www.googleapis.com/oauth2/v1/certs",
                  client_x509_cert_url= "https://www.googleapis.com/robot/v1/metadata/x509/zonefystorage%40zoneefystorage.iam.gserviceaccount.com",
                  universe_domain= "googleapis.com"
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
