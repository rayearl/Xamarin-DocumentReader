namespace DocumentReaderSample;

public partial class SummaryPage : ContentPage
{
    public SummaryPage(IDocReaderScannerEvent scanResult)
    {
        InitializeComponent();

        // Populate the UI with scan results
        DocumentTypeLabel.Text = string.IsNullOrEmpty(scanResult.DocumentType) ? "Not detected" : scanResult.DocumentType;
        NameLabel.Text = string.IsNullOrEmpty(scanResult.SurnameAndGivenNames) ? "Not detected" : scanResult.SurnameAndGivenNames;
        DocumentNumberLabel.Text = string.IsNullOrEmpty(scanResult.DocumentNumber) ? "Not detected" : scanResult.DocumentNumber;
        DateOfBirthLabel.Text = string.IsNullOrEmpty(scanResult.DateOfBirth) ? "Not detected" : scanResult.DateOfBirth;
        DateOfExpiryLabel.Text = string.IsNullOrEmpty(scanResult.DateOfExpiry) ? "Not detected" : scanResult.DateOfExpiry;
        IssuingCountryLabel.Text = string.IsNullOrEmpty(scanResult.IssuingCountry) ? "Not detected" : scanResult.IssuingCountry;

        // Set images if available
        if (scanResult.PortraitField != null && scanResult.PortraitField.Length > 0)
        {
            PortraitImage.Source = ImageSource.FromStream(() => new MemoryStream(scanResult.PortraitField));
        }
        else
        {
            // Keep default placeholder image
            PortraitImage.Source = "mainpage_portrait_icon.png";
        }

        if (scanResult.DocumentField != null && scanResult.DocumentField.Length > 0)
        {
            DocumentImage.Source = ImageSource.FromStream(() => new MemoryStream(scanResult.DocumentField));
        }
        else
        {
            // Keep default placeholder image
            DocumentImage.Source = "mainpage_id_icon.png";
        }
    }

    private async void OnScanAnotherClicked(object sender, EventArgs e)
    {
        // Navigate back to MainPage
        await Navigation.PopAsync();
    }
}
