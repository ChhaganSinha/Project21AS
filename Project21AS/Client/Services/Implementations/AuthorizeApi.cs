
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Project21AS.Client.Services.Contracts;
using Project21AS.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project21AS.Client.Services.Implementations
{
    public class AuthorizeApi : IAuthorizeApi
    {
        private readonly HttpClient _httpClient;

        public AuthorizeApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Login(LoginParameters loginParameters)
        {
            //var stringContent = new StringContent(JsonSerializer.Serialize(loginParameters), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/Login", loginParameters);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task Logout()
        {
            var result = await _httpClient.PostAsync("api/Authorize/Logout", null);
            result.EnsureSuccessStatusCode();
        }

        public async Task Register(RegisterParameters registerParameters)
        {
            //var stringContent = new StringContent(JsonSerializer.Serialize(registerParameters), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/Register", registerParameters);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task<UserInfo> GetUserInfo()
        {
            var result = await _httpClient.GetFromJsonAsync<UserInfo>("api/Authorize/UserInfo");
            return result;
        }

        public async Task<List<UserViewModel>> GetUsers()
        {
            var result = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Authorize/GetUsers");
            return result;
        }

        public async Task UpdateUserRole(UserViewModel userViewModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/UpdateUserRole", userViewModel);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task<int> GetUserCount()
        {
            var result = await _httpClient.GetAsync("api/Authorize/UserCount");
            result.EnsureSuccessStatusCode();

            var responseContent = await result.Content.ReadAsStringAsync();
            var countObject = JsonSerializer.Deserialize<Dictionary<string, int>>(responseContent);
            if (countObject != null && countObject.TryGetValue("count", out int count))
            {
                return count;
            }

            throw new Exception("Invalid response format");
        }

        public async Task<List<string>> GetRoles()
        {
            var result = await _httpClient.GetFromJsonAsync<List<string>>("api/Authorize/GetRoles");
            return result;
        }

        ////////
        ///
        public async Task ChangePassword(ResetPassword resetPassword)
        {
            var result = await _httpClient.PostAsJsonAsync("api/Authorize/ChangePassword", resetPassword);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await result.Content.ReadAsStringAsync());
            }

            result.EnsureSuccessStatusCode();
        }
        public async Task UpdateUserDetails(UserDetailsUpdateParameters updateParameters)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/Authorize/UpdateUserDetails", updateParameters);
                if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception(await result.Content.ReadAsStringAsync());
                }

                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception($"Failed to update user details: {ex.Message}");
            }
        }

        public async Task RequestPasswordResetByEmail(ResetPasswordByAdmin Parameters)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("api/Authorize/RequestPasswordResetByEmail", Parameters);
                if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new Exception(await result.Content.ReadAsStringAsync());
                }

                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception($"Failed to reset the password: {ex.Message}");
            }
        }
    }
}
