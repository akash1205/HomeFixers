using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomeFixers.Models;
using HomeFixers.Filters;

namespace HomeFixers.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Index()
        {
            if(User.IsInRole("Admin"))
            { 
            var transactions = db.Transactions.Include(t => t.customerbankaccount);
            return View(transactions.ToList());
            }
            else
            {
                var custbank = db.CustomerBankAccounts.Single(cb => cb.UserName == User.Identity.Name);
                var transactions = db.Transactions.Where(t => t.CustomerBankAccountID == custbank.ID);
                return View(transactions.ToList());
            }
        }

        // GET: Transactions/Details/5
        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? id)
        {
            var bankc = db.CustomerBankAccounts.Where(b => b.CustomerID == id);
            ViewBag.CustomerBankAccountID = new SelectList(bankc, "ID", "NameOnCard");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Spam]
        public ActionResult Create([Bind(Include = "ID,Amount,CustomerBankAccountID")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var cust = db.CustomerBankAccounts.Single(c => c.ID == transaction.CustomerBankAccountID);
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details", "Customers", new { id = cust.CustomerID });
                //return RedirectToAction("Index");
            }
            if (Spam.spamflag == 1)
            {
                var cust = db.CustomerBankAccounts.Single(c => c.ID == transaction.CustomerBankAccountID);
                return RedirectToAction("Details","Customers",new {id = cust.CustomerID});
            }

            ViewBag.CustomerBankAccountID = new SelectList(db.CustomerBankAccounts, "ID", "NameOnCard", transaction.CustomerBankAccountID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerBankAccountID = new SelectList(db.CustomerBankAccounts, "ID", "NameOnCard", transaction.CustomerBankAccountID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Amount,CustomerBankAccountID")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerBankAccountID = new SelectList(db.CustomerBankAccounts, "ID", "NameOnCard", transaction.CustomerBankAccountID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
