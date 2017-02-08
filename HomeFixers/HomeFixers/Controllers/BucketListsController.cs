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
using System.Net.Http;
using System.Net.Http.Headers;
using HomeFixers.WebserviceModels;
using System.Threading.Tasks;

namespace HomeFixers.Controllers
{
    [Authorize]
    public class BucketListsController : Controller
    {
        public static int bucredirect = 0;
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<Schedule> scheduleList = new List<Schedule>();
        // GET: BucketLists
        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Index()
        {
            if(User.IsInRole("Customer"))
            {
                bucredirect = 0;
                var cust = db.Customers.Single(cu => cu.EmailId == User.Identity.Name);
                return RedirectToAction("Details", "Customers", new { id = cust.Id });
            }
            var bucketLists = db.BucketLists.Include(b => b.customer).Include(b => b.serviceprovider);
            return View(bucketLists.ToList());
        }

        // GET: BucketLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BucketList bucketList = db.BucketLists.Find(id);
            var cust = db.Customers.Single(c => c.Id == bucketList.CustomerID);
            var serv = db.ServiceProviders.Single(s => s.Id == bucketList.ServiceProviderID);
            if (cust.EmailId != User.Identity.Name || serv.EmailId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bucketList == null)
            {
                return HttpNotFound();
            }
            return View(bucketList);
        }

        //public static int servicepid;
        //public ActionResult Books(int id)
        //{
        //    servicepid = id;
        //    return RedirectToAction("Create", "BucketLists");
        //}

        public static int servicepid;
        public static DateTime slotDate;
        public static string sTime;
        public ActionResult BkBook(int id, DateTime? DateAdded, string slotbooked)
        {

            servicepid = id;
            if (DateAdded == null)
                DateAdded = DateTime.Now;
            slotDate = (DateTime)DateAdded;
            sTime = slotbooked;
            return RedirectToAction("Create", "BucketLists");
        }

        // GET: BucketLists/Create

        [Authorize(Roles = "Admin, Customer")]
        public ActionResult Create()
        {
            int custid;
            var cust = db.Customers.Where(cu => cu.EmailId == User.Identity.Name);
            var serv = db.ServiceProviders.Where(ser => ser.Id == servicepid);
            foreach (Customer cu in cust)
            {
                custid = cu.Id;
            }
            ViewBag.DateAdded = slotDate.Date;
            ViewBag.slotbooked = sTime;
            ViewBag.CustomerID = new SelectList(cust, "Id", "FirstName");
            ViewBag.ServiceProviderID = new SelectList(serv, "Id", "FirstName");
            return View();
        }

        // POST: BucketLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Spam]
        public ActionResult Create([Bind(Include = "ID,DateAdded,Description,EmailId,Address_1,Address_2,City,State,Zip,Completed,CustomerID,ServiceProviderID,slotbooked")] BucketList bucketList)
        {
            
            //bucketList.DateAdded = DateTime.Now;
            bucketList.EmailId = User.Identity.Name;
            //bucketList.ServiceProviderID = servicepid;
            if (ModelState.IsValid)
            {
                db.BucketLists.Add(bucketList);
                UpdateSlotInfo(bucketList.DateAdded, bucketList.slotbooked, bucketList.ServiceProviderID);
                maketransaction(bucketList.CustomerID);
                db.SaveChanges();
                var service = db.ServiceProviders.Single(s => s.Id == bucketList.ServiceProviderID);
                var provide = db.Services.Single(p => p.id == service.ServiceID);
                sendEmail(service.EmailId, provide.ServiceName, bucketList.ID, bucketList.DateAdded,bucketList.slotbooked).Wait();
                return RedirectToAction("Details","Customers", new {id=bucketList.CustomerID });
            }
            if (Spam.spamflag == 1)
            {
                bucredirect = 1;
                return RedirectToAction("Index");
            }
            var cust = db.Customers.Where(cu => cu.EmailId == User.Identity.Name);
            var serv = db.ServiceProviders.Where(ser => ser.Id == servicepid);
            ViewBag.DateAdded = slotDate.Date;
            ViewBag.slotbooked = sTime;
            ViewBag.CustomerID = new SelectList(cust, "Id", "FirstName");
            ViewBag.ServiceProviderID = new SelectList(serv, "Id", "FirstName");
            return View(bucketList);
        }
        public async Task sendEmail(string sEmail,string service,int id,DateTime bookingDate,string slot)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:54589/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Email email = new Email();
                email.serviceEmail = sEmail;
                email.customerEmail = User.Identity.Name;
                email.serviceName = service;
                email.bookingID = id;//booking id
                email.bookingDate = bookingDate;
                email.slot = slot;//appointment date
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Mail", email).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                   
                }
                
            }

        }
        // GET: BucketLists/Edit/5
        public static int flag = 0;
        public static DateTime date;
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BucketList bucketList = db.BucketLists.Find(id);
            var cust = db.Customers.Single(c => c.Id == bucketList.CustomerID);
            var serv = db.ServiceProviders.Single(s => s.Id == bucketList.ServiceProviderID);
            if (cust.EmailId != User.Identity.Name || serv.EmailId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bucketList == null)
            {
                return HttpNotFound();
            }
            
            if(bucketList.Completed)
            { 
            flag = 1;
            }
            var cust1 = db.Customers.Where(cu => cu.Id == bucketList.CustomerID);
            var serv1 = db.BucketLists.Where(ser => ser.ServiceProviderID == bucketList.ServiceProviderID);
            foreach (BucketList bc in serv1)
            {
                bc.ServiceProviderID = servicepid;
            }
            ViewBag.CustomerID = new SelectList(cust1, "Id", "FirstName");
            ViewBag.ServiceProviderID = new SelectList(serv1, "Id", "FirstName");
            return View(bucketList);
        }

        // POST: BucketLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DateAdded,Description,EmailId,Address_1,Address_2,City,State,Zip,Completed,CustomerID,ServiceProviderID")] BucketList bucketList)
        {
            
            if (ModelState.IsValid)
            {
                if(flag == 1 && !bucketList.Completed)
                { ModelState.AddModelError("Completed", "Service Status one completed can't be changed.");
                    //ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", bucketList.CustomerID);
                    //ViewBag.ServiceProviderID = new SelectList(db.ServiceProviders, "Id", "FirstName", bucketList.ServiceProviderID);
                    
                }
                else {
                    if (ModelState.IsValid)
                    { 
                        db.Entry(bucketList).State = EntityState.Modified;
                        bucketList.DateAdded = date;
                        db.SaveChanges();
                        flag = 0;
                        if(User.IsInRole("Customer"))
                        return RedirectToAction("Details","Customers",new {id=bucketList.CustomerID });
                        else
                        return RedirectToAction("Details", "ServiceProviders", new { id = bucketList.ServiceProviderID });
                    }
                }
            }
            var cust = db.Customers.Where(cu => cu.Id == bucketList.CustomerID);
            var serv = db.ServiceProviders.Where(ser => ser.Id == servicepid);
            ViewBag.CustomerID = new SelectList(cust, "Id", "FirstName");
            ViewBag.ServiceProviderID = new SelectList(serv, "Id", "FirstName");
            return View(bucketList);
        }

        private void maketransaction(int customerID)
        {
            var cust = db.CustomerBankAccounts.Single(b => b.CustomerID == customerID);
            Transaction newTran = new Transaction();
            newTran.Amount = 25;
            newTran.CustomerBankAccountID = cust.ID;
            db.Transactions.Add(newTran);
        }

        // GET: BucketLists/Delete/5
        [Authorize(Roles ="Admin, Customer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BucketList bucketList = db.BucketLists.Find(id);
            if (bucketList == null)
            {
                return HttpNotFound();
            }
            return View(bucketList);
        }

        // POST: BucketLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BucketList bucketList = db.BucketLists.Find(id);
            db.BucketLists.Remove(bucketList);
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


        public void UpdateSlotInfo(DateTime DateAdded, string slotbooked, int ServiceProviderID)
        {
            var schexist = db.Schedules.Where(sch => sch.scheduledate == DateAdded && sch.ServiceproviderID == ServiceProviderID).Count();
            if (schexist != 0)
            {
                var schedule = db.Schedules.Single(s => s.ServiceproviderID == ServiceProviderID && s.scheduledate == DateAdded);
                if (!string.IsNullOrEmpty(slotbooked))
                {
                    if (slotbooked.Equals("8-10")) schedule.Sch_slot1 = true;
                    else if (slotbooked.Equals("10-12")) schedule.Sch_slot2 = true;
                    else if (slotbooked.Equals("1-3")) schedule.Sch_slot3 = true;
                    else if (slotbooked.Equals("3-5")) schedule.Sch_slot4 = true;
                }

            }
            else
            {
                //var schid = db.Schedules.Max(sch => sch.Id);
                Schedule sc = new Schedule();
                //sc.Id = ++schid;
                sc.scheduledate = DateAdded;
                if (!string.IsNullOrEmpty(slotbooked))
                {
                    if (slotbooked.Equals("8-10")) sc.Sch_slot1 = true;
                    else if (slotbooked.Equals("10-12")) sc.Sch_slot2 = true;
                    else if (slotbooked.Equals("1-3")) sc.Sch_slot3 = true;
                    else if (slotbooked.Equals("3-5")) sc.Sch_slot4 = true;
                }
                sc.ServiceproviderID = ServiceProviderID;
                db.Schedules.Add(sc);
            }
        }
    }
}
