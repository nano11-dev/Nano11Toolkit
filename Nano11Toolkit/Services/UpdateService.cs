using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Nano11Toolkit.Services
{
    public class UpdateService
    {

        private const string GitHubApiUrl = "https://api.github.com/repos/nano11-dev/Nano11Toolkit/releases/latest";
        private string LocalVersion = File.ReadAllText("Version.txt"); // Replace with actual local version

        public async Task CheckForUpdatesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Debug.WriteLine("Running Check for updates");
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
                    var response = await client.GetStringAsync(GitHubApiUrl);
                    var release = JsonSerializer.Deserialize<GitHubRelease>(response);
                    Debug.WriteLine("Local version: " + LocalVersion);
                    Debug.WriteLine("Remote version: " +  release.TagName);
                    if (IsNewerVersion(release.TagName, LocalVersion))
                    {
                        PromptUserToUpdate(release);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for updates: {ex.Message}");
            }
        }

        private bool IsNewerVersion(string latestVersion, string localVersion)
        {
            return string.Compare(latestVersion, localVersion, StringComparison.Ordinal) > 0;
        }

        private void PromptUserToUpdate(GitHubRelease release)
        {
            var result = MessageBox.Show($"A new version ({release.TagName}) is available. Do you want to update?", "Update Available", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                // Implement update logic here
            }
        }

        private class GitHubRelease
        {
            [JsonPropertyName("tag_name")]
            public string TagName { get; set; }
        }
    }
}
