using Osmuim.GUI.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Osmuim.GUI.Models;
using Osmuim.GUI.Helpers;
using System;

namespace Osmuim.GUI
{
    public partial class MainWindow : Window
    {
        private readonly ChromeDataService _chromeDataService;

        private const string ChromeDataUrl = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";
        private string currentOsmiumFilePath;

        // this will be used throughout once it's setup
        private ChromeForTesting chromeForTestingData;
        private bool chromeForTestingDataLoaded = false;

        public MainWindow(ChromeDataService chromeDataService)
        {
            InitializeComponent();
            _chromeDataService = chromeDataService;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            chromeForTestingData = await _chromeDataService.FetchChromeDataAsync(ChromeDataUrl);

            // Get the latest Chrome versions and extract version strings
            var chromeVersions = (await _chromeDataService.GetLatestVersionsAsync(chromeForTestingData, 10))
                                 .Select(versionInfo => versionInfo.Version.ToString())
                                 .ToList();

            // Update the ComboBox with the list of versions
            ChromeVersionsDropdown.ItemsSource = chromeVersions;
            ChromeVersionsDropdown.SelectedIndex = 0;

            // Allow UI to refresh
            await Task.Yield();

            // Update it's loaded
            chromeForTestingDataLoaded = true;

            // Update status bar
            StatusBarText.Text = "Installed versions refreshed.";
        }



        //private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var chromeData = await _chromeDataService.FetchChromeDataAsync(ChromeDataUrl);

        //    // Get the latest Chrome versions and extract version strings
        //    var chromeVersions = (await _chromeDataService.GetLatestVersionsAsync(chromeData, 10))
        //                         .Select(versionInfo => versionInfo.Version.ToString())
        //                         .ToList();

        //    // Display versions in the TextBox, each on a new line
        //    ChromeVersionsTextBox.Text = string.Join(Environment.NewLine, chromeVersions);

        //    // Update status bar
        //    StatusBarText.Text = "Installed versions refreshed.";
        //}



        // First Tab: Install Chrome Version Button
        private async void InstallChromeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!chromeForTestingDataLoaded || ChromeVersionsDropdown.SelectedItem == null)
            {
                StatusBarText.Text = "Please refresh versions and select a version to install.";
                return;
            }

            string selectedVersion = ChromeVersionsDropdown.SelectedItem.ToString();
            var selectedVersionInfo = chromeForTestingData.Versions.FirstOrDefault(v => v.Version == selectedVersion);

            if (selectedVersionInfo == null)
            {
                StatusBarText.Text = "Selected version not found.";
                return;
            }

            string platform = Environment.Is64BitOperatingSystem ? "win64" : "win32";
            var urls = await DownloadHelper.GetDownloadUrlsByPlatformAsync(selectedVersionInfo, platform);

            if (urls.Count == 0)
            {
                StatusBarText.Text = $"No downloads available for {platform}.";
                return;
            }

            // Set up temporary download folder and target extraction folder
            string tempDir = Path.Combine(Path.GetTempPath(), "ChromeDownloadTemp");
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;

            // Download and extract files
            StatusBarText.Text = $"Downloading Chrome {selectedVersion}...";
            await DownloadHelper.DownloadFilesAsync(urls, tempDir);

            StatusBarText.Text = $"Installing Chrome {selectedVersion}...";
            ExtractionHelper.ExtractFiles(tempDir, targetDir, selectedVersion);

            // Clean up
            Directory.Delete(tempDir, true);

            StatusBarText.Text = $"Chrome {selectedVersion} installation complete.";
        }



        // First Tab: Install Edge Version Button
        private void InstallEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedVersion = EdgeVersionsDropdown.SelectedItem?.ToString();
            if (selectedVersion != null)
            {
                StatusBarText.Text = $"Installing Edge {selectedVersion}...";
                // Trigger install logic
            }
        }

        // First Tab: Install Firefox Version Button
        private void InstallFirefoxButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedVersion = FirefoxVersionsDropdown.SelectedItem?.ToString();
            if (selectedVersion != null)
            {
                StatusBarText.Text = $"Installing Firefox {selectedVersion}...";
                // Trigger install logic
            }
        }

        // Second Tab: Open File Button
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Osmium Files (*.osmium)|*.osmium|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                currentOsmiumFilePath = openFileDialog.FileName;
                FileContentTextBox.Text = File.ReadAllText(currentOsmiumFilePath);
                StatusBarText.Text = $"Opened file: {currentOsmiumFilePath}";
            }
        }

        // Second Tab: Save File Button
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentOsmiumFilePath != null)
            {
                File.WriteAllText(currentOsmiumFilePath, FileContentTextBox.Text);
                StatusBarText.Text = $"Saved file: {currentOsmiumFilePath}";
            }
            else
            {
                // Trigger the Save As dialog if no file path is set
                NewFileButton_Click(sender, e);
            }
        }

        // Second Tab: New File Button
        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileContentTextBox.Text = string.Empty;
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Osmium Files (*.osmium)|*.osmium|All Files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                currentOsmiumFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentOsmiumFilePath, FileContentTextBox.Text);
                StatusBarText.Text = $"Created new file: {currentOsmiumFilePath}";
            }
        }

        // Third Tab: Set Directory Button
        private void SetDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryPathTextBox.Text = folderDialog.SelectedPath;
                StatusBarText.Text = $"Set results directory: {folderDialog.SelectedPath}";
            }
        }

        // Third Tab: Execute Button
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string browser = BrowserSelectionDropdown.SelectedItem?.ToString();
            string version = BrowserVersionDropdown.SelectedItem?.ToString();
            bool saveResults = SaveResultsCheckBox.IsChecked ?? false;
            string resultsDirectory = DirectoryPathTextBox.Text;

            if (string.IsNullOrWhiteSpace(browser) || string.IsNullOrWhiteSpace(version))
            {
                StatusBarText.Text = "Select a browser and version to execute.";
                return;
            }

            if (saveResults && string.IsNullOrWhiteSpace(resultsDirectory))
            {
                StatusBarText.Text = "Please set a results directory.";
                return;
            }

            StatusBarText.Text = $"Executing with {browser} {version}...";
            ExecutionOutputTextBox.AppendText($"Started execution for {browser} {version}\n");

            // Placeholder for main execution logic
            Task.Run(() =>
            {
                // Simulate execution
                System.Threading.Thread.Sleep(2000);
                Dispatcher.Invoke(() =>
                {
                    ExecutionOutputTextBox.AppendText($"Execution completed for {browser} {version}\n");
                    StatusBarText.Text = "Execution completed.";
                });
            });
        }
    }
}
