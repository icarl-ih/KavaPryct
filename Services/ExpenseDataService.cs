using System;
using System.Collections.Generic;
using ExpenseTracker.Models;
using KavaPryct.Services;


namespace ExpenseTracker.Service
{
    public class ExpenseDataService
    {
        private readonly TransactionService _remote;
        public ExpenseDataService(TransactionService remote) => _remote = remote;

        public event Action? OnChanged;

        public DateTime StartDate { get; private set; } = DateTime.Today.AddMonths(-1);
        public DateTime EndDate { get; private set; } = DateTime.Today;

        // Todo lo que viene de B4A en la ventana activa
        public List<ExpenseData> ExpenseData { get; private set; } = new();

        public List<CategoryData> CategoryIncomeData { get; set; } = new();
        public List<CategoryData> CategoryExpenseData { get; set; } = new();
        public string CurrentBalance { get; set; }
        public IEnumerable<ExpenseData> CurrentExpenseData { get; set; }
        public event Action OnChange;

        public void SetDate(DateTime? start, DateTime? end)
        {
            if (start.HasValue) StartDate = start.Value.Date;
            if (end.HasValue) EndDate = end.Value.Date.AddDays(1).AddTicks(-1); // fin de día
        }

        public void UpdateCurrentBalance(string currentBalance)
        {
            CurrentBalance = currentBalance;
            OnChange?.Invoke();
        }
        public async Task ReloadWindowAsync(string? empleadoId = null, int? giro = null)
        {
            ExpenseData = await _remote.GetAllTransactionsAsync();
            OnChanged?.Invoke();
        }

        // Se usa tras Create/Update/Delete para refrescar y notificar
        public async Task NotifyReloadAsync() => await ReloadWindowAsync();
        public void SetCurrentData(IEnumerable<ExpenseData> currentExpenseData)
        {
            CurrentExpenseData = currentExpenseData;
        }
        public void IniteCats()
        {
            CategoryIncomeData = new List<CategoryData>
            {
                //new CategoryData { Class = "category-icon Interests",     Category = "Interests",     Id = "Interests" },
                //new CategoryData { Class = "category-icon Business",      Category = "Business",      Id = "Business" },
                new CategoryData { Class = "category-icon Ingreso",  Category = "Ingreso",  Id = "Extra income" },
                new CategoryData { Class = "category-icon Consultas", Category = "Consultas",     Id = "Consultas" },
            };

            CategoryExpenseData = new List<CategoryData>
            {
                new CategoryData { Class = "category-icon Renta",           Category = "Renta", Id = "Rent" },
                new CategoryData { Class = "category-icon Comida",           Category = "Comida",            Id = "Food" },
                new CategoryData { Class = "category-icon Bills",          Category = "Servicios",           Id = "Bills" },
                new CategoryData { Class = "category-icon Servicios",      Category = "Reparaciones",       Id = "Utilities" },
                new CategoryData { Class = "category-icon Viaticos", Category = "Viaticos",  Id = "Transportation" },
                new CategoryData { Class = "category-icon Accidentes",      Category = "Accidentes",       Id = "Insurance" },
                new CategoryData { Class = "category-icon Insumos",       Category = "Insumos",        Id = "Shopping" },
                //new CategoryData { Class = "category-icon Entertainment",  Category = "Entertainment",   Id = "Entertainment" },
                //new CategoryData { Class = "category-icon Health Care",    Category = "Health Care",     Id = "Health Care" },
                //new CategoryData { Class = "category-icon Housing",        Category = "Housing",         Id = "Housing" },
                new CategoryData { Class = "category-icon Impuestos",          Category = "Impuestos",             Id = "Tax" },
                //new CategoryData { Class = "category-icon Clothing",       Category = "Clothing",        Id = "Clothing" },
                //new CategoryData { Class = "category-icon Education",      Category = "Education",       Id = "Education" },
                new CategoryData { Class = "category-icon Otros",  Category = "Otros",   Id = "Miscellaneous" },
                new CategoryData { Class = "category-icon Salario",         Category = "Salario",         Id = "Salary" },
            };

        }
        public ExpenseDataService()
        {
            
            ExpenseData = new List<ExpenseData>();
        }

        //// << ctor inyectado: encadena al anterior >>
        //public ExpenseDataService(TransactionService remote) : this()
        //{
        //    _remote = remote;
        //}

    }
}
