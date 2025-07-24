using Project21AS.DataContext;
using Project21AS.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Threading.Tasks.Dataflow;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Project21AS.Dto.Dashboard;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Project21AS.Repositories
{
    public class AppRepository : BaseRepository, IAppRepository
    {
        public AppDbContext AppDbCxt { get; set; }
        public AuthDbContext AuthDbCxt { get; set; }

        public AppRepository(ILogger<AppRepository> logger, AppDbContext appContext, AuthDbContext authDbContext) : base(logger)
        {
            AppDbCxt = appContext;
            AuthDbCxt = authDbContext;
        }

        #region Details
        public async Task<Details> GetDetailsById(int id)
        {
            Details result = null;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            result = AppDbCxt.Details.FirstOrDefault(o => o.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<Details>> GetAllDetails()
        {
            IEnumerable<Details> result = null;

            result = AppDbCxt.Details.ToList();
            return result;
        }
        public async Task<Details> UpsertDetails(Details details)
        {
            Details result = null;

            if (details == null)
                throw new ArgumentNullException("Invalid Details data");

            if (details.Id > 0)
            {


                AppDbCxt.Details.Update(details);

            }
            else
            {
                AppDbCxt.Details.Add(details);
            }

            AppDbCxt.SaveChanges();

            return details;
        }
        #endregion

        #region Student
        public async Task<Student> GetStudentById(int id)
        {
            Student result = null;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            result = AppDbCxt.Student.FirstOrDefault(o => o.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<Student>> GetAllStudent(string _userName)
        {
            IEnumerable<Student> result = null;
            if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                result = AppDbCxt.Student.Where(x => x.Admin == _userName).ToList();
            }
            else
            {
                result = AppDbCxt.Student.ToList();
            }
            return result;
        }
        public async Task<ApiResponse<Student>> UpsertStudent(Student Student)
        {
            var result = new ApiResponse<Student>();
            try
            {
                if (Student == null)
                    throw new ArgumentNullException("Invalid Student data");
                int StudentCountForBatch = await AppDbCxt.Student.Where(o=>o.Batch == Student.Batch && o.Admin == Student.Admin).CountAsync();
                if (StudentCountForBatch >= Student.MaxStudentPerBatch)
                {
                    result.IsSuccess = false;
                    result.Message = $"You can add only {Student.MaxStudentPerBatch} students per batch. For further assistance, please contact your administrator.";
                    return result;
                }
                if (Student.Id > 0)
                {
                    AppDbCxt.Student.Update(Student);
                }
                else
                {
                    AppDbCxt.Student.Add(Student);
                }

                AppDbCxt.SaveChanges();
                result.Result = Student;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ApiResponse<StudentBatchTransfer>> StudentBatchTransfer(StudentBatchTransfer Student)
        {
            var result = new ApiResponse<StudentBatchTransfer>();
            try
            {
                var data =  AppDbCxt.Student.Where(o=>o.Id == Student.Id).FirstOrDefault();
                if(data != null)
                {
                    string batch = Student.NewBatch.Split("+")[0];
                    string admin = Student.NewBatch.Split("+")[1];
                    data.Batch = batch;
                    data.Admin = admin;
                    data.ModifiedBy = "Admin";
                    data.ModifiedOn = DateTime.Now;
                    AppDbCxt.Student.Update(data);
                    AppDbCxt.SaveChanges();
                    result.Result = Student;
                    result.Message = $"Student Transferred to <strong>{Student.NewBatch}</strong> batch.";
                    result.IsSuccess = true;
                    return result;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Student Not Found in Database!";
                    return result;
                }
                
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }


        public async Task<(bool, string)> DeleteStudentById(int id,string path)
        {
            Student result = await AppDbCxt.Student.FindAsync(id);

            if (result != null)
            {
                try
                {
                    
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Retrieve the filenames associated with the student
                    var studentFingerMapping = await AppDbCxt.StudentFingerPrintMapping
                        .Where(x => x.Id == result.Id)
                        .FirstOrDefaultAsync();

                    if (studentFingerMapping != null)
                    {
                        // Construct the filenames list
                        var filenames = new List<string>
                        {
                            studentFingerMapping.FingerPrint1,
                            studentFingerMapping.FingerPrint2,
                            studentFingerMapping.FingerPrint3,
                            studentFingerMapping.FingerPrint4,
                            studentFingerMapping.FingerPrint5,
                            studentFingerMapping.FingerPrint6,
                            studentFingerMapping.FingerPrint7,
                            studentFingerMapping.FingerPrint8,
                            studentFingerMapping.FingerPrint9,
                            studentFingerMapping.FingerPrint10
                        };

                        // Delete files with filenames associated with the student in StudentZone directory
                        foreach (var filename in filenames)
                        {
                            if (!string.IsNullOrEmpty(filename))
                            {
                                var filePath = Path.Combine(path, filename);
                                var filePathPng = filePath.Replace(".txt", ".png");
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                    File.Delete(filePathPng);
                                }
                            }
                        }
                    }

                    // Delete the student from the database
                    AppDbCxt.Student.Remove(result);
                    if(studentFingerMapping != null)
                        AppDbCxt.StudentFingerPrintMapping.Remove(studentFingerMapping);

                    await AppDbCxt.SaveChangesAsync();

                    return (true, "Data deleted successfully");
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }
            }

            return (false, "Student not found.");
        }


        public async Task<ApiResponse<StudentFingerPrintMapping>> UpsertStudentFingerPrint(StudentFingerPrintMapping data)
        {
            var result = new ApiResponse<StudentFingerPrintMapping>();

            var existingData = await AppDbCxt.StudentFingerPrintMapping.FirstOrDefaultAsync(e => e.Id == data.Id);

            if (existingData != null)
            {
                if (!string.IsNullOrEmpty(data.FingerPrint1))
                {
                    existingData.FingerPrint1 = data.FingerPrint1;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint2))
                {
                    existingData.FingerPrint2 = data.FingerPrint2;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint3))
                {
                    existingData.FingerPrint3 = data.FingerPrint3;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint4))
                {
                    existingData.FingerPrint4 = data.FingerPrint4;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint5))
                {
                    existingData.FingerPrint5 = data.FingerPrint5;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint6))
                {
                    existingData.FingerPrint6 = data.FingerPrint6;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint7))
                {
                    existingData.FingerPrint7 = data.FingerPrint7;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint8))
                {
                    existingData.FingerPrint8 = data.FingerPrint8;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint9))
                {
                    existingData.FingerPrint9 = data.FingerPrint9;
                }
                if (!string.IsNullOrEmpty(data.FingerPrint10))
                {
                    existingData.FingerPrint10 = data.FingerPrint10;
                }
                // Update the database
                AppDbCxt.StudentFingerPrintMapping.Update(existingData);
            }
            else
            {
                // Add new data to the database
                AppDbCxt.StudentFingerPrintMapping.Add(data);
            }

            await AppDbCxt.SaveChangesAsync();
            return result;
        }

        public async Task<StudentFingerPrintMapping> GetStudentFingerPrintsById(int id)
        {
            StudentFingerPrintMapping result = null;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            result = AppDbCxt.StudentFingerPrintMapping.FirstOrDefault(o => o.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            return result;
        }

        public async Task<IEnumerable<StudentFingerPrintMapping>> GetAllStudentFingerPrintMapping(string _userName)
        {
            IEnumerable<StudentFingerPrintMapping> result = null;
            if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                result = AppDbCxt.StudentFingerPrintMapping.Where(x => x.Admin == _userName).ToList();
            }
            else
            {
                result = AppDbCxt.StudentFingerPrintMapping.ToList();
            }
            return result;
        }

        public async Task<IEnumerable<Student>> GetStudentsByBatchName(Batch batch)
        {
            IEnumerable<Student> result = new List<Student>();

            result = AppDbCxt.Student.Where(x => x.Admin == batch.Admin && x.Batch == batch.Name).ToList();
      
            return result;
        }
        #endregion

        #region Batch
        public async Task<Batch> GetBatchById(int id)
        {
            Batch result = null;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            result = AppDbCxt.Batch.FirstOrDefault(o => o.Id == id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<Batch>> GetAllBatch(string _userName)
        {
            IEnumerable<Batch> result = null;
            if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                result = AppDbCxt.Batch.Where(x => x.Admin == _userName).ToList();
            }
            else
            {
                result = AppDbCxt.Batch.ToList();
            }
            return result;
        }
        public async Task<ApiResponse<Batch>> UpsertBatch(Batch batch)
        {
            var result = new ApiResponse<Batch>();
            try
            {
                if (batch == null)
                    throw new ArgumentNullException(nameof(batch), "Invalid batch data");

                // Check if the number of batches for the admin exceeds the allowed limit
                int batchCount = await AppDbCxt.Batch.CountAsync(o => o.Admin == batch.Admin);
                if (batchCount >= batch.AllowedBatches)
                {
                    result.IsSuccess = false;
                    result.Message = $"You can create only {batch.AllowedBatches} batches. For further assistance, please contact your administrator.";
                    return result;
                }

                if (batch.Id > 0)
                {
                    // Detach existing Batch entity if it's being tracked
                    var existingBatch = await AppDbCxt.Batch.FindAsync(batch.Id);
                    if (existingBatch != null)
                    {
                        AppDbCxt.Entry(existingBatch).State = EntityState.Detached;
                    }

                    // Update students' batch names if the batch name has changed
                    if (existingBatch != null && existingBatch.Name != batch.Name)
                    {
                        var studentsToUpdate = await AppDbCxt.Student.Where(x => x.Batch == existingBatch.Name && x.Admin == existingBatch.Admin).ToListAsync();
                        foreach (var student in studentsToUpdate)
                        {
                            student.Batch = batch.Name;                           
                            student.ModifiedOn = DateTime.Now;
                            AppDbCxt.Student.Update(student);
                        }
                    }
                }

                if (batch.Id > 0)
                {
                    // Update existing batch
                    AppDbCxt.Batch.Update(batch);
                }
                else
                {
                    // Add new batch
                    AppDbCxt.Batch.Add(batch);
                }

                await AppDbCxt.SaveChangesAsync();
                result.Result = batch;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                return result;
            }
        }



        public async Task<(bool, string)> DeleteBatchById(int id,string path)
        {
            Batch result = await AppDbCxt.Batch.FindAsync(id);

            if (result != null)
            {
                try
                {
                    // Get all students associated with the batch
                    var students = await AppDbCxt.Student
                        .Where(x => x.Admin == result.Admin && x.Batch == result.Name)
                        .ToListAsync();

                    // Loop through each student to delete them and their associated files
                    foreach (var student in students)
                    {
                        // Retrieve the filenames associated with the student
                        var studentFingerMapping = await AppDbCxt.StudentFingerPrintMapping
                            .Where(x => x.Id == student.Id)
                            .FirstOrDefaultAsync();

                        if (studentFingerMapping != null)
                        {
                            // Construct the filenames list
                            var filenames = new List<string>
                            {
                                studentFingerMapping.FingerPrint1,
                                studentFingerMapping.FingerPrint2,
                                studentFingerMapping.FingerPrint3,
                                studentFingerMapping.FingerPrint4,
                                studentFingerMapping.FingerPrint5,
                                studentFingerMapping.FingerPrint6,
                                studentFingerMapping.FingerPrint7,
                                studentFingerMapping.FingerPrint8,
                                studentFingerMapping.FingerPrint9,
                                studentFingerMapping.FingerPrint10
                            };

                            // Delete files with filenames associated with the student

                            foreach (var filename in filenames)
                            {
                                if (!string.IsNullOrEmpty(filename))
                                {
                                    var filePath = Path.Combine(path, filename);
                                    var filePathPng = filePath.Replace(".txt", ".png");
                                    if (File.Exists(filePath))
                                    {
                                        File.Delete(filePath);
                                        File.Delete(filePathPng);
                                    }
                                }
                            }
                        }

                        // Delete the StudentFingerPrintMapping from the database
                        AppDbCxt.StudentFingerPrintMapping.Remove(studentFingerMapping);
                        // Delete the student from the database
                        AppDbCxt.Student.Remove(student);
                    }

                    // Delete the batch from the database
                    AppDbCxt.Batch.Remove(result);

                    // Save changes to the database
                    await AppDbCxt.SaveChangesAsync();

                    return (true, "Data deleted successfully");
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }
            }

            return (false, "Batch not found.");
        }

        #endregion


        #region Dashboard
        public async Task<Dashboard> GetDashboardStatistics(string _userName)
        {
            Dashboard dashboard = new Dashboard();

            if (!string.Equals(_userName, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                dashboard.TotalBatches = await AppDbCxt.Batch.Where(x => x.Admin == _userName).CountAsync();
                dashboard.TotalStudent = await AppDbCxt.Student.Where(x => x.Admin == _userName).CountAsync();
                dashboard.TotalUsers = await AuthDbCxt.Users.CountAsync();

                var allowed = await AuthDbCxt.Users
                    .Where(u => u.UserName == _userName)
                    .Select(u => u.AllowedBatches)
                    .FirstOrDefaultAsync();
                dashboard.RemainingBatches = Math.Max(0, allowed - dashboard.TotalBatches);

                dashboard.StudentsByBatch = await AppDbCxt.Student
                    .Where(x => x.Admin == _userName)
                    .GroupBy(x => x.Batch)
                    .Select(g => new BatchStudentCount
                    {
                        BatchName = g.Key,
                        StudentCount = g.Count()
                    }).ToListAsync();
            }
            else
            {
                dashboard.TotalBatches = await AppDbCxt.Batch.CountAsync();
                dashboard.TotalStudent = await AppDbCxt.Student.CountAsync();
                dashboard.TotalUsers = await AuthDbCxt.Users.CountAsync();

                dashboard.RemainingBatches = 0;

                dashboard.StudentsByBatch = await AppDbCxt.Student
                    .GroupBy(x => x.Batch)
                    .Select(g => new BatchStudentCount
                    {
                        BatchName = g.Key,
                        StudentCount = g.Count()
                    }).ToListAsync();
            }

            return dashboard;
        }
        #endregion
    }
}
