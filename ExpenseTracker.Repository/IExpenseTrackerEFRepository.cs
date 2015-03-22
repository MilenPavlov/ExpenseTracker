using System;
namespace ExpenseTracker.Repository
{
    public interface IExpenseTrackerRepository
    {
        RepositoryActionResult<ExpenseTracker.Repository.Entities.Expense> DeleteExpense(int id);
        RepositoryActionResult<ExpenseTracker.Repository.Entities.ExpenseGroup> DeleteExpenseGroup(int id);
        ExpenseTracker.Repository.Entities.Expense GetExpense(int id, int? expenseGroupId = null);
        ExpenseTracker.Repository.Entities.ExpenseGroup GetExpenseGroup(int id);
        ExpenseTracker.Repository.Entities.ExpenseGroup GetExpenseGroup(int id, string userId);
        System.Linq.IQueryable<ExpenseTracker.Repository.Entities.ExpenseGroup> GetExpenseGroups();
        System.Linq.IQueryable<ExpenseTracker.Repository.Entities.ExpenseGroup> GetExpenseGroups(string userId);
        ExpenseTracker.Repository.Entities.ExpenseGroupStatus GetExpenseGroupStatus(int id);
        System.Linq.IQueryable<ExpenseTracker.Repository.Entities.ExpenseGroupStatus> GetExpenseGroupStatusses();
        System.Linq.IQueryable<ExpenseTracker.Repository.Entities.ExpenseGroup> GetExpenseGroupsWithExpenses();
        ExpenseTracker.Repository.Entities.ExpenseGroup GetExpenseGroupWithExpenses(int id);
        ExpenseTracker.Repository.Entities.ExpenseGroup GetExpenseGroupWithExpenses(int id, string userId);
        System.Linq.IQueryable<ExpenseTracker.Repository.Entities.Expense> GetExpenses();
        System.Linq.IQueryable<ExpenseTracker.Repository.Entities.Expense> GetExpenses(int expenseGroupId);
    
        RepositoryActionResult<ExpenseTracker.Repository.Entities.Expense> InsertExpense(ExpenseTracker.Repository.Entities.Expense e);
        RepositoryActionResult<ExpenseTracker.Repository.Entities.ExpenseGroup> InsertExpenseGroup(ExpenseTracker.Repository.Entities.ExpenseGroup eg);
        RepositoryActionResult<ExpenseTracker.Repository.Entities.Expense> UpdateExpense(ExpenseTracker.Repository.Entities.Expense e);
        RepositoryActionResult<ExpenseTracker.Repository.Entities.ExpenseGroup> UpdateExpenseGroup(ExpenseTracker.Repository.Entities.ExpenseGroup eg);
    }
}
