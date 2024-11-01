using Osmuim.GUI.Models;
using Osmuim.GUI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace Osmuim.GUI
{
    public partial class MainWindow : Window
    {
        private readonly ChromeDataService chromeDataService = new ChromeDataService();
        private const string ChromeDataUrl = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";
        private string currentOsmiumFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Example lists of versions (replace with actual data retrieval logic)
            var firefoxVersions = new List<string> { "Firefox 89.0", "Firefox 88.0", "Firefox 87.0" };
            var edgeVersions = new List<string> { "Edge 91.0", "Edge 90.0", "Edge 89.0" };
            var chromeVersions = new List<string> { "Chrome 91.0", "Chrome 90.0", "Chrome 89.0" };

            // Populate the TextBoxes with the list content, joined by newline
            FirefoxVersionsTextBox.Text = string.Join(Environment.NewLine, firefoxVersions);
            EdgeVersionsTextBox.Text = string.Join(Environment.NewLine, edgeVersions);
            ChromeVersionsTextBox.Text = string.Join(Environment.NewLine, chromeVersions);

            // Update status bar
            StatusBarText.Text = "Installed versions refreshed.";
        }


        // First Tab: Install Chrome Version Button
        private void InstallChromeButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder logic to install selected Chrome version.
            string selectedVersion = ChromeVersionsDropdown.SelectedItem?.ToString();
            if (selectedVersion != null)
            {
                StatusBarText.Text = $"Installing Chrome {selectedVersion}...";
                // Trigger install logic
            }
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
