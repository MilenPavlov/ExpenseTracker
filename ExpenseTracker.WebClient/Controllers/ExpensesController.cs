using ExpenseTracker.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ExpenseTracker.WebClient.Controllers
{
    public class ExpensesController : Controller
    {
        // GET: Expenses
        public ActionResult Index()
        {
            return View();
        }
               

        // GET: Expenses/Create
        public ActionResult Create(int expenseGroupId)
        {
            // create a new expense for an expensegroup
            return View();
        }


        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Expense expense)
        {
            return View();
        }

         

        // GET: Expenses/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Expense expense)
        {
             
            return View();

        }

        // GET: Expenses/Delete/5
        public ActionResult Delete(int expenseGroupId, int id)
        { 

            return View();
 
        }
         
    }
}
