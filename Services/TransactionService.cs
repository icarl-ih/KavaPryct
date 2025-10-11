using ExpenseTracker.Models;
using KavaPryct.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KavaPryct.Services
{
    public class TransactionService
    {
        private readonly HttpClient _http;
        private readonly AppSettings _appSetting = new AppSettings();
        public TransactionService(HttpClient httpClient) => _http = httpClient;
        
        

        public async Task DeleteTransactionAsync(string id)
        {
            var response = await _http.DeleteAsync($"classes/Balance/{id}");
            response.EnsureSuccessStatusCode();
        }
        public async Task<ExpenseData> GetMovById(string Id)
        {
            var response = await _http.GetAsync("/classes/Balance");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExpenseData>(json);

            return result;

        }
        public async Task CreateMovementeAsync(ExpenseData data)
        {
            try
            {
                var parseobject = new Dictionary<string, object>
                {
                    {"TransactionTypeId",data.TransactionTypeId },
                    {"Amount",data.Amount},
                    {"DatFecha",""},
                    {"Category",data.Category},
                    {"Description",data.Description},
                    {"PayMethodId",data.PayMethodId},
                    {"PaymentMode",data.PaymentMode},
                    {"EmployeeObjectId",data.EmployeeObjectId},
                    {"EmployeeName",data.EmployeeName},{"citaObjectId",data.citaObjectId}
                };

                if(data.DatFecha != null)
                {
                    parseobject["DatFecha"] = new Dictionary<string, object>
                    {
                        { "__type" , "Date"},
                        { "iso", data.DatFecha.Iso.ToUniversalTime() }
                    };
                }
                var json = JsonSerializer.Serialize(parseobject);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _http.PostAsync("/classes/Balance", content);

                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Parse {((int)response.StatusCode)} {response.ReasonPhrase}: {body}");
                response.EnsureSuccessStatusCode();
            }catch(Exception ex)
            {

            }
        }
        public async Task UpdateTransactionAsync (ExpenseData data)
        {
            var parseobject = new Dictionary<string, object>
            {
                {"TransactionTypeId",data.TransactionTypeId },
                {"Amount",data.Amount},
                {"DatFecha",""},
                {"Category",data.Category},
                {"Description",data.Description},
                {"PayMethodId",data.PayMethodId},
                {"PaymentMode",data.PaymentMode},
                {"EmployeeObjectId",data.EmployeeObjectId},
                {"EmployeeName",data.EmployeeName}
            };
            if (data.DatFecha != null)
            {
                parseobject["DatFecha"] = new Dictionary<string, object>
                    {
                        { "__type" , "Date"},
                        { "iso", data.DatFecha.Iso.ToUniversalTime() }
                    };
            }
            var json = JsonSerializer.Serialize(parseobject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PutAsync(
                $"https://parseapi.back4app.com/classes/Balance/{data.ObjectId}",
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

        public async Task<List<ExpenseData>> GetAllTransactionsAsync()
        {
            var response = await _http.GetAsync("/classes/Balance?limit=1000");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ParseResponse<ExpenseData>>(json);

            return result?.Results ?? new List<ExpenseData>();
        }








    }
}
