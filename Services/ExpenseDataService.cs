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

        public UserInfo UserInfo { get; set; }
        public List<CategoryData> CategoryIncomeData { get; set; }
        public List<CategoryData> CategoryExpenseData { get; set; }
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

        public ExpenseDataService()
        {
            //StartDate = new DateTime(2019, 06, 01, 00, 00, 00);
            //EndDate = new DateTime(2019, 11, 30, 23, 59, 59);
            CurrentBalance = "$0";

            UserInfo = new UserInfo { FirstName = "Nicholas", FullName = "Nicholas Delacruz", Email = "nicholas@gmail.com" };

            CategoryIncomeData = new List<CategoryData>();
            CategoryIncomeData.AddRange(new List<CategoryData>
            {
                new CategoryData { Class = "category-icon Interests", Category = "Interests", Id = "Interests" },
                new CategoryData { Class = "category-icon Business", Category = "Business", Id = "Business" },
                new CategoryData { Class = "category-icon Extra income", Category = "Extra income", Id = "Extra income" },
                                new CategoryData { Class = "category-icon Personal Care", Category = "Consultas", Id = "Consultas" },

            });

            CategoryExpenseData = new List<CategoryData>();
            CategoryExpenseData.AddRange(new List<CategoryData>
            {
                new CategoryData { Class = "category-icon Rent", Category = "Mortgage / Rent", Id = "Mortgage / Rent" },
                new CategoryData { Class = "category-icon Food", Category = "Food", Id = "Food" },
                new CategoryData { Class = "category-icon Bills", Category = "Bills", Id = "Bills" },
                new CategoryData { Class = "category-icon Utilities", Category = "Utilities", Id = "Utilities" },
                new CategoryData { Class = "category-icon Transportation", Category = "Transportation", Id = "Transportation" },
                new CategoryData { Class = "category-icon Insurance", Category = "Insurance", Id = "Insurance" },
                new CategoryData { Class = "category-icon Shopping", Category = "Shopping", Id = "Shopping" },
                new CategoryData { Class = "category-icon Entertainment", Category = "Entertainment", Id = "Entertainment" },
                new CategoryData { Class = "category-icon Health Care", Category = "Health Care", Id = "Health Care" },
                new CategoryData { Class = "category-icon Housing", Category = "Housing", Id = "Housing" },
                new CategoryData { Class = "category-icon Taxes", Category = "Tax", Id = "Tax" },
                new CategoryData { Class = "category-icon Clothing", Category = "Clothing", Id = "Clothing" },
                new CategoryData { Class = "category-icon Education", Category = "Education", Id = "Education" },
                new CategoryData { Class = "category-icon Miscellaneous", Category = "Miscellaneous", Id = "Miscellaneous" },
                                new CategoryData { Class = "category-icon Salary", Category = "Salario", Id = "Salary" },

            });

            ExpenseData = new List<ExpenseData>();


        }
    }
}
