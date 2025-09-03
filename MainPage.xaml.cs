namespace DocumentReaderSample;

public partial class MainPage : ContentPage
{
    readonly IDocReaderScanner docReaderScanner;

    public MainPage()
    {
        InitializeComponent();
        Application.Current.UserAppTheme = AppTheme.Light;
        Application.Current.RequestedThemeChanged += (s, a) => { Application.Current.UserAppTheme = AppTheme.Light; };

        // Disable scan button until initialization is complete
        ScanDocumentButton.IsEnabled = false;

        docReaderScanner = DependencyService.Get<IDocReaderScanner>();
        docReaderScanner.ResultsObtained += OnScanResultsObtained;

        // Initialize the document reader
        IDocReaderInit docReaderInit = DependencyService.Get<IDocReaderInit>();
        docReaderInit.ScenariosObtained += (object sender, IDocReaderInitEvent e) =>
        {
            if (e.IsSuccess)
            {
                StatusLabel.Text = "Ready to scan";
                ScanDocumentButton.IsEnabled = true;
            }
            else
            {
                StatusLabel.Text = "Initialization failed";
                ScanDocumentButton.IsEnabled = false;
            }
        };
        StatusLabel.Text = "Initializing...";
        docReaderInit.InitDocReader();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Reset status when returning to this page
        if (ScanDocumentButton.IsEnabled)
        {
            StatusLabel.Text = "Ready to scan";
        }
    }

    private async void ScanDocument_Clicked(object sender, EventArgs e)
    {
        StatusLabel.Text = "Opening scanner...";
        ScanDocumentButton.IsEnabled = false;
        docReaderScanner.ShowScanner(false); // Disable RFID for simplicity
    }

    private async void OnScanResultsObtained(object sender, IDocReaderScannerEvent e)
    {
        // Re-enable button in case scan was cancelled or failed
        ScanDocumentButton.IsEnabled = true;
        StatusLabel.Text = "Ready to scan";

        // Navigate to summary page with results
        await Navigation.PushAsync(new SummaryPage(e));
    }
}
