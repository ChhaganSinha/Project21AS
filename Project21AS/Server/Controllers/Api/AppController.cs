
using AsseProject21AStManagement.Server.Controllers.Api;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Project21AS.DataContext.Models;
using Project21AS.Dto;
using Project21AS.Dto.Dashboard;
using Project21AS.Repositories;
using System.Net.Http;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;



namespace Project21AS.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppController : ControllerBase
    {
        readonly AppRepository _appRepository;
        readonly ILogger _logger;
        readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AppController(ILogger<AppController> logger, IConfiguration appConfig, IAppRepository appRepository, IWebHostEnvironment env) : base()
        {
            _appRepository = (AppRepository?)appRepository;
            _logger = logger;
            _configuration = appConfig;
            _env = env;
            // Retrieve username and roles in the constructor and store them in _userName and _userRoles
           
        }

        #region Details
        [HttpGet]
        [Route("details/{id}")]
        public async Task<Details> GetDetailsById(int id)
        {
            return await _appRepository.GetDetailsById(id);
        }

        [HttpGet]
        [Route("all-details")]
        [AllowAnonymous]
        public async Task<IEnumerable<Details>> GetAllDetails()
        {
            return await _appRepository.GetAllDetails();
        }

        [HttpPost]
        [Route("UpsertDetails")]
        public async Task<Details> UpsertDetails(Details details)
        {

            return await _appRepository.UpsertDetails(details);
        }
        #endregion

        #region Student
        [HttpGet]
        [Route("Student/{id}")]
        public async Task<Student> GetStudentById(int id)
        {
            return await _appRepository.GetStudentById(id);
        }

        [HttpGet]
        [Route("all-Student")]
        public async Task<IEnumerable<Student>> GetAllStudent()
        {
            string _userName = User.Identity?.Name;
            return await _appRepository.GetAllStudent( _userName);
        }

        [HttpPost]
        [Route("UpsertStudent")]
        public async Task<ApiResponse<Student>> UpsertStudent(Student Student)
        {
            string _userName = User.Identity?.Name;

            //if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            //    Student.Admin = _userName;
            //else if (string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase) && Student.Id == 0)
            //    Student.Admin = _userName;
            if(Student.Id == 0)
                Student.Admin = _userName;

            return await _appRepository.UpsertStudent(Student);
        }
        
        [HttpPost]
        [Route("StudentBatchTransfer")]
        public async Task<ApiResponse<StudentBatchTransfer>> StudentBatchTransfer(StudentBatchTransfer Student)
        {
            return await _appRepository.StudentBatchTransfer(Student);
        }

        [HttpDelete]
        [Route("DeleteStudent/{id}")]
        public async Task<(bool, string)> DeleteStudentById(int id)
        {
            var path = string.Empty;
#if DEBUG
            path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Project21AS\\Client\\wwwroot\\StudentZone";
#else
            path = Path.Combine(_env.ContentRootPath, "wwwroot", "StudentZone");
#endif
            return await _appRepository.DeleteStudentById(id, path);
        }

        [HttpPost]
        [Route("UpsertStudentFingerPrint")]
        public async Task<ApiResponse<StudentFingerPrintMapping>> UpsertStudentFingerPrint(StudentFingerPrintMapping data)
        {
            var result = new ApiResponse<StudentFingerPrintMapping>();

            try
            {
                string _userName = User.Identity?.Name;
                if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
                    data.Admin = _userName;
                else if (string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase) && data.Id == 0)
                    data.Admin = _userName;

                result = await _appRepository.UpsertStudentFingerPrint(data);
                if (!result.IsSuccess)
                {
                    result.Message = result.Message;
                }
                else
                {
                    result.Result = data;
                    result.IsSuccess = true;
                    result.Message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }

            return result;
        }

        [HttpGet]
        [Route("StudentFingerPrintsById/{id}")]
        public async Task<StudentFingerPrintMapping> GetStudentFingerPrintsById(int id)
        {
            return await _appRepository.GetStudentFingerPrintsById(id);
        }

        [HttpGet]
        [Route("all-StudentFingerPrintMapping")]
        public async Task<IEnumerable<StudentFingerPrintMapping>> GetAllStudentFingerPrintMapping()
        {
            string _userName = User.Identity?.Name;
            return await _appRepository.GetAllStudentFingerPrintMapping( _userName);
        }
        //get batchwise student list
        [HttpGet]
        [Route("getStudentsByBatchName")]
        public async Task<IEnumerable<Student>> GetStudentsByBatchName(Batch batch)
        {
            return await _appRepository.GetStudentsByBatchName(batch);
        }


        [HttpPost]
        [Route("printPDF")]
        public async Task<IActionResult> PrintPDF(Batch batch) // Make the method asynchronous
        {
            try
            {
                //var BaseUri = new Uri($"{Request.Scheme}://{Request.Host}/");
                var BaseUri = _configuration["BaseUrl"];

                var studentListTask = GetStudentsByBatchName(batch);
                var fingerPrintMappingTask = GetAllStudentFingerPrintMapping();

                // Await the tasks to get the actual data
                var studentList = await studentListTask;
                var fingerPrintMapping = await fingerPrintMappingTask;

                // Create a temporary file path
                var filePath = Path.GetTempFileName();

                // Create a PdfWriter object
                using (var writer = new PdfWriter(filePath))
                {
                    // Create a PdfDocument object
                    using (var pdf = new PdfDocument(writer))
                    {
                        // Create a Document object
                        using (var document = new Document(pdf))
                        {
                            // Add header with admin name, batch name, and today's date
                            Paragraph header = new Paragraph()
                                .SetTextAlignment(TextAlignment.CENTER)
                                .Add($"Batch Admin: {batch.Admin}  |  Batch: {batch.Name}  |  Date: {DateTime.Today.ToShortDateString()}");
                            header.SetFontColor(ColorConstants.BLUE);
                            document.Add(header);

                            // Create a table with 9 columns
                            Table table = new Table(9);

                            // Set font for table cells
                            float fontSize = 8;


                            // Add table header
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Name").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Mobile").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Batch").SetFontSize(8)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Address").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Finger1").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Finger2").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Finger3").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Finger4").SetFontSize(fontSize)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Finger5").SetFontSize(fontSize)));

                            // Add table data
                            foreach (var student in studentList)
                            {
                                table.AddCell(new Cell().Add(new Paragraph(student.Name).SetFontSize(fontSize)));
                                table.AddCell(new Cell().Add(new Paragraph(student.Mobile).SetFontSize(fontSize)));
                                table.AddCell(new Cell().Add(new Paragraph(student.Batch ?? "").SetFontSize(fontSize))); // Handle null Batch
                                table.AddCell(new Cell().Add(new Paragraph(student.Address).SetFontSize(fontSize)));

                                // Add finger image cells
                                var studentFingerPrints = fingerPrintMapping.FirstOrDefault(o => o.Id == student.Id);
                                if (studentFingerPrints != null)
                                {
                                    // Iterate through each finger print
                                    for (int i = 1; i <= 5; i++)
                                    {

                                        string fingerPrintFileName = $"FingerPrint{i}";
                                        string fingerPrintFilePath = $"{BaseUri}StudentZone/{studentFingerPrints.GetType().GetProperty(fingerPrintFileName)?.GetValue(studentFingerPrints)}";

                                        // Check if the file exists
                                        if (fingerPrintFilePath.EndsWith(".txt"))
                                        {
                                            ImageData imageData = ImageDataFactory.Create(fingerPrintFilePath.Replace(".txt", ".png"));
                                            Image image = new Image(imageData);
                                            // Set image width and height
                                            image.SetWidth(40f); // Set width to 50 points (1 point = 1/72 inch)
                                            image.SetHeight(40f); // Set height to 50 points

                                            table.AddCell(new Cell().Add(image));
                                        }
                                        else
                                        {
                                            // Add an empty cell if the file doesn't exist
                                            table.AddCell(new Cell().Add(new Paragraph("").SetFontSize(fontSize)));
                                        }
                                    }
                                }
                                else
                                {
                                    // Add empty cells if no finger prints are found
                                    for (int i = 0; i < 5; i++)
                                    {
                                        table.AddCell(new Cell().Add(new Paragraph("").SetFontSize(fontSize)));
                                    }
                                }
                            }

                            // Add the table to the document
                            document.Add(table);
                        }
                    }
                }

                // Return the PDF file for download
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", "example.pdf");
            }
            catch (Exception ex)
            {
                // Handle any errors
                return StatusCode(500, "Internal server error");
            }
        }


        #endregion

        #region Batch
        [HttpGet]
        [Route("Batch/{id}")]
        public async Task<Batch> GetBatchById(int id)
        {
            
            return await _appRepository.GetBatchById(id);
        }

        [HttpGet]
        [Route("all-Batch")]
        public async Task<IEnumerable<Batch>> GetAllBatch()
        {
            string _userName = User.Identity?.Name;
            return await _appRepository.GetAllBatch( _userName);
        }

        [HttpPost]
        [Route("UpsertBatch")]
        public async Task<ApiResponse<Batch>> UpsertBatch(Batch Batch)
        {
            string _userName = User.Identity?.Name;
            //if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            //    Batch.Admin = _userName;
            //else if (string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase) && Batch.Id == 0)
            //    Batch.Admin = _userName;

            if (Batch.Id == 0)
                Batch.Admin = _userName;


            return await _appRepository.UpsertBatch(Batch);
        }

        [HttpDelete]
        [Route("DeleteBatch/{id}")]
        public async Task<(bool, string)> DeleteBatchById(int id)
        {
            var path = string.Empty;
#if DEBUG
            path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Project21AS\\Client\\wwwroot\\StudentZone";
#else
                    path = Path.Combine(_env.ContentRootPath, "wwwroot", "StudentZone");
#endif
            return await _appRepository.DeleteBatchById(id, path);
        }
        #endregion

        #region Dashboard
        
        [HttpGet]
        [Route("get-DashboardStatistics")]
        public async Task<Dashboard> GetDashboardStatistics()
        {
            string _userName = User.Identity?.Name;
            return await _appRepository.GetDashboardStatistics( _userName);
        }

        #endregion
    }
}
