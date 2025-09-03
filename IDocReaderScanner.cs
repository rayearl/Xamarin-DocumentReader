namespace DocumentReaderSample
{
    public interface IDocReaderScanner
    {
        void ShowScanner(bool IsReadRfid);
        void RecognizeImage(Stream stream, bool IsReadRfid);
        void SelectScenario(string scenarioName);
        event EventHandler<IDocReaderScannerEvent> ResultsObtained;
    }
    public interface IDocReaderScannerEvent
    {
        string SurnameAndGivenNames { get; set; }
        byte[] PortraitField { get; set; }
        byte[] DocumentField { get; set; }
        string DocumentType { get; set; }
        string DocumentNumber { get; set; }
        string DateOfBirth { get; set; }
        string DateOfExpiry { get; set; }
        string IssuingCountry { get; set; }
    }
}
