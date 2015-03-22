using ExpenseTracker.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class ExpenseTrackerEFRepository : ExpenseTracker.Repository.IExpenseTrackerRepository
    {

        // TODO: in a later stage, everything must be user-specific, eg: the
        // userid must always be passed in!  Don't do this in the first stage,
        // this allows us to show what can go wrong if you don't include the
        // user check.

        ExpenseTrackerContext _ctx;

        public ExpenseTrackerEFRepository(ExpenseTrackerContext ctx)
        {
            _ctx = ctx;
            
            // Disable lazy loading - if not, related properties are auto-loaded when
            // they are accessed for the first time, which means they'll be included when
            // we serialize (b/c the serialization process accesses those properties).  
            // 
            // We don't want that, so we turn it off.  We want to eagerly load them (using Include)
            // manually.

            _ctx.Configuration.LazyLoadingEnabled = false;

        }

        public Expense GetExpense(int id, int? expenseGroupId = null)
        {
            return _ctx.Expenses.FirstOrDefault(e => e.Id == id && 
                (expenseGroupId == null || expenseGroupId == e.ExpenseGroupId));
        }


        public IQueryable<Expense> GetExpenses(int expenseGroupId)
        {
            // if the expense group exists, we return the expenses for that group.
            // if it doesn't exist, we return null, so we can throw a not found exception

            var correctGroup = _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == expenseGroupId);
            if (correctGroup != null)
            {
                return _ctx.Expenses.Where(e => e.ExpenseGroupId == expenseGroupId);
            }
            else
            {
                return null;
            }
            
           
        }


        public IQueryable<Expense> GetExpenses()
        {
            return _ctx.Expenses;
        }
                
        public ExpenseGroup GetExpenseGroup(int id)
        {
           return _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == id);
        }



        public ExpenseGroup GetExpenseGroup(int id, string userId)
        {
            return _ctx.ExpenseGroups.FirstOrDefault(eg => eg.Id == id && eg.UserId == userId);
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id, string userId)
        {
            return _ctx.ExpenseGroups.Include("Expenses").FirstOrDefault(eg => eg.Id == id && eg.UserId == userId);
        }

        public ExpenseGroup GetExpenseGroupWithExpenses(int id)
        {
            return _ctx.ExpenseGroups.Include("Expenses").FirstOrDefault(eg => eg.Id == id);
        }
        
        public IQueryable<ExpenseGroup> GetExpenseGroups()
        {
            return _ctx.ExpenseGroups;
        }


        public IQueryable<ExpenseGroup> GetExpenseGroupsWithExpenses()
        {
            return _ctx.ExpenseGroups.Include("Expenses");
        }


        public IQueryable<ExpenseGroup> GetExpenseGroups(string userId)
        {
            return _ctx.ExpenseGroups.Where(eg => eg.UserId == userId);
        }

        public ExpenseGroupStatus GetExpenseGroupStatus(int id)
        {
            return _ctx.ExpenseGroupStatusses.FirstOrDefault(egs => egs.Id == id);
        }

        public IQueryable<ExpenseGroupStatus> GetExpenseGroupStatusses()
        {
            return _ctx.ExpenseGroupStatusses;
        }



        /// <summary>
        /// Return bool if ok, the newly-created ExpenseGroup is still available (including
        /// the id) to the calling class.
        /// </summary>
        /// <param name="eg"></param>
        /// <returns></returns>
        public RepositoryActionResult<ExpenseGroup> InsertExpenseGroup(ExpenseGroup eg)
        {
            try
            {
                _ctx.ExpenseGroups.Add(eg);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.Created);
                }
                else
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NothingModified, null);
                }
            }
           catch (Exception ex)
           {
               return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
           }
        }


        public RepositoryActionResult<Expense> InsertExpense(Expense e)
        {
            try
            {
                _ctx.Expenses.Add(e);
                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.Created);
                }
                else
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NothingModified, null);
                }

            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }


        public RepositoryActionResult<ExpenseGroup> UpdateExpenseGroup(ExpenseGroup eg)
        {
            try
            {

                // you can only update when an expensegroup already exists for this id

                var existingEG = _ctx.ExpenseGroups.FirstOrDefault(exg => exg.Id == eg.Id);

                if (existingEG == null)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NotFound);
                }

                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                _ctx.Entry(existingEG).State = EntityState.Detached;
                    
                // attach & save
                _ctx.ExpenseGroups.Attach(eg);

                // set the updated entity state to modified, so it gets updated.
                _ctx.Entry(eg).State = EntityState.Modified;
                   

                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new RepositoryActionResult<ExpenseGroup>(eg, RepositoryActionStatus.NothingModified, null);
                }
            }
           catch (Exception ex)
           {
               return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
           }
 
        }

        public  RepositoryActionResult<Expense> UpdateExpense(Expense e)
        {
            try
            {
                // you can only update when an expense already exists for this id

                var existingExpense = _ctx.Expenses.FirstOrDefault(exp => exp.Id == e.Id);

                if (existingExpense == null)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NotFound);
                }

                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                _ctx.Entry(existingExpense).State = EntityState.Detached;

                // attach & save
                _ctx.Expenses.Attach(e);

                // set the updated entity state to modified, so it gets updated.
                _ctx.Entry(e).State = EntityState.Modified;

                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.Updated);
                }
                else
                {
                    return new RepositoryActionResult<Expense>(e, RepositoryActionStatus.NothingModified, null);
                }
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
           
        }

        public RepositoryActionResult<Expense> DeleteExpense(int id)
        {
            try
            {
                var exp = _ctx.Expenses.Where(e => e.Id == id).FirstOrDefault();
                if (exp != null)
                {
                    _ctx.Expenses.Remove(exp);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Expense>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<ExpenseGroup> DeleteExpenseGroup(int id)
        {
            try
            {
            
                var eg = _ctx.ExpenseGroups.Where(e => e.Id == id).FirstOrDefault();
                if (eg != null)
                {
                    // also remove all expenses linked to this expensegroup
                    
                    _ctx.ExpenseGroups.Remove(eg);
                    
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Deleted);
                }
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<ExpenseGroup>(null, RepositoryActionStatus.Error, ex);
            }
        }
    }
}
