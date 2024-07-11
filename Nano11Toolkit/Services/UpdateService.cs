using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Semver;

namespace Nano11Toolkit.Services
{
    public class UpdateService
    {

        private const string GitHubApiUrl = "https://api.github.com/repos/nano11-dev/Nano11Toolkit/releases/latest";
        private Version LocalVersion = new Version(File.ReadAllText("Version.txt").Replace("v", "")); // Replace with actual local version

        public string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (File.Exists(tempDirectory))
            {
                return GetTemporaryDirectory();
            }
            else
            {
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }
        }

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
                    if (new Version(release.TagName) > LocalVersion)
                    {
                        await PromptUserToUpdate(release);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for updates: {ex.Message}");
            }
        }

        

        private async Task PromptUserToUpdate(GitHubRelease release)
        {
            var result = MessageBox.Show($"A new version ({release.TagName}) is available. Do you want to update?", "Update Available", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {

                    using (HttpClient client = new HttpClient())
                    {
                        string TempDir = GetTemporaryDirectory();

                        HttpResponseMessage response = await client.GetAsync(release.Assets[0].DownloadUrl);
                        response.EnsureSuccessStatusCode();
                        File.WriteAllBytes(Path.Combine([TempDir, "Updated.zip"]), await response.Content.ReadAsByteArrayAsync());
                        Debug.WriteLine("Written updated file to " + TempDir);
                        Directory.CreateDirectory(Path.Combine([TempDir, "Extracted"]));
                        System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine([TempDir, "Updated.zip"]), Path.Combine([TempDir, "Extracted"]));
                        Debug.WriteLine("Extracted to " + Path.Combine([TempDir, "Extracted"]));
                        if (AppDomain.CurrentDomain.BaseDirectory == "")
                        {
                            throw new Exception("Not gonna wipe your entire drive man :)");
                        }
                        string BatchFile = $"""
                            taskkill /f /im Nano11Toolkit.exe
                            del /S /Q {AppDomain.CurrentDomain.BaseDirectory}\\*
                            move /y {Path.Combine([TempDir, "Extracted"])}\\* {AppDomain.CurrentDomain.BaseDirectory}\\
                            {AppDomain.CurrentDomain.BaseDirectory}\\Nano11Toolkit.exe
                            pause
                            """;
                        Debug.WriteLine(BatchFile);
                        File.WriteAllText(Path.Combine([TempDir, "update.bat"]), BatchFile);
                        ProcessStartInfo si = new ProcessStartInfo();
                        si.FileName = Path.Combine([TempDir, "update.bat"]);
                        var proc = Process.Start(si);
                        proc.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading update files", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private class GithubAsset
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("browser_download_url")]
            public string DownloadUrl { get; set; }
        }

        private class GitHubRelease
        {
            [JsonPropertyName("tag_name")]
            public string TagName { get; set; }

            [JsonPropertyName("assets")]
            public GithubAsset[] Assets { get; set; }
        }
    }
}
