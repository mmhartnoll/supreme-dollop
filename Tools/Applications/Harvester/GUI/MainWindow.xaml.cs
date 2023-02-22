using MindSculptor.App.AppDataContext;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.Tools.Applications.Harvester.Processing;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MindSculptor.Tools.Applications.Harvester.GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async void InitializeAsync(object sender, RoutedEventArgs e)
            => await PopulateSetListAsync();

        private async void ProcessSetAsync(object sender, RoutedEventArgs e)
        {
            await using var dataContext = AppDataContext.Create(DBConnectionString);
            try
            {
                EnableControls(false);

                var selectedSetName = SetSelection.SelectedItem.ToString();
                var processRunning = false;

                foreach (var selectedSetItem in SetSelection.Items)
                {
                    var setName = selectedSetItem!.ToString();
                    if (!processRunning && setName != selectedSetName)
                        continue;
                    //processRunning = true;

                    await using var transaction = await dataContext.BeginTransactionAsync();
                    try
                    {
                        await transaction.ExecuteAsync(TransactionScope);
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        MessageBox.Show(ex.Message);
                    }

                    async Task TransactionScope()
                    {
                        var setResult = await dataContext.Cards.Sets
                            .QueryWhere(setDetail => setDetail.Name == setName)
                            .TryGetSingleAsync();
                        if (!setResult.Success)
                            throw new Exception($"{nameof(SetRecord)} is missing for '{setName}'.");

                        var progress = new Progress<Progress>(ReportProgress);
                        var setProcessor = SetProcessor.Create(dataContext, setResult.Value, progress);
                        await Task.Run(() => setProcessor.ProcessAsync());
                    }
                }
            }
            finally
            {
                EnableControls(true);
            }
        }

        private async Task PopulateSetListAsync()
        {
            SetSelection.Items.Clear();
            await using var dataContext = AppDataContext.Create(DBConnectionString);
            try
            {
                EnableControls(false);
                var query = dataContext.Cards.Sets
                    .OrderBy(setDetails => setDetails.ReleaseYear)
                    .OrderBy(setDetails => setDetails.ReleaseMonth)
                    .OrderBy(setDetails => setDetails.ReleaseDay);
                await foreach (var setDetailRecord in query)
                    SetSelection.Items.Add(setDetailRecord.Name);
            }
            finally
            {
                if (SetSelection.Items.Count > 0)
                    SetSelection.SelectedIndex = 0;
                EnableControls(true);
            }
        }

        private void ReportProgress(Progress progress)
        {
            if (progress.Total > 0)
            {
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Value = progress.Current * 100.0 / progress.Total;
            }
            else
                ProgressBar.IsIndeterminate = true;
            ProgressTitle.Text = progress.Title;
            ProgressDetail.Text = progress.Detail;
        }

        private void EnableControls(bool enable)
        {
            SetSelection.IsEnabled = enable;
            //BtnAddNewSet.IsEnabled = enable;
            BtnProcessSet.IsEnabled = enable;
        }

        private const string DBConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MindSculptorApp;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
    }
}
