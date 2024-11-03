using System.Windows;
using System.Windows.Controls;

namespace Osmuim.GUI.Views
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent(); // Ensures the XAML elements are initialized
        }

        // Sets the total number of steps for the progress bar
        public void SetTotalSteps(int totalSteps)
        {
            ProgressBar.Maximum = totalSteps;
        }

        // Updates the current progress in the progress bar and text
        public void UpdateProgress(int currentStep, string stepText)
        {
            ProgressBar.Value = currentStep;
            ProgressText.Text = $"{currentStep}/{ProgressBar.Maximum} - {stepText}";
        }

        // Closes the progress window
        public void Complete()
        {
            Close();
        }
    }
}
