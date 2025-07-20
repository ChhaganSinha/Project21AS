
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Project21AS.Dto;
using Project21AS.Client.Client;
using Project21AS.Dto.Dashboard;
using Project21AS.Dto.Auth;
using Microsoft.JSInterop;

namespace Project21AS.Client.Client
{
    public class AppClient : BaseHttpClient
    {
        public AppClient(ILogger<AppClient> logger, HttpClient httpClient) : base(logger, httpClient)
        {

        }

        #region Details
        public async Task<Details> GetDetailsById(int id)
        {
            Details details = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/details/{id}");

                res.EnsureSuccessStatusCode();

                details = await res.Content.ReadFromJsonAsync<Details>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return details;
        }
        public async Task<IEnumerable<Details>> GetAllDetails()
        {
            IEnumerable<Details> details = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/all-details");

                res.EnsureSuccessStatusCode();

                details = await res.Content.ReadFromJsonAsync<IEnumerable<Details>>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return details;
        }

        public async Task<Details> UpsertDetailsAsync(Details details)
        {

            Details result = null;

            try
            {
                var res = await HttpClient.PostAsJsonAsync($"api/App/UpsertDetails", details);

                res.EnsureSuccessStatusCode();

                result = await res.Content.ReadFromJsonAsync<Details>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return result;

        }
        #endregion

        #region Student
        public async Task<Student> GetStudentById(int id)
        {
            Student Student = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/Student/{id}");

                res.EnsureSuccessStatusCode();

                Student = await res.Content.ReadFromJsonAsync<Student>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return Student;
        }
        public async Task<IEnumerable<Student>> GetAllStudent()
        {
            IEnumerable<Student> Student = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/all-Student");

                res.EnsureSuccessStatusCode();

                Student = await res.Content.ReadFromJsonAsync<IEnumerable<Student>>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return Student;
        }

        //public async Task<ApiResponse<Student>> UpsertStudentAsync(Student Student)
        public async Task<ApiResponse<Student>> UpsertStudentAsync(Student Student)
        {

            try
            {
                var userInfo = await HttpClient.GetFromJsonAsync<UserInfo>("api/Authorize/UserInfo");
                if(userInfo != null)
                {
                    Student.MaxStudentPerBatch = userInfo.MaxStudentPerBatch;
                }
                var res = await HttpClient.PostAsJsonAsync($"api/App/UpsertStudent", Student);
                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadFromJsonAsync<ApiResponse<Student>>();
                return json;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }
        }

        public async Task<ApiResponse<StudentBatchTransfer>> StudentBatchTransfer(StudentBatchTransfer Student)
        {

            try
            {
                var res = await HttpClient.PostAsJsonAsync($"api/App/StudentBatchTransfer", Student);
                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadFromJsonAsync<ApiResponse<StudentBatchTransfer>>();
                return json;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }


        }
        public async Task<(bool, string)> DeleteStudentById(int id)
        {
            try
            {
                var res = await HttpClient.DeleteAsync($"api/App/DeleteStudent/{id}");

                res.EnsureSuccessStatusCode();

                var result = await res.Content.ReadAsStringAsync();

                var jsonDocument = JsonDocument.Parse(result);

                // Access the root element
                var rootElement = jsonDocument.RootElement;

                // Access the values of item1 and item2
                bool item1 = rootElement.GetProperty("item1").GetBoolean();
                string item2 = rootElement.GetProperty("item2").GetString();

                return (item1, item2);

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }
        }

        public async Task<ApiResponse<StudentFingerPrintMapping>> UpsertStudentFingerPrint(StudentFingerPrintMapping data)
        {
            try
            {
                var res = await HttpClient.PostAsJsonAsync($"api/App/UpsertStudentFingerPrint", data);

                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadFromJsonAsync<ApiResponse<StudentFingerPrintMapping>>();
                return json;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

        }

        public async Task<StudentFingerPrintMapping> GetStudentFingerPrintsById(int id)
        {
            StudentFingerPrintMapping result = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/StudentFingerPrintsById/{id}");
                res.EnsureSuccessStatusCode();

                if (res.Content is not null)
                {
                    string responseContent = await res.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        result = await res.Content.ReadFromJsonAsync<StudentFingerPrintMapping>();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return result;
        }

        public async Task<IEnumerable<StudentFingerPrintMapping>> GetAllStudentFingerPrintMapping()
        {
            IEnumerable<StudentFingerPrintMapping> result = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/all-StudentFingerPrintMapping");

                res.EnsureSuccessStatusCode();

                result = await res.Content.ReadFromJsonAsync<IEnumerable<StudentFingerPrintMapping>>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return result;
        }

        public async Task<string> PrintPDF(Batch batch)
        {
            try
            {
                HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/App/printPDF",batch);
                response.EnsureSuccessStatusCode(); // Ensure successful response

                // Read the PDF content as a byte array
                byte[] pdfContent = await response.Content.ReadAsByteArrayAsync();

                // Convert the byte array to base64 string
                string base64String = Convert.ToBase64String(pdfContent);

                return base64String;
            }
            catch (HttpRequestException ex)
            {
                // Handle any HTTP request errors
                Console.WriteLine($"HTTP request error: {ex.Message}");
                Console.WriteLine($"HTTP request InnerException: {ex.InnerException}");
                return null;
            }
        }



        #endregion

        #region Batch
        public async Task<Batch> GetBatchById(int id)
        {
            Batch Batch = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/Batch/{id}");

                res.EnsureSuccessStatusCode();

                Batch = await res.Content.ReadFromJsonAsync<Batch>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return Batch;
        }
        public async Task<IEnumerable<Batch>> GetAllBatch()
        {
            IEnumerable<Batch> Student = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/all-Batch");

                res.EnsureSuccessStatusCode();

                Student = await res.Content.ReadFromJsonAsync<IEnumerable<Batch>>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return Student;
        }

        public async Task<ApiResponse<Batch>> UpsertBatchAsync(Batch Batch)
        {
            try
            {
                var userInfo = await HttpClient.GetFromJsonAsync<UserInfo>("api/Authorize/UserInfo");
                if (userInfo != null)
                {
                    Batch.AllowedBatches = userInfo.AllowedBatches;
                }
                var res = await HttpClient.PostAsJsonAsync($"api/App/UpsertBatch", Batch);

                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadFromJsonAsync<ApiResponse<Batch>>();
                return json;

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

        }
        public async Task<(bool, string)> DeleteBatchById(int id)
        {
            try
            {
                var res = await HttpClient.DeleteAsync($"api/App/DeleteBatch/{id}");

                res.EnsureSuccessStatusCode();

                var result = await res.Content.ReadAsStringAsync();

                var jsonDocument = JsonDocument.Parse(result);

                // Access the root element
                var rootElement = jsonDocument.RootElement;

                // Access the values of item1 and item2
                bool item1 = rootElement.GetProperty("item1").GetBoolean();
                string item2 = rootElement.GetProperty("item2").GetString();

                return (item1, item2);

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }
        }

        #endregion

        #region Dashboard
        public async Task<Dashboard> GetDashboardStatistics()
        {
            Dashboard Dashboard = null;
            try
            {
                var res = await HttpClient.GetAsync($"api/App/get-DashboardStatistics");

                res.EnsureSuccessStatusCode();

                Dashboard = await res.Content.ReadFromJsonAsync<Dashboard>();

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                throw;
            }

            return Dashboard;
        }

        
        #endregion
    }
}
