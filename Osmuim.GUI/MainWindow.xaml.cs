using Osmuim.GUI.Models;
using Osmuim.GUI.Helpers;
using Osmuim.GUI.Services;
using Osmuim.GUI.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Osmuim.GUI
{
    public partial class MainWindow : Window
    {
        private readonly ChromeDataService _chromeDataService;

        private ProgressWindow _progressWindow;

        private const string ChromeDataUrl = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";
        private string currentOsmiumFilePath;

        // this will be used throughout once it's setup
        private ChromeForTesting chromeForTestingData;
        private bool chromeForTestingDataLoaded = false;

        private List<string> chromeVersionsInstalled = new List<string>();
        private List<string> firefoxVersionsInstalled = new List<string>();
        private List<string> edgeVersionsInstalled = new List<string>();

        private List<string> browsersEnabled = new List<string>();

        public MainWindow(ChromeDataService chromeDataService)
        {
            InitializeComponent();
            _chromeDataService = chromeDataService;

            // Start async initialization after construction
            Loaded += async (s, e) => await InitializeBrowserVersionsAsync();
        }

        private async Task InitializeBrowserVersionsAsync()
        {
            // Load installed versions of each browser asynchronously
            var browserVersions = await BrowserFolderChecker.GetBrowserVersionFoldersAsync();
            chromeVersionsInstalled = BrowserFolderChecker.GetBrowserVersions(browserVersions, "Chrome");
            firefoxVersionsInstalled = BrowserFolderChecker.GetBrowserVersions(browserVersions, "Firefox");
            edgeVersionsInstalled = BrowserFolderChecker.GetBrowserVersions(browserVersions, "Edge");

            if (chromeVersionsInstalled.Count > 0)
            {
                browsersEnabled.Add("Chrome");
            }
            if (firefoxVersionsInstalled.Count > 0)
            {
                browsersEnabled.Add("Firefox");
            }
            if (edgeVersionsInstalled.Count > 0)
            {
                browsersEnabled.Add("Edge");
            }

            BrowserSelectionDropdown.ItemsSource = browsersEnabled;

            // Update TextBoxes with populated lists
            ChromeVersionsTextBox.Text = string.Join(Environment.NewLine, chromeVersionsInstalled);
            FirefoxVersionsTextBox.Text = string.Join(Environment.NewLine, firefoxVersionsInstalled);
            EdgeVersionsTextBox.Text = string.Join(Environment.NewLine, edgeVersionsInstalled);
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

            var steps = new List<string>
            {
                $"Downloading Chrome {selectedVersion}...",
                $"Installing Chrome {selectedVersion}...",
                $"Chrome {selectedVersion} installation complete."
            };
            int stepsI = 0;

            // Show the progress window with the total steps
            ShowProgressWindow(steps.Count);

            // Set up temporary download folder and target extraction folder
            string tempDir = Path.Combine(Path.GetTempPath(), "ChromeDownloadTemp");
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;

            // Download and extract files
            StatusBarText.Text = steps[stepsI];
            UpdateProgressWindow(++stepsI, steps[stepsI - 1]);
            await DownloadHelper.DownloadFilesAsync(urls, tempDir);

            StatusBarText.Text = steps[stepsI];
            UpdateProgressWindow(++stepsI, steps[stepsI - 1]);
            ExtractionHelper.ExtractFiles(tempDir, targetDir, selectedVersion);
            await Task.Delay(1000);

            // Clean up
            Directory.Delete(tempDir, true);

            StatusBarText.Text = steps[stepsI];
            UpdateProgressWindow(++stepsI, steps[stepsI - 1]);
            await Task.Delay(1000);

            CloseProgressWindow();

            // Directly call InitializeBrowserVersionsAsync to refresh UI
            await InitializeBrowserVersionsAsync();
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

        // Shows the progress window with the total steps
        public void ShowProgressWindow(int totalSteps)
        {
            // Initialize the ProgressWindow
            _progressWindow = new ProgressWindow
            {
                Owner = this // Set owner to block interaction with the main window
            };

            _progressWindow.SetTotalSteps(totalSteps);
            _progressWindow.Show();
        }

        // Updates the progress window with the current step and text
        public void UpdateProgressWindow(int currentStep, string stepText)
        {
            _progressWindow?.UpdateProgress(currentStep, stepText);
        }

        // Closes the progress window
        public void CloseProgressWindow()
        {
            _progressWindow?.Complete();
            _progressWindow = null;
        }

        private void BrowserSelectionDropdown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Check if a valid item is selected
            if (BrowserSelectionDropdown.SelectedItem is string selectedBrowser && selectedBrowser == "Chrome")
            {
                // Do something when "Chrome" is selected
                RefreshBrowserVersionDropdown(selectedBrowser);
                BrowserVersionDropdown.SelectedIndex = -1;
            }
        }

        private void BrowserVersionDropdown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void RefreshBrowserVersionDropdown(string selectedBrowser)
        {
            switch (selectedBrowser)
            {
                case "Chrome":
                    BrowserVersionDropdown.ItemsSource = chromeVersionsInstalled;
                    break;
                case "Firefox":
                    BrowserVersionDropdown.ItemsSource = firefoxVersionsInstalled;
                    break;
                case "Edge":
                    BrowserVersionDropdown.ItemsSource = edgeVersionsInstalled;
                    break;
            }
        }

        private void ClearExecuteTabSettings_Click(object sender, RoutedEventArgs e)
        {
            DirectoryPathTextBox.Text = null;
            SaveResultsCheckBox.IsChecked = false;
            BrowserVersionDropdown.SelectedIndex = -1;
            BrowserSelectionDropdown.SelectedIndex = -1;
        }
    }
}
