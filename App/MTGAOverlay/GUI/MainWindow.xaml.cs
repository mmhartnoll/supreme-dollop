using MindSculptor.App.MtgaOverlay.GUI.Draft;
using MindSculptor.App.MtgaOverlay.LogProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MindSculptor.App.MtgaOverlay.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Initialized(object sender, System.EventArgs e)
        {
            //var logProcessor = new LogProcessor(@"C:\Users\mmhar\OneDrive\Desktop\output_log_19.txt");
            var logProcessor = new LogProcessor(@"C:\Users\mmhar\AppData\LocalLow\Wizards Of The Coast\MTGA\output_log.txt");

            await foreach (var guiEvent in logProcessor.ProcessLogs())
                guiEvent.Invoke(this);
        }

        public async Task LogMessage(string message)
        {
            var border = new Border();
            border.BorderBrush = Brushes.AntiqueWhite;
            border.BorderThickness = new Thickness(1);
            border.Margin = new Thickness(0, 0, 0, 2);

            var containerGrid = new Grid();
            var backgroundGrid = new Grid();

            backgroundGrid.Background = Brushes.DarkSlateGray;
            backgroundGrid.Opacity = 0.4;

            var timeStampTextBlock = new TextBlock();
            timeStampTextBlock.Text = DateTime.Now.ToLongTimeString();
            timeStampTextBlock.FontSize = 8;
            timeStampTextBlock.Background = Brushes.Transparent;
            timeStampTextBlock.Foreground = Brushes.AntiqueWhite;
            timeStampTextBlock.Padding = new Thickness(5, 2, 10, 0);

            var messageTextBlock = new TextBlock();
            messageTextBlock.Text = message;
            messageTextBlock.FontSize = 10;
            messageTextBlock.Background = Brushes.Transparent;
            messageTextBlock.Foreground = Brushes.AntiqueWhite;
            messageTextBlock.Padding = new Thickness(15,14,10,5);
            messageTextBlock.TextWrapping = TextWrapping.Wrap;

            containerGrid.Children.Add(backgroundGrid);
            containerGrid.Children.Add(timeStampTextBlock);
            containerGrid.Children.Add(messageTextBlock);
            border.Child = containerGrid;

            if (LogDisplayPanel.Children.Count >= 12)
                LogDisplayPanel.Children.RemoveAt(0);

            LogDisplayPanel.Children.Add(border);
            await Task.Delay(3000);
            LogDisplayPanel.Children.Remove(border);
        }

        public void ClearScene()
            => ContentGrid.Children.Clear();

        public void DisplayDraftPickOptions(IEnumerable<DraftPickOption> draftPickOptions)
        {
            ContentGrid.Children.Clear();
            var draftPickPanels = draftPickOptions.Select(option => new DraftPickPanel(option.Name, option.Rarity.ToString(), option.InventoryCount));
            var draftPicksScene = new DraftPicksScene(draftPickPanels, DraftPicksSceneConfiguration.Horizontal);
            ContentGrid.Children.Add(draftPicksScene);
        }
    }
}
