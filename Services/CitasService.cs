using KavaPryct.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KavaPryct.Services
{
    public class CitasService
    {   
        private readonly HttpClient _http;
        private readonly AppSettings _appSetting = new AppSettings();
        public CitasService(HttpClient httpClient) 
        { 
            _http = httpClient;
            _http.BaseAddress = new Uri(_appSetting.ParseBaseUrl);
            _http.DefaultRequestHeaders.Add("X-Parse-Application-Id", _appSetting.ApplicationId);
            _http.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _appSetting.RestApiKey);
        }

        public async Task<List<CitasModel>> GetAllCitasAsync()
        {   
            var response = await _http.GetAsync("classes/Citas");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<CitasModel>>(json);

            return result?.Results ?? new List<CitasModel>();

        }

        public async Task<CitasModel> GetCitaById(string id)
        {   
            var response = await _http.GetAsync($"classes/Citas/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var cita = JsonSerializer.Deserialize<CitasModel>(json);
            return cita;

        }
    }
}
