using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HomeFixers.Models;

namespace HomeFixers.Controllers
{
    public class ProviderBankAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProviderBankAccounts
        public ActionResult Index()
        {
            var providerBankAccounts = db.ProviderBankAccounts.Include(p => p.provider);
            return View(providerBankAccounts.ToList());
        }

        // GET: ProviderBankAccounts/Details/5
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderBankAccount providerBankAccount = db.ProviderBankAccounts.Find(id);
            if (providerBankAccount == null)
            {
                return HttpNotFound();
            }
            return View(providerBankAccount);
        }

        // GET: ProviderBankAccounts/Create
        public static int? Accid;
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ViewBag.ProviderID = new SelectList(db.ServiceProviders, "Id", "FirstName");
            Accid = id;
            return View();
        }

        // POST: ProviderBankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,AccountName,AccountNumber,RoutingNumber,Active,ProviderID")] ProviderBankAccount providerBankAccount)
        {
            providerBankAccount.ProviderID = Accid.GetValueOrDefault();
            if (ModelState.IsValid)
            {
                db.ProviderBankAccounts.Add(providerBankAccount);
                db.SaveChanges();
                return RedirectToAction("Details","ServiceProviders",new { id = Accid });
            }

            //ViewBag.ProviderID = new SelectList(db.ServiceProviders, "Id", "FirstName", providerBankAccount.ProviderID);
            return View(providerBankAccount);
        }

        // GET: ProviderBankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderBankAccount providerBankAccount = db.ProviderBankAccounts.Find(id);
            if (providerBankAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProviderID = new SelectList(db.ServiceProviders, "Id", "FirstName", providerBankAccount.ProviderID);
            return View(providerBankAccount);
        }

        // POST: ProviderBankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,AccountName,AccountNumber,RoutingNumber,Active,ProviderID")] ProviderBankAccount providerBankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(providerBankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProviderID = new SelectList(db.ServiceProviders, "Id", "FirstName", providerBankAccount.ProviderID);
            return View(providerBankAccount);
        }

        // GET: ProviderBankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderBankAccount providerBankAccount = db.ProviderBankAccounts.Find(id);
            if (providerBankAccount == null)
            {
                return HttpNotFound();
            }
            return View(providerBankAccount);
        }

        // POST: ProviderBankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProviderBankAccount providerBankAccount = db.ProviderBankAccounts.Find(id);
            db.ProviderBankAccounts.Remove(providerBankAccount);
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
