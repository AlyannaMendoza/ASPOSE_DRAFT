using DMS_DRAFT.Data;
using DMS_DRAFT.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.OCRProcessor;
using Syncfusion.Pdf.Parsing;
using System.Drawing;
using Tesseract;

namespace DMS_DRAFT.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FilesController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var files = await _context.FilesMetadata.ToListAsync();
            return View(files);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(nameof(file), "File is empty");
            }

            var allowedContentTypes = new List<string> { "application/pdf", "image/jpeg" };

            if (!allowedContentTypes.Contains(file.ContentType))
            {
                throw new InvalidOperationException("Unsupported file type. Only PDF and PNG files are allowed.");
            }

            var uploadDate = DateTime.Now;
            var monthFolder = $"{uploadDate.ToString("MMMM")} {uploadDate.Year.ToString()}";
            var dayFolder = $"{uploadDate.ToString("MMMM")} {uploadDate.Day} {uploadDate.Year.ToString()}";

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Documents Uploaded",
                uploadDate.Year.ToString(),
                monthFolder,
                dayFolder
             );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var uniqueFileName = $"{timestamp}_{{Path.GetFileNameWithoutExtension(file.FileName)}}";
            var filePath = Path.Combine(folderPath, uniqueFileName);
            //var filePath = Path.Combine(folderPath, file.FileName);

            //if (file.ContentType == "application/pdf")
            //{
            // Convert PDF to JPG and resize
            // await ConvertPdfToJpg(file, filePath, 150);
            //}

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileMetadata = new FileMetadata
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                FilePath = filePath,
                UploadDate = uploadDate,
                //UploaderId = uploaderId
            };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var fileData = new FileData
                {
                    Data = memoryStream.ToArray(),
                    FileMetadata = fileMetadata
                };

                _context.FilesData.Add(fileData);
                _context.FilesMetadata.Add(fileMetadata);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var file = await _context.FilesMetadata
                .Include(f => f.FileData)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }

        [HttpGet]
        public async Task<IActionResult> GetFile(int id)
        {
            var fileData = await _context.FilesData
                .Include(fd => fd.FileMetadata)
                .FirstOrDefaultAsync(fd => fd.FileMetadataId == id);

            if (fileData == null)
            {
                return NotFound();
            }

            var contentType = fileData.FileMetadata.ContentType;
            var fileName = fileData.FileMetadata.FileName;

            Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            return File(fileData.Data, contentType);
        }

        //Syncfusion OCR
        [HttpGet]
        public async Task<IActionResult> PerformOcr(int id)
        {
            var fileData = await _context.FilesData
                .Include(fd => fd.FileMetadata)
                .FirstOrDefaultAsync(fd => fd.FileMetadataId == id);

            if (fileData == null)
            {
                return NotFound();
            }

            string extractedText = string.Empty;

            try
            {
                //    if (fileData.FileMetadata.ContentType == "application/pdf")
                //    {
                //        // Initialize the OCR processor and perform OCR
                //        using (OCRProcessor processor = new OCRProcessor())
                //        {
                //            using (MemoryStream pdfStream = new MemoryStream(fileData.Data))
                //            {
                //                PdfLoadedDocument loadedDoc = new PdfLoadedDocument(pdfStream);
                //                processor.Settings.Language = Languages.English;
                //                extractedText = processor.PerformOCR(loadedDoc);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        extractedText = "This file is not a PDF.";
                //    }
                //}
                //catch (Exception ex)
                //{
                //    return StatusCode(500, $"Internal server error: {ex.Message}");
                //}
                using (MemoryStream pdfStream = new MemoryStream(fileData.Data))
                {
                    // Ensure stream is not null or empty
                    if (pdfStream == null || pdfStream.Length == 0)
                    {
                        throw new Exception("PDF stream is null or empty.");
                    }

                    using (OCRProcessor processor = new OCRProcessor())
                    {
                        PdfLoadedDocument lDoc = new PdfLoadedDocument(pdfStream);

                        if (lDoc == null)
                        {
                            throw new Exception("Failed to load PDF document.");
                        }

                        // Perform OCR on the loaded PDF document
                        extractedText = processor.PerformOCR(lDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it as necessary
                Console.WriteLine("Error during OCR: " + ex.Message);
                // Optionally, return an error view or message
                return View("Error", new { message = "OCR processing failed: " + ex.Message });
            }

            // Pass the FileMetadata object to the view again for display
            var fileMetadata = _context.FilesMetadata
                .FirstOrDefault(f => f.Id == id);

            // Set the OCR result in ViewBag to display it in the view
            ViewBag.OcrResult = extractedText;

            return View("Details", fileData.FileMetadata);
        }
    }
}
