using KavaPryct.Components.Models;
using KavaPryct.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public EmpleadoRemoteService(HttpClient httpClient) => _http = httpClient;

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

        public async Task<List<EmpleadosModel>> GetAllPsicosAsync()
        {
            var whereObj = new { RolId = 1 };
            var whereStr = Uri.EscapeDataString(JsonSerializer.Serialize(whereObj));
            var url = $"classes/Empleados?where={whereStr}&limit=1000";

            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<EmpleadosModel>>(json);

            return result?.Results ?? new List<EmpleadosModel>();
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

        public async Task<WorkHours> GetWorkHoursByEmpleadoAsync(string empleadoObjectId)
        {


            // WHERE por string: { "EmpleadoObjectId": "<id>" }
            var whereObj = new { EmpleadoObjectId = empleadoObjectId };
            var whereStr = Uri.EscapeDataString(JsonSerializer.Serialize(whereObj));
            var url = $"classes/WorkRules?where={whereStr}&limit=1000";

            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var parsed = JsonSerializer.Deserialize<ParseResponse<WorkDayRuleDto>>(json);

            var rules = (parsed?.Results ?? new())
                .Select(d => new WorkDayRule
                {
                    // Mapea DayWeek (0..6) a DayOfWeek (Sunday=0)
                    Day = SafeToDayOfWeek(d.DayOfWeek),
                    WorkStart = ParseTime(d.WorkStart) ?? TimeSpan.Zero,
                    WorkEnd = ParseTime(d.WorkEnd) ?? TimeSpan.Zero,

                    // Si State == "RemoveBreak", no ponemos break
                    BreakStart = string.Equals(d.State, "RemoveBreak", StringComparison.OrdinalIgnoreCase)
                                 ? (TimeSpan?)null
                                 : ParseTime(d.BreakStart),
                    BreakEnd = string.Equals(d.State, "RemoveBreak", StringComparison.OrdinalIgnoreCase)
                                 ? (TimeSpan?)null
                                 : ParseTime(d.BreakEnd),

                    Enable = d.Enable,
                    State = d.State ?? string.Empty
                })
                .OrderBy(r => r.Day)
                .ToList();

            return new WorkHours
            {
                EmpleadoObjectId = empleadoObjectId,
                Rules = rules
            };
        }
        private static DayOfWeek SafeToDayOfWeek(int value)
        {
            // Normaliza por si te llegan 1..7 (donde 7 sería domingo)
            // Sunday=0, Monday=1, … Saturday=6
            var v = value % 7;
            if (v < 0) v += 7;
            return (DayOfWeek)v;
        }
        private static TimeSpan? ParseTime(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            // Acepta "H:mm", "HH:mm" o "HH:mm:ss"
            var formats = new[] { @"h\:mm", @"hh\:mm", @"H\:mm", @"HH\:mm", @"hh\:mm\:ss", @"HH\:mm\:ss" };
            if (TimeSpan.TryParseExact(s, formats, CultureInfo.InvariantCulture, out var ts))
                return ts;

            // Intento genérico
            if (TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out ts))
                return ts;

            return null;
        }

    }
}
