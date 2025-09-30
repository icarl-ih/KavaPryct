using KavaPryct.Components.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KavaPryct.Services
{
    public class PacienteService
    {   
        private readonly HttpClient _http;
        private readonly AppSettings _appSetting = new AppSettings();
        public PacienteService(HttpClient httpClient) => _http = httpClient;

        public async Task<List<PacienteModel>> GetAllPacientesAsync()
        {   
            var response = await _http.GetAsync("/classes/Pacientes");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<PacienteModel>>(json);

            return result?.Results ?? new List<PacienteModel>();

        }
        public async Task DeletePacienteAsync(string id)
        {   
            var response = await _http.DeleteAsync($"classes/Pacientes/{id}");
            response.EnsureSuccessStatusCode();

            

        }

        public async Task<PacienteModel> GetPacienteById(string id)
        {   
            var response = await _http.GetAsync($"classes/Pacientes/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var paciente = JsonSerializer.Deserialize<PacienteModel>(json);
            return paciente;

        }
        public async Task<PacienteModel> CreatePacienteAsync(PacienteModel c)
        {
            try
            {
                var parseobject = new Dictionary<string, object>
                {
                    {"PacienteNombre" , c.PacienteNombre.ToUpper() },
                    {"PacienteEdad" , c.PacienteEdad },
                    {"PacienteCelular" , c.PacienteCelular },
                    {"PacienteTypeId" , c.PacienteTypeId },
                    {"MotivoConsulta" , c.MotivoConsulta.ToUpper() },
                    {"PsicoObjectId" , c.PsicoObjectId },
                };

                var json = JsonSerializer.Serialize(parseobject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync("/classes/Pacientes", content);

                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Parse {((int)response.StatusCode)} {response.ReasonPhrase}: {body}"); 

                }

                response.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<PacienteModel>(body);
                return result;
                
            }
            catch (Exception ex) 
            {
                return new PacienteModel();
            }
        }
        
        public async Task UpdatePacienteAsync(PacienteModel c)
        {
            var parseobject = new Dictionary<string, object>
                {
                    {"PacienteNombre" , c.PacienteNombre.ToUpper() },
                    {"PacienteEdad" , c.PacienteEdad },
                    {"PacienteCelular" , c.PacienteCelular },
                    {"PacienteTypeId" , c.PacienteTypeId },
                    {"MotivoConsulta" , c.MotivoConsulta.ToUpper() },
                    {"PsicoObjectId" , c.PsicoObjectId },
                };


            var json = JsonSerializer.Serialize(parseobject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PutAsync(
                $"https://parseapi.back4app.com/classes/Pacientes/{c.ObjectId}",
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
        public async Task<ParseResult> CreateExpAsync(FormularioExp f)
        {
            try
            {
                var paseobject = new Dictionary<string, object>
                {
                    {"nombre",f.nombre },
                    { "sexoId",f.SexoID},
                    { "NacimientoDt",""},
                    {"lastEstudiosId",f.LastEstudiosId },
                    {"ocupacion",f.Ocupacion },
                    { "prevTerapia",f.prevTerapia},
                    {"hLive" , f.hLive },
                    { "mLive" , f.mLive},
                    {"familia","" },
                    {"domicilio",f.domicilio},
                    {"mConsulta" , f.mConsulta }
                };
                if(f.Nacimiento != null)
                {
                    paseobject["NacimientoDt"] = new Dictionary<string, object>
                    {
                        { "__type" , "Date"},
                        { "iso", f.Nacimiento.Iso.ToUniversalTime() }

                    };
                }
                var json = JsonSerializer.Serialize(paseobject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync("/classes/FormExp", content);

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
        public async Task<FormularioExp> GetFormularioById(string Id)
        {
            var response = await _http.GetAsync($"classes/FormExp/{Id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var paciente = JsonSerializer.Deserialize<FormularioExp>(json);
            return paciente;
        }
    }
}
