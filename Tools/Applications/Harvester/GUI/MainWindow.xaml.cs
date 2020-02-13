using MindSculptor.App.AppDataContext;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.Tools.Applications.Harvester.Processing;
using MindSculptor.Tools.Extensions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace MindSculptor.Tools.Applications.Harvester.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    EnableControls(false);

                    var selectedSetName = SetSelection.SelectedItem.ToString();
                    var processRunning = false;

                    await dbConnection.OpenAsync();
                    foreach (var selectedSetItem in SetSelection.Items)
                    {
                        var setName = selectedSetItem!.ToString();
                        if (!processRunning && setName != selectedSetName)
                            continue;
                        processRunning = true;
                        processRunning = false;

                        using (var dbTransaction = await dbConnection.BeginTransactionAsync(IsolationLevel.Serializable))
                            try
                            {
                                var dataContext = AppDataContext.Create(dbConnection, dbTransaction);

                                //var selectedSetName = SetSelection.SelectedItem.ToString();
                                var setResult = await dataContext.Cards.Sets
                                .QueryWhere(setDetail => setDetail.Name == setName)
                                .TryGetSingleAsync();
                                if (!setResult.Success)
                                    throw new Exception($"{nameof(SetRecord)} is missing for '{setName}'.");

                                var progress = new Progress<Progress>(ReportProgress);
                                var setProcessor = SetProcessor.Create(dataContext, setResult.Value, progress);
                                await Task.Run(() => setProcessor.ProcessAsync());

                                await dbTransaction.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                await dbTransaction.RollbackAsync();
                                MessageBox.Show(ex.Message);
                            }
                    }
                }
                finally
                {
                    await dbConnection.CloseAsync();
                    EnableControls(true);
                }
        }

        private async Task PopulateSetListAsync()
        {
            SetSelection.Items.Clear();
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    EnableControls(false);
                    await dbConnection.OpenAsync();
                    var dataContext = AppDataContext.Create(dbConnection);
                    var query = dataContext.Cards.Sets
                        .OrderBy(setDetails => setDetails.ReleaseYear)
                        .OrderBy(setDetails => setDetails.ReleaseMonth)
                        .OrderBy(setDetails => setDetails.ReleaseDay);
                    await foreach (var setDetailRecord in query)
                        SetSelection.Items.Add(setDetailRecord.Name);
                }
                finally
                {
                    await dbConnection.CloseAsync();
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

        private const string DBConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MindSculptorApp001;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}
