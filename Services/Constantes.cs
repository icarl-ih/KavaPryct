using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KavaPryct.Services
{
    public enum Giro
    {
        Ingreso = 1,
        Egreso = 2
    }

    public enum MetodosPago
    {
        Efectivo = 1,
        Transferencia = 2,
        Tarjeta = 3,
        Otro = 0
    }

    public enum RolEmpleo
    {
        Terapeuta = 1,
        Administrativo = 2,
        Empleado = 3
    }

    public enum UltimoGradoEstudios
    {
        SinEstudios = 0,
        Secundaria = 1,
        Preparatoria = 2,
        Licenciatura = 3,
        Posgrado = 4
    }

    public enum Especialidad
    {
        Especialidad = 0,
        Maestria = 1,
        Doctorado = 2
    }

    public enum TipoPaciente
    {
        Individual = 1,
        Pareja = 2,
        Familia = 3,
        Infante = 4,
        Adolescente = 5

    }
    public enum EstatusCita
    {
        Cancelada = 0,
        Asignada = 1,
        ReAgendada = 201,
        Confirmada = 100,
        Asistencia = 10,
        Inasistencia = 11

    }

    public enum EstadoCivil
    {
        Otro = 0,
        Soltería = 1,
        Matrmonio = 2,
        Viudez = 3,
        Divorcio = 4,
        Concubinato = 5
    }
}
