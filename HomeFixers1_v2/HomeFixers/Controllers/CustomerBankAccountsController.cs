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
    
    [Authorize(Roles ="Customer,Admin")]
    public class CustomerBankAccountsController : Controller
    {
        //public static int cba = 0;
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerBankAccounts
        [Authorize(Roles ="Admin,Customer")]
        public ActionResult Index()
        {
            
            if(User.IsInRole("Customer"))
            {
                var cust = db.Customers.Single(cu => cu.EmailId == User.Identity.Name);
                return RedirectToAction("Details", "Customers", new { id = cust.Id });
            }
             
            var customerBankAccounts = db.CustomerBankAccounts.Include(c => c.customer);
            return View(customerBankAccounts.ToList());
            
        }

        public ActionResult Redirect(int? id)
        {
            int? idr;
            var custb = db.CustomerBankAccounts.Single(b => b.UserName == User.Identity.Name);
            idr = custb.ID;
            return RedirectToAction("Details", "CustomerBankAccounts", new { id = idr });
        }

        // GET: CustomerBankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            var cust = db.Customers.Single(c => c.EmailId == User.Identity.Name);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerBankAccount customerBankAccount = db.CustomerBankAccounts.Find(id);
            if (customerBankAccount.CustomerID != cust.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (customerBankAccount == null)
            {
                return HttpNotFound();
            }
            return View(customerBankAccount);
        }
        public static int? Accid;
        // GET: CustomerBankAccounts/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accid = id;
            //ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName");
            return View();
        }

        // POST: CustomerBankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Spam]
        public ActionResult Create([Bind(Include = "ID,NameOnCard,CardNumber,CVV,ExpiryDate,CustomerID")] CustomerBankAccount customerBankAccount)
        {
            customerBankAccount.CustomerID = Accid.GetValueOrDefault();
            if (ModelState.IsValid)
            {
                customerBankAccount.UserName = User.Identity.Name;
                db.CustomerBankAccounts.Add(customerBankAccount);
                db.SaveChanges();
                return RedirectToAction("Details","Customers",new { id = Accid });
            }
            if (Spam.spamflag == 1)
            {
                
                return RedirectToAction("Index");
            }

            //ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", customerBankAccount.CustomerID);
            return View(customerBankAccount);
        }

        // GET: CustomerBankAccounts/Edit/5
        public static int custid;
        public ActionResult Edit(int? id)
        {
            var cust = db.Customers.Single(c => c.EmailId == User.Identity.Name);
            custid = cust.Id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerBankAccount customerBankAccount = db.CustomerBankAccounts.Find(id);
            if (customerBankAccount.CustomerID != cust.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (customerBankAccount == null)
            {
                return HttpNotFound();
            }
            
            //ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", customerBankAccount.CustomerID);
            return View(customerBankAccount);
        }

        // POST: CustomerBankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameOnCard,CardNumber,CVV,ExpiryDate")] CustomerBankAccount customerBankAccount)
        {

            if (ModelState.IsValid)
            {
                customerBankAccount.CustomerID = custid;
                customerBankAccount.UserName = User.Identity.Name;
                db.Entry(customerBankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "CustomerBankAccounts", new { id = customerBankAccount.ID });
            }
            //ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", customerBankAccount.CustomerID);
            return View(customerBankAccount);
        }

        // GET: CustomerBankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            var cust = db.Customers.Single(c => c.EmailId == User.Identity.Name);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerBankAccount customerBankAccount = db.CustomerBankAccounts.Find(id);
            if (customerBankAccount.CustomerID != cust.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (customerBankAccount == null)
            {
                return HttpNotFound();
            }
            return View(customerBankAccount);
        }

        // POST: CustomerBankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerBankAccount customerBankAccount = db.CustomerBankAccounts.Find(id);
            db.CustomerBankAccounts.Remove(customerBankAccount);
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
