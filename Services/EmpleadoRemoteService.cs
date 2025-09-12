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
        private readonly AppSettings _appSetting = new AppSettings();
        public EmpleadoRemoteService(HttpClient httpClient)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri(_appSetting.ParseBaseUrl);
            _http.DefaultRequestHeaders.Add("X-Parse-Application-Id", _appSetting.ApplicationId);
            _http.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", _appSetting.RestApiKey);

        }

        public async Task<List<EmpleadosModel>> GetAllEmpleadosAsync()
        {
            var response = await _http.GetAsync("classes/Empleados");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<EmpleadosModel>>(json);

            return result?.Results ?? new List<EmpleadosModel>();
        }

        public async Task<EmpleadosModel> GetEmpleadoByIdAsync(string id)
        {
            var response = await _http.GetAsync($"classes/Empleados/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var empleado = JsonSerializer.Deserialize<EmpleadosModel>(json);
            return empleado;
        }

        public async Task<EstudiosModel> GetEstudiosByIdAsync(string id)
        {
            var response = await _http.GetAsync($"classes/Estudios/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var estudios = JsonSerializer.Deserialize<EstudiosModel>(json);
            return estudios;
        }

        public async Task<List<EstudiosModel>> GetEstudiosAllAsync() {             
            var response = await _http.GetAsync("classes/Estudios");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<EstudiosModel>>(json);
            return result?.Results ?? new List<EstudiosModel>();
        }
        public async Task<List<PosgradosModel>> GetPosgradosByEstudioAsync(string estudioObjectId)
        {
            // where = { "$relatedTo": { "key": "Posgrados", "object": { "__type": "Pointer", "className": "Estudios", "objectId": "<ID>" } } }
            var whereObj = new Dictionary<string, object>
            {
                ["$relatedTo"] = new Dictionary<string, object>
                {
                    ["key"] = "Posgrados",
                    ["object"] = new Dictionary<string, object>
                    {
                        ["__type"] = "Pointer",
                        ["className"] = "Estudios",
                        ["objectId"] = estudioObjectId
                    }
                }
            };

            var whereStr = Uri.EscapeDataString(JsonSerializer.Serialize(whereObj));
            // Ajusta limit si esperas más de 100 (límite por defecto)
            var url = $"classes/Posgrados?where={whereStr}&limit=1000";

            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<ParseResponse<PosgradosModel>>(json);
            return parsed?.Results ?? new List<PosgradosModel>();
        }


        public async Task<List<EstudiosModel>> GetEstudiosWithPosgradosAsync()
        {
            // 1) Trae todos los estudios (igual que tu método actual)
            var estudios = await GetEstudiosAllAsync();

            // 2) Para cada Estudio, trae sus Posgrados relacionados
            var tareas = estudios.Select(async e =>
            {
                try
                {
                    e.Posgrados = await GetPosgradosByEstudioAsync(e.ObjectId);
                }
                catch
                {
                    // Evita null refs si algo falla
                    e.Posgrados = new List<PosgradosModel>();
                }
            });

            await Task.WhenAll(tareas);
            return estudios;
        }

    }
}
