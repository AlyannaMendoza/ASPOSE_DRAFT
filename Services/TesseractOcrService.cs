//using Aspose.OCR;
//using DMS_DRAFT.Models;
//using PdfiumViewer;
//using System.Drawing;
//using Tesseract;

//namespace DMS_DRAFT.Services
//{
//    public interface ITesseractOcrService
//    {
//        Task<string> ConvertPdfToImageAsync(IFormFile file, string outputPath, int dpi);
//        Task<string> ExtractTextFromImageAsync(Stream imageStream);
//    }

//    public class TesseractOcrService : ITesseractOcrService
//    {
//        public async Task<string> ConvertPdfToImageAsync(IFormFile file, string outputPath, int dpi)
//        {
//            using (var document = PdfDocument.Load(file.OpenReadStream()))
//            {
//                for (int pageNumber = 0; pageNumber < document.PageCount; pageNumber++)
//                {
//                    using (var page = document.Render(pageNumber, dpi, dpi, true))
//                    {
//                        var imagePath = Path.Combine(outputPath, $"page_{pageNumber}.jpg");

//                        // Save the rendered page as a Bitmap
//                        using (var bitmap = new Bitmap(page))
//                        {
//                            // Save the Bitmap as a JPEG image
//                            bitmap.Save(imagePath, ImageFormat.Jpeg);
//                        }

//                        ResizeImage(imagePath, dpi);
//                    }
//                }
//            }
//        }

//        private void ResizeImage(string imagePath, int dpi)
//        {
//            using (var image = Image.FromFile(imagePath))
//            {
//                var resized = new Bitmap(image, new Size(image.Width / 2, image.Height / 2)); // Adjust the size as needed
//                resized.SetResolution(dpi, dpi);
//                resized.Save(imagePath, ImageFormat.Jpeg);
//            }
//        }

//        public async Task<string> ExtractTextFromImageAsync(Stream imageStream)
//        {
//            return await Task.Run(() =>
//            {
//                var pdfInput = new OcrInput(InputType.PDF);
//                pdfInput.Add(filePath);

//                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
//                {
//                    using (var img = Pix.LoadFromFile(testImagePath))
//                    {
//                        using (var page = engine.Process(img))
//                        {
//                            var text = page.GetText();
//                            Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

//                            Console.WriteLine("Text (GetText): \r\n{0}", text);
//                            Console.WriteLine("Text (iterator):");
//                        }
//                    }
//                }
//            });
//        }
//    }
//}
