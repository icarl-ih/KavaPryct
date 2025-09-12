using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KavaPryct.Components.Models
{
    // Reglas base (no guardan fecha, solo día de semana y horas del día)
    public class WorkDayRule
    {
        public DayOfWeek Day { get; set; }                // Lunes, Martes, etc.
        public TimeSpan WorkStart { get; set; }           // p.ej. 08:00
        public TimeSpan WorkEnd { get; set; }             // p.ej. 21:00
        public TimeSpan? BreakStart { get; set; }         // p.ej. 14:00
        public TimeSpan? BreakEnd { get; set; }           // p.ej. 15:00
        public bool Enable { get; set; } = true;          // si labora ese día
        public string State { get; set; } = "";           // "RemoveBreak" si no hay break
    }

    // Horario de un empleado (colección de reglas)
    public class WorkHours
    {
        public string EmpleadoObjectId { get; set; } = default!;
        public List<WorkDayRule> Rules { get; set; } = new();
    }

    // Versión “anclada” a una semana concreta (con DateTime completos)
    public class AnchoredWorkDay
    {
        public DayOfWeek Day { get; set; }
        public DateTime WorkStart { get; set; }
        public DateTime WorkEnd { get; set; }
        public DateTime? BreakStart { get; set; }
        public DateTime? BreakEnd { get; set; }
        public bool Enable { get; set; }
    }

    // Helpers para “anclar” a la semana visible
    public static class WorkHoursExtensions
    {
        public static IEnumerable<AnchoredWorkDay> AnchorToWeek(this WorkHours workHours, DateTime weekStart)
        {
            if (workHours?.Rules == null) yield break;

            foreach (var r in workHours.Rules)
            {
                // Calcula la fecha exacta del r.Day dentro de la semana que inicia en weekStart
                int delta = ((int)r.Day - (int)weekStart.DayOfWeek + 7) % 7;
                DateTime date = weekStart.Date.AddDays(delta);

                yield return new AnchoredWorkDay
                {
                    Day = r.Day,
                    WorkStart = date.Date + r.WorkStart,
                    WorkEnd = date.Date + r.WorkEnd,
                    BreakStart = r.BreakStart.HasValue ? date.Date + r.BreakStart.Value : (DateTime?)null,
                    BreakEnd = r.BreakEnd.HasValue ? date.Date + r.BreakEnd.Value : (DateTime?)null,
                    Enable = r.Enable
                };
            }
        }
    }

    public class WorkDayRuleDto
    {
        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }

        // Pointer al empleado (si tu esquema lo usa)
        [JsonPropertyName("EmpleadoObjectId")]
        public string EmpleadoId { get; set; }

        [JsonPropertyName("DayWeek")]
        public int DayOfWeek { get; set; } // 0..6 (Sunday..Saturday)

        [JsonPropertyName("WorkStart")]
        public string WorkStart { get; set; }   // "08:00"

        [JsonPropertyName("WorkEnd")]
        public string WorkEnd { get; set; }     // "21:00"

        [JsonPropertyName("BreakStart")]
        public string BreakStart { get; set; }  // "14:00" (opcional)

        [JsonPropertyName("BreakEnd")]
        public string BreakEnd { get; set; }    // "15:00" (opcional)

        [JsonPropertyName("Enable")]
        public bool Enable { get; set; }

        [JsonPropertyName("State")]
        public string State { get; set; }       // p.ej. "RemoveBreak"

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

}
