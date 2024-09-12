using Aspose.OCR;
using DMS_DRAFT.Data;

namespace DMS_DRAFT.Services
{
    public interface IAsposeOcrService
    {
        Task<string> ExtractTextFromPdfAsync(string filePath);
    }

    public class AsposeOcrService : IAsposeOcrService
    {
        private readonly ApplicationDbContext _context;

        public AsposeOcrService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> ExtractTextFromPdfAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                // Create OcrInput for PDF
                var pdfInput = new OcrInput(InputType.PDF);
                pdfInput.Add(filePath);

                // Initialize the OCR engine
                var ocrEngine = new AsposeOcr();

                // Recognize the text from the document
                var extractedTexts = ocrEngine.Recognize(pdfInput);

                var ocrResult = string.Join("\n", extractedTexts.Select(et => et.RecognitionText));
                return ocrResult;
            });
        }
    }
}
