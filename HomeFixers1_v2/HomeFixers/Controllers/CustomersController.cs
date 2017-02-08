using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HomeFixers.Models;
using HomeFixers.Filters;

namespace HomeFixers.Controllers
{
    [Authorize(Roles = "Customer, Admin")]
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public static int custredirect = 0;
        // GET: Customers
        public ActionResult Index()
        {
            int AccountID = 0;
            if (User.IsInRole("Customer"))
            {

                var userexist = db.Customers
                                            .Where(c => c.EmailId == User.Identity.Name)
                                            .Count();
                
                var bankcheck = db.CustomerBankAccounts
                                            .Where(bc => bc.UserName == User.Identity.Name)
                                            .Count();
                if(custredirect == 1)
                {
                    var user = db.Customers.Single(u => u.EmailId == User.Identity.Name);
                    custredirect = 0;
                    return RedirectToAction("Create", "CustomerBankAccounts", new { id = user.Id });
                }

                if (userexist == 0)
                {
                    return RedirectToAction("Create", "Customers");
                }
                else if (bankcheck == 0)
                {
                    var user = db.Customers.Single(u => u.EmailId == User.Identity.Name);
                    return RedirectToAction("Create", "CustomerBankAccounts",new { id = user.Id });
                }
                else
                {
                    var userp = db.Customers.Where(u => u.EmailId == User.Identity.Name);
                    foreach (Customer customerp in userp)
                    {
                        AccountID = customerp.Id;
                    }
                    return RedirectToAction("Details", "Customers", new { id = AccountID });
                }
            }
            else if (User.IsInRole("Admin"))
            {
                return View(db.Customers.ToList());
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //return View(db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if(customer.EmailId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        public PartialViewResult bucketlist(int? id)
        {

            var bucket = db.BucketLists
                .Where(b => b.CustomerID == id);

            return PartialView(bucket.ToList());
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Spam]
        public ActionResult Create([Bind(Include = "Id,FirstName,MiddleName,LastName,MobileNumber,EmailId,Address_1,Address_2,City,State,Zip")] Customer customer)
        {
            customer.EmailId = User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Create","CustomerBankAccounts",new { id = customer.Id });
            }
            if (Spam.spamflag == 1)
            {
                custredirect = 1;
                return RedirectToAction("Index");
            }

            return View(customer);
            
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer.EmailId != User.Identity.Name)
            {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,MiddleName,LastName,MobileNumber,EmailId,Address_1,Address_2,City,State,Zip")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer.EmailId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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
