using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Repository.Helpers;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseGroupFactory
    {
        ExpenseFactory expenseFactory = new ExpenseFactory();

        public ExpenseGroupFactory()
        {

        }

        public ExpenseGroup CreateExpenseGroup(DTO.ExpenseGroup expenseGroup)
        {
            return new ExpenseGroup()
            {
                Description = expenseGroup.Description,
                ExpenseGroupStatusId = expenseGroup.ExpenseGroupStatusId,
                Id = expenseGroup.Id,
                Title = expenseGroup.Title,
                UserId = expenseGroup.UserId,
                Expenses = expenseGroup.Expenses == null ? new List<Expense>() : expenseGroup.Expenses.Select(e => expenseFactory.CreateExpense(e)).ToList()
            };
        }


        public DTO.ExpenseGroup CreateExpenseGroup(ExpenseGroup expenseGroup)
        {
            return new DTO.ExpenseGroup()
            {
                Description = expenseGroup.Description,
                ExpenseGroupStatusId = expenseGroup.ExpenseGroupStatusId,
                Id = expenseGroup.Id,
                Title = expenseGroup.Title,
                UserId = expenseGroup.UserId,
                Expenses = expenseGroup.Expenses.Select(e => expenseFactory.CreateExpense(e)).ToList()
            };
        }

        public object CreateDataShapedObject(ExpenseGroup expenseGroup, List<string> listOfFields)
        {
            return CreateDataShapedObject(CreateExpenseGroup(expenseGroup), listOfFields);
        }

        public object CreateDataShapedObject(DTO.ExpenseGroup expenseGroup, List<string> listOfFields)
        {
            var lstOfFieldsToWorkWith = new List<string>(listOfFields);
            if (!lstOfFieldsToWorkWith.Any())
            {
                return expenseGroup;
            }

            var lstOfExpenseFileds = lstOfFieldsToWorkWith.Where(x => x.Contains("expenses")).ToList();

            bool returnPartialExpense = lstOfExpenseFileds.Any() && !lstOfExpenseFileds.Contains("expenses");

            if (returnPartialExpense)
            {
                lstOfFieldsToWorkWith.RemoveRange(lstOfExpenseFileds);
                lstOfExpenseFileds = lstOfExpenseFileds.Select(x => x.Substring(x.IndexOf(".") + 1)).ToList();
            }
            else
            {
                lstOfExpenseFileds.Remove("expenses");
                lstOfFieldsToWorkWith.RemoveRange(lstOfExpenseFileds);
            }

            var objectToReturn = new ExpandoObject();

            foreach (var field in lstOfFieldsToWorkWith)
            {
                var fieldValue =
                    expenseGroup.GetType()
                        .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(expenseGroup, null);

                ((IDictionary<string, object>)objectToReturn).Add(field, fieldValue);
            }

            if (returnPartialExpense)
            {
                var expenses = expenseGroup.Expenses.Select(expense => this.expenseFactory.CreateDataShapedObject(expense, lstOfExpenseFileds)).ToList();

                ((IDictionary<string, object>)objectToReturn).Add("expenses", expenses);
            }

            return objectToReturn;
        }               
    }
}
