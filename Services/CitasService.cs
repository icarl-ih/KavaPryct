using KavaPryct.Components.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
        public CitasService(HttpClient httpClient) => _http = httpClient;

        public async Task<List<CitasModel>> GetAllCitasAsync()
        {   
            var response = await _http.GetAsync("/classes/Citas");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<CitasModel>>(json);

            return result?.Results ?? new List<CitasModel>();

        }
        public async Task DeleteCitaAsync(string id)
        {   
            var response = await _http.DeleteAsync($"classes/Citas/{id}");
            response.EnsureSuccessStatusCode();

            

        }

        public async Task<CitasModel> GetCitaById(string id)
        {   
            var response = await _http.GetAsync($"classes/Citas/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var cita = JsonSerializer.Deserialize<CitasModel>(json);
            return cita;

        }
        public async Task CreateCitaAsync(CitasModel c)
        {
            try
            {
                var parseobject = new Dictionary<string, object>
                {
                    {"PacienteNombre" , c.PacienteNombre.ToUpper()  },
                    {"PacienteEdad" , c.PacienteEdad },
                    {"PacienteCelular" , c.PacienteCelular },
                    {"PacienteTypeId" , c.PacienteTypeId },
                    {"MotivoConsulta" , c.MotivoConsulta.ToUpper()  },
                    {"StatusCitaId" , c.StatusCitaId },   // 1
                    {"PsicoObjectId" , c.PsicoObjectId },
                    {"FechaIni", "" },{"FechaFin", "" }, {"PacienteObjecId",c.PacienteObjectId},
                    {"Amount",c.Amount },{"IsPaid",c.IsPaid},{ "PayMethodId" ,c.PayMethodId},
                };
                if (c.FechaIni != null && c.FechaFin != null)
                {
                    parseobject["FechaIni"] = new Dictionary<string, object>
                    {
                        { "__type" , "Date"},
                        { "iso", c.FechaIni.Iso.ToUniversalTime() }
                    };
                    parseobject["FechaFin"] = new Dictionary<string, object>
                    {
                        { "__type" , "Date"},
                        { "iso", c.FechaFin.Iso.ToUniversalTime() }
                    };
                }

                var json = JsonSerializer.Serialize(parseobject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync("/classes/Citas", content);

                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Parse {((int)response.StatusCode)} {response.ReasonPhrase}: {body}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) 
            {
                
            }
        }
        
        public async Task UpdateCitaAsync(CitasModel c)
        {
            var parseobject = new Dictionary<string, object>
            {
                {"PacienteNombre" , c.PacienteNombre.ToUpper() },
                {"PacienteEdad" , c.PacienteEdad },
                {"PacienteCelular" , c.PacienteCelular },
                {"PacienteTypeId" , c.PacienteTypeId },
                {"MotivoConsulta" , c.MotivoConsulta.ToUpper() },
                {"StatusCitaId" , c.StatusCitaId },   // 1
                {"PsicoObjectId" , c.PsicoObjectId },
                {"FechaIni", "" },{"FechaFin", "" },
                {"Amount",c.Amount },{"IsPaid",c.IsPaid},{ "PayMethodId" ,c.PayMethodId},
                
                {"PacienteObjectId",c.PacienteObjectId }

            };
            if(c.FechaIni != null && c.FechaFin != null)
            {
                parseobject["FechaIni"] = new Dictionary<string, object>
                {
                    { "__type" , "Date"},
                    { "iso", c.FechaIni.Iso.ToUniversalTime() }
                };
                parseobject["FechaFin"]= new Dictionary<string, object>
                {
                    { "__type" , "Date"},
                    { "iso", c.FechaFin.Iso.ToUniversalTime() }
                };
            }

            var json = JsonSerializer.Serialize(parseobject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PutAsync(
                $"https://parseapi.back4app.com/classes/Citas/{c.ObjectId}",
                content);

            if (response.IsSuccessStatusCode)
            {
                // ¡Éxito!
                var respuestaString = await response.Content.ReadAsStringAsync();
                // Opcional: puedes extraer el objeto actualizado aquí
            }
            else
            {
                // Manejo de error
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error: {response.StatusCode} - {errorContent}");
            }
        }

    }
}
