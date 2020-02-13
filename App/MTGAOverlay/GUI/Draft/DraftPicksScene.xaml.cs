using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MindSculptor.App.MtgaOverlay.GUI.Draft
{
    /// <summary>
    /// Interaction logic for DraftPicksScene.xaml
    /// </summary>
    internal partial class DraftPicksScene : UserControl
    {
        private readonly IEnumerable<DraftPickPanel> draftPickPanels;

        public DraftPicksSceneConfiguration Configuration { get; }

        public DraftPicksScene(IEnumerable<DraftPickPanel> draftPickPanels, DraftPicksSceneConfiguration configuration)
        {
            this.draftPickPanels = draftPickPanels;
            Configuration = configuration;

            InitializeComponent();
            SetLayout();
        }

        private void SetLayout()
        {
            ContentGrid.Children.Clear();
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.ColumnDefinitions.Clear();

            switch (Configuration)
            {
                case DraftPicksSceneConfiguration.Horizontal:
                    SetHorizontalLayout();
                    break;
                case DraftPicksSceneConfiguration.Vertical:
                    SetVerticalLayout();
                    break;
                default:
                    throw new NotSupportedException($"{nameof(DraftPicksSceneConfiguration)} value '{Configuration}' is not supported.");
            }
        }

        private void SetHorizontalLayout()
        {
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(400, GridUnitType.Star) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(804, GridUnitType.Star) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(956, GridUnitType.Star) });

            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(74, GridUnitType.Star) }); // -500 for log panel
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2496, GridUnitType.Star) });
            ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(770, GridUnitType.Star) });

            var innerGrid = new Grid();
            innerGrid.SetValue(Grid.RowProperty, 1);
            innerGrid.SetValue(Grid.ColumnProperty, 1);
            ContentGrid.Children.Add(innerGrid);

            innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            innerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            var ordinal = 0;
            foreach (var panel in draftPickPanels)
            {
                panel.SetValue(Grid.RowProperty, (int)Math.Floor(ordinal / 8m));
                panel.SetValue(Grid.ColumnProperty, (int)(ordinal++ % 8m));

                innerGrid.Children.Add(panel);
            }
        }

        private void SetVerticalLayout()
        {
            throw new NotImplementedException("Vertical layout not yet supported.");
        }
    }
}
