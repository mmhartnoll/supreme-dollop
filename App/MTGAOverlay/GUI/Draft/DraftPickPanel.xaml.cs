using System.Windows.Controls;

namespace MindSculptor.App.MtgaOverlay.GUI.Draft
{
    /// <summary>
    /// Interaction logic for DraftPickPanel.xaml
    /// </summary>
    internal partial class DraftPickPanel : UserControl
    {
        public string CardName { get; }
        public string Rarity { get; }
        public int Count { get; }

        public DraftPickPanel(string cardName, string rarity, int count)
        {
            CardName = cardName;
            Rarity = rarity;
            Count = count;

            InitializeComponent();

            CardNameDisplay.Text = CardName;
            CountDisplay.Text = $"{Count}x";
        }
    }
}
