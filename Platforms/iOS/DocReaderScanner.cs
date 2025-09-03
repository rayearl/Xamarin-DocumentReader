using DocReaderApi.iOS;
using Foundation;
using UIKit;
#pragma warning disable CA1422

namespace DocumentReaderSample.Platforms.iOS
{
    public class DocReaderScannerEvent : EventArgs, IDocReaderScannerEvent
    {
        public string SurnameAndGivenNames { get; set; }
        public byte[] PortraitField { get; set; }
        public byte[] DocumentField { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfExpiry { get; set; }
        public string IssuingCountry { get; set; }
    }
    public class DocReaderScanner : IDocReaderScanner
    {
        public event EventHandler<IDocReaderScannerEvent> ResultsObtained;
        private bool IsReadRfid = false;
        private string selectedScenario = "Ocr";
        public void ShowScanner(bool IsReadRfid)
        {
            this.IsReadRfid = IsReadRfid;
            RGLScannerConfig config = new(selectedScenario);
            RGLDocReader.Shared.StartScannerFromPresenter(UIApplication.SharedApplication.KeyWindow.RootViewController, config, OnResultsObtained);
        }
        public void RecognizeImage(Stream stream, bool IsReadRfid)
        {
            this.IsReadRfid = IsReadRfid;
            var imageData = NSData.FromStream(stream);
            var image = UIImage.LoadFromData(imageData);
            RGLRecognizeConfig config = new(image) { Scenario = selectedScenario };
            RGLDocReader.Shared.RecognizeWithConfig(config, OnResultsObtained);
        }
        private void OnResultsObtained(RGLDocReaderAction action, RGLDocumentReaderResults result, NSError error)
        {
            if (action != RGLDocReaderAction.Complete && action != RGLDocReaderAction.ProcessTimeout) return;
            if (IsReadRfid && result != null && result.ChipPage != 0)
            {
                RGLDocReader.Shared.StartRFIDReaderFromPresenter(UIApplication.SharedApplication.KeyWindow.RootViewController, OnResultsObtained);
                IsReadRfid = false;
                return;
            }

            var portrait = result.GetGraphicFieldImageByType(RGLGraphicFieldType.Portrait);
            var rfidPortrait = result.GetGraphicFieldImageByType(RGLGraphicFieldType.Portrait, RGLResultType.RfidImageData);
            if(rfidPortrait != null) portrait = rfidPortrait;
            ResultsObtained(this, new DocReaderScannerEvent
            {
                SurnameAndGivenNames = result.GetTextFieldValueByType(RGLFieldType.Surname_And_Given_Names),
                PortraitField = ConvertImage(portrait),
                DocumentField = ConvertImage(result.GetGraphicFieldImageByType(RGLGraphicFieldType.DocumentImage)),
                DocumentType = result.GetTextFieldValueByType(RGLFieldType.Document_Class_Code) ?? "Unknown",
                DocumentNumber = result.GetTextFieldValueByType(RGLFieldType.Document_Number) ?? "Not found",
                DateOfBirth = "Not available", // Date fields may not be accessible in this iOS binding
                DateOfExpiry = "Not available", // Date fields may not be accessible in this iOS binding
                IssuingCountry = "Not available" // Country field may not be accessible in this iOS binding
            });
        }
        protected static byte[] ConvertImage(UIImage image)
        {
            if (image == null) return null;
            using NSData imageData = image.AsPNG();
            byte[] myByteArray = new byte[imageData.Length];
            System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));
            return myByteArray;
        }
        public void SelectScenario(string scenarioName) { selectedScenario = scenarioName; }
    }
}
