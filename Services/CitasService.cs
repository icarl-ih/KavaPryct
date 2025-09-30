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
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public CitasService(HttpClient httpClient) => _http = httpClient;

        public async Task<List<CitasModel>> GetAllCitasAsync(CancellationToken ct = default)
        {
            var all = new List<CitasModel>();
            const int pageSize = 1000;
            int skip = 0;

            while (true)
            {
                var url = $"/classes/Citas?limit={pageSize}&skip={skip}&order=createdAt";

                using var res = await _http.GetAsync(url, ct);
                res.EnsureSuccessStatusCode();

                await using var s = await res.Content.ReadAsStreamAsync(ct);
                var page = await JsonSerializer.DeserializeAsync<ParseResponse<CitasModel>>(s, _jsonOpts, ct);
                var batch = page?.Results ?? [];

                all.AddRange(batch);

                if (batch.Count < pageSize) break; // última página
                skip += pageSize;
            }

            return all;

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
        public async Task<ParseResult> CreateCitaAsync(CitasModel c)
        {
            try
            {
                var parseobject = new Dictionary<string, object>
                {
                    {"PacienteNombre" , c.PacienteNombre.ToUpper()  },
                    {"PacienteEdad" , c.PacienteEdad },
                    {"PacienteEdad1" , c.PacienteEdad1 },
                    {"PacienteDomicilio" , c.PacienteDomicilio },
                    {"EstudiosId" , c.EstudiosLast },
                    {"EstudiosId1" , c.EstudiosLast1 },
                    {"PacienteCelular" , c.PacienteCelular },
                    {"PacienteCelular1" , c.PacienteCelular1 },
                    {"PacienteTypeId" , c.PacienteTypeId },
                    {"MotivoConsulta" , c.MotivoConsulta.ToUpper()  },
                    {"StatusCitaId" , c.StatusCitaId },   // 1
                    {"PsicoObjectId" , c.PsicoObjectId },
                    {"FechaIni", "" },{"FechaFin", "" }, {"PacienteObjectId",c.PacienteObjectId},
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

                bool status;
                var body = await response.Content.ReadAsStringAsync();
                var reason = response.ReasonPhrase ?? response.StatusCode.ToString();

                if (response.IsSuccessStatusCode) // típicamente 201 Created
                {
                    var created = JsonSerializer.Deserialize<ParseCreateResponse>(body,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    status = true;
                    return new ParseResult
                    {
                        Ok = status,
                        Reason = reason,
                        ObjectId = created?.ObjectId
                    };
                }
                else
                {
                    status = false;
                }

                // Error: devolvemos reason + cuerpo
                return new ParseResult
                {
                    Ok = status,
                    Reason = reason,
                    ErrorBody = body
                };
            }
            catch (Exception ex)
            {
                return new ParseResult
                {
                    Ok = false,
                    Reason = "Exception",
                    ErrorBody = ex.Message
                };
            }
        }

        public async Task UpdateCitaAsync(CitasModel c)
        {
            var parseobject = new Dictionary<string, object>
            {
                {"PacienteNombre" , c.PacienteNombre.ToUpper() },
                {"PacienteEdad" , c.PacienteEdad },
                    {"PacienteEdad1" , c.PacienteEdad1 },
                    {"PacienteDomicilio" , c.PacienteDomicilio },
                    {"EstudiosId" , c.EstudiosLast },
                    {"EstudiosId1" , c.EstudiosLast1 },
                    {"PacienteCelular" , c.PacienteCelular },
                    {"PacienteCelular1" , c.PacienteCelular1 },
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
        public async Task UpdateCitaReagendadaAsync(CitasModel c)
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
                
                {"Amount",c.Amount },{"IsPaid",c.IsPaid},{ "PayMethodId" ,c.PayMethodId},
                
                {"PacienteObjectId",c.PacienteObjectId }

            };
            

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
