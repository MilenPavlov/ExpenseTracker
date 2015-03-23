using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;
using Marvin.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace ExpenseTracker.API.Controllers
{
    using System.Web.Http.Routing;

    using ExpenseTracker.API.Helpers;

    using Newtonsoft.Json;

    [RoutePrefix("api")]
    public class ExpensesController : ApiController
    {
        private const int MaxPageSize = 10;
        IExpenseTrackerRepository _repository;
        ExpenseFactory _expenseFactory = new ExpenseFactory();

        public ExpensesController()
        {
            _repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }

        public ExpensesController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }
         

     

        [Route("expenses/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {

                var result = _repository.DeleteExpense(id);

                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("expenses")]
        public IHttpActionResult Post([FromBody]DTO.Expense expense)
        {
            try
            {
                if (expense == null)
                {
                    return BadRequest();
                }

                // map
                var exp = _expenseFactory.CreateExpense(expense);

                var result = _repository.InsertExpense(exp);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    // map to dto
                    var newExp = _expenseFactory.CreateExpense(result.Entity);
                    return Created<DTO.Expense>(Request.RequestUri + "/" + newExp.Id.ToString(), newExp);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [Route("expenses/{id}")]
        public IHttpActionResult Put(int id, [FromBody]DTO.Expense expense)
        {
            try
            {
                if (expense == null)
                {
                    return BadRequest();
                }

                // map
                var exp = _expenseFactory.CreateExpense(expense);

                var result = _repository.UpdateExpense(exp);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedExpense = _expenseFactory.CreateExpense(result.Entity);
                    return Ok(updatedExpense);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [Route("expenses/{id}")]
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody]JsonPatchDocument<DTO.Expense> expensePatchDocument)
        {
            try
            {
                // find 
                if (expensePatchDocument == null)
                {
                    return BadRequest(); 
                }

                var expense = _repository.GetExpense(id);
                if (expense == null)
                {
                    return NotFound();
                }

                //// map
                var exp = _expenseFactory.CreateExpense(expense);

                // apply changes to the DTO
                expensePatchDocument.ApplyTo(exp);

                // map the DTO with applied changes to the entity, & update
                var result = _repository.UpdateExpense(_expenseFactory.CreateExpense(exp));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedExpense = _expenseFactory.CreateExpense(result.Entity);
                    return Ok(updatedExpense);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("expensegroups/{expenseGroupId}/expenses", Name = "ExpensesForGroup")]
        public IHttpActionResult Get(int expenseGroupId, string sort = "date", string fields = null, int page = 1, int pageSize = MaxPageSize)
        {
            try
            {
                var listOfFields = new List<string>();

                if (fields != null)
                {
                    listOfFields = fields.ToLower().Split(',').ToList();
                }

                var expenses = _repository.GetExpenses(expenseGroupId);

                if (expenses == null)
                {
                    return this.NotFound();
                }

                if (pageSize < MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }

                var totalCount = expenses.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var urlHelper = new UrlHelper(Request);

                var prevLink = page > 1
                                   ? urlHelper.Link(
                                       "ExpensesForGroup",
                                       new
                                           {
                                               page = page - 1,
                                               pageSize = pageSize,
                                               expenseGroupId = expenseGroupId,
                                               sort = sort,
                                               fields = fields
                                           })
                                   : "";
                var nextLink = page < totalPages
                                   ? urlHelper.Link(
                                       "ExpensesForGroup",
                                       new
                                           {
                                               page = page + 1,
                                               pageSize = pageSize,
                                               expenseGroupId = expenseGroupId,
                                               sort = sort,
                                               fields = fields
                                           })
                                   : "";

                var paginationHeader =
                    new
                        {
                            currentPage = page,
                            pageSize = pageSize,
                            totalCount = totalCount,
                            totalPages = totalPages,
                            previousPageLink = prevLink,
                            nextPageLnk = nextLink
                        };

                HttpContext.Current.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationHeader));

                var expensesResult = expenses
                    .ApplySort(sort)
                    .Skip(pageSize * (page -1))
                    .Take(pageSize)
                    .ToList().Select(exp => _expenseFactory.CreateDataShapedObject(exp, listOfFields));

                return this.Ok(expensesResult);
            }
            catch (Exception)
            {
                return this.InternalServerError();
            }        
        }

        [Route("expensegroups/{expenseGroupId}/expenses/{id}")]
        [Route("expenses/{id}")]
        public IHttpActionResult Get(int id, int? expenseGroupId = null)
        {
            try
            {
                Repository.Entities.Expense expense = null;

                if (expenseGroupId == null)
                {
                    expense = _repository.GetExpense(id);
                }
                else
                {
                    var expensesForGroup = _repository.GetExpenses((int)expenseGroupId);

                    if (expensesForGroup != null)
                    {
                        expense = expensesForGroup.FirstOrDefault(x => x.Id == id);
                    }
                }

                if (expense != null)
                {
                    var returnValue = _expenseFactory.CreateExpense(expense);

                    return this.Ok(returnValue);
                }
                else
                {
                    return this.NotFound();
                }
            }
            catch (Exception)
            {
                return this.InternalServerError();
            }
          
        }
    }
}