using KavaPryct.Components.Models;
using KavaPryct.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KavaPryct.Services
{
   public class EmpleadoRemoteService
    {
        private readonly HttpClient _http;
        private readonly AppSettings _appSetting;
        public EmpleadoRemoteService(HttpClient httpClient)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri(_appSetting.ParseBaseUrl);
            _http.DefaultRequestHeaders.Add("X-Parse-Application-Id", _appSetting.ApplicationId);
            _http.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _appSetting.RestApiKey);

        }

        public async Task<List<EmpleadosModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("classes/Empleados");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<EmpleadosModel>>(json);

            return result?.Results ?? new List<EmpleadosModel>();
        }
    }
}
