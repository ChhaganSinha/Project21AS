using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project21AS.Dto;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

[ApiController]
[Route("[controller]")]
public class FilesaveController : ControllerBase
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<FilesaveController> logger;

    public FilesaveController(IWebHostEnvironment env, ILogger<FilesaveController> logger)
    {
        this.env = env;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files)
    {
        var maxAllowedFiles = 3;
        long maxFileSize = 1024 * 2048; // 2Mb
        var filesProcessed = 0;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        List<UploadResult> uploadResults = new();

        foreach (var file in files)
        {
            var uploadResult = new UploadResult();
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

            if (filesProcessed < maxAllowedFiles)
            {
                if (file.Length == 0)
                {
                    logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
                    uploadResult.ErrorCode = 1;
                }
                else if (file.Length > maxFileSize)
                {
                    logger.LogInformation("{FileName} of {Length} bytes is larger than the limit of {Limit} bytes (Err: 2)", trustedFileNameForDisplay, file.Length, maxFileSize);
                    uploadResult.ErrorCode = 2;
                }
                else
                {
                    try
                    {
                        string path;
#if DEBUG
                        path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Project21AS\\Client\\wwwroot\\StudentZone";
#else
                        path = Path.Combine(env.ContentRootPath, "wwwroot", "StudentZone");
#endif

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        var paths = Path.Combine(path, untrustedFileName);
                        // check for similar type file
                        //check from start with
                        //var parts = untrustedFileName.Split("-");
                        //var commonPrefix = string.Join("-", parts.Take(3));

                        //checking from end with to ensure delete file in student name updated as well
                        var untrustedFileNameParts = untrustedFileName.Split("-");
                        var commonSuffix = string.Join("-", untrustedFileNameParts.Skip(1));

                        string[] folderFiles = Directory.GetFiles(path);

                        foreach (string f in folderFiles)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(f);
                            //if (fileName.StartsWith(commonPrefix))
                            //{
                            //    System.IO.File.Delete(f);
                            //}
                            if (fileName.EndsWith(commonSuffix.Split('.')[0]))
                            {
                                System.IO.File.Delete(f);
                            }
                        }

                        try
                        {
                            using (var stream = new FileStream(paths, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);

                                // Reset the stream position to the beginning
                                stream.Seek(0, SeekOrigin.Begin);

                                // Read the base64 content from the stream
                                string base64Content = new StreamReader(stream).ReadToEnd();

                                // Convert Base64 string to ImageSharp Image
                                using (var memoryStream = new MemoryStream(Convert.FromBase64String(base64Content)))
                                using (Image<Rgba32> image = Image.Load<Rgba32>(memoryStream))
                                {
                                    // Negate the colors (create a negative effect)
                                    image.Mutate(x => x.Invert());

                                    // Set the background color to black
                                    image.Mutate(x => x.BackgroundColor(Color.Black));
                                    //image.Mutate(x => x.Contrast(1.5f));

                                    // Flip the image horizontally
                                    image.Mutate(x => x.Flip(FlipMode.Horizontal));

                                    // Get the save image path
                                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(untrustedFileName);
                                    string pngFilePath = Path.Combine(path, $"{fileNameWithoutExtension}.png");

                                    // Save ImageSharp Image as PNG
                                    image.Save(pngFilePath, new PngEncoder());

                                    logger.LogInformation("Files saved: {PNGFilePath}, {TXTFilePath}", pngFilePath, untrustedFileName);

                                    uploadResult.Uploaded = true;
                                    uploadResult.StoredFileName = untrustedFileName;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.LogError("{FileName} error on upload (Err: 3): {Message}", trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                        }
                    }
                    catch (IOException ex)
                    {
                        logger.LogError("{FileName} error on upload (Err: 3): {Message}", trustedFileNameForDisplay, ex.Message);
                        uploadResult.ErrorCode = 3;
                    }
                }

                filesProcessed++;
            }
            else
            {
                logger.LogInformation("{FileName} not uploaded because the request exceeded the allowed {Count} of files (Err: 4)", trustedFileNameForDisplay, maxAllowedFiles);
                uploadResult.ErrorCode = 4;
            }

            uploadResults.Add(uploadResult);
        }

        return new CreatedResult(resourcePath, uploadResults);
    }

    // Method to convert base64 string to Bitmap
    //public static Bitmap Base64StringToBitmap(string base64String)
    //{
    //    Bitmap bmpReturn = null;
    //    // Convert Base64 string to byte[]
    //    byte[] byteBuffer = Convert.FromBase64String(base64String);
    //    MemoryStream memoryStream = new MemoryStream(byteBuffer);

    //    memoryStream.Position = 0;

    //    bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

    //    memoryStream.Close();
    //    memoryStream = null;
    //    byteBuffer = null;

    //    return bmpReturn;
    //}
}
