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
using HomeFixers.WebserviceModels;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HomeFixers.Controllers
{
    [Authorize]
    public class ServiceProvidersController : Controller
    {
        public static int provredirect = 0;
        public static DateTime slotDate;
        public static string sTime;


        HashSet<string> stype = new HashSet<string>();

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceProviders
        [Authorize(Roles = "Provider, Admin, Customer")]
        public ActionResult Index()
        {
            int AccountID = 0;
            if (User.IsInRole("Provider"))
            {

                var userexist = db.ServiceProviders
                                            .Where(s => s.EmailId == User.Identity.Name)
                                            .Count();

                if (userexist == 0 && provredirect == 0)
                {
                    return RedirectToAction("Create", "ServiceProviders");
                }
                else
                {
                    provredirect = 0;
                    var userp = db.ServiceProviders.Where(u => u.EmailId == User.Identity.Name);
                    foreach (ServiceProvider servicep in userp)
                    {
                        AccountID = servicep.Id;
                    }
                    return RedirectToAction("Details", "ServiceProviders", new { id = AccountID });
                }
            }
            else if(User.IsInRole("Admin"))
            {
                var serviceProviders = db.ServiceProviders.Include(s => s.service);
                return View(serviceProviders.ToList());
            }
            else if (User.IsInRole("Customer"))
            {
                var serviceProviders = db.ServiceProviders.Include(s => s.service);
                return View(serviceProviders.ToList());
            }
            else
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //var serviceProviders = db.ServiceProviders.Include(s => s.service);
            //return View(serviceProviders.ToList());
        }

        public static IQueryable<ServiceProvider> filter;
        //------------------------ No Longer Required ------------------------
        //public ActionResult ProviderSearch (string service, string Location)
        //{
        //    bool searchPerformed = false;
        //    var filterProviders = db.ServiceProviders.Include(s => s.service);
        //    if(!string.IsNullOrWhiteSpace(service) & string.IsNullOrWhiteSpace(Location))
        //    {
        //        var selser = db.Services.Single(ss => ss.ServiceName == service);
        //        filterProviders = db.ServiceProviders.Where(ps=>ps.ServiceID== selser.id);
        //        searchPerformed = true;
        //    }
        //    else
        //    if(string.IsNullOrWhiteSpace(service) & !string.IsNullOrWhiteSpace(Location))
        //    {
        //        filterProviders = db.ServiceProviders.Where(fp2 => fp2.Zip == Location);
        //        searchPerformed = true;
        //    }
        //    else
        //        if(!string.IsNullOrWhiteSpace(service) & !string.IsNullOrWhiteSpace(Location))
        //    {
        //        var selser = db.Services.Single(ss => ss.ServiceName == service);
        //        filterProviders = db.ServiceProviders.Where(fp3 => fp3.Zip == Location && fp3.ServiceID == selser.id );
        //        searchPerformed = true;
        //    }

        //        filter = filterProviders;
        //        return View();           

        //}

        //public PartialViewResult SearchResult()
        //{
        //    return PartialView(filter.ToList());
        //}

        //public ActionResult Book()
        //{

        //    var serviceProviders = db.ServiceProviders.Include(s => s.service);
        //    return View(serviceProviders.ToList());
            
        //}
        //public ActionResult Booked(int? id)
        //{
        //    int? sp = id;
        //    return RedirectToAction("Books", "BucketLists", new { id = sp });

        //}

        //<------------------------------------------------------------------------->


        // GET: ServiceProviders/Details/5
        [Authorize(Roles = "Provider, Admin, Customer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceProvider serviceProvider = db.ServiceProviders.Find(id);
            var serv = db.ServiceProviders.Single(s => s.Id == id);
            if (serv.EmailId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (serviceProvider == null)
            {
                return HttpNotFound();
            }
            return View(serviceProvider);
        }

        public PartialViewResult bucketlist(int? id)
        {

            var bucket = db.BucketLists
                .Where(b => b.ServiceProviderID == id);

            return PartialView(bucket.ToList());
        }

        // GET: ServiceProviders/Create
        [Authorize(Roles = "Provider, Admin")]
        public ActionResult Create()
        {
            ViewBag.ServiceID = new SelectList(db.Services, "id", "ServiceName");
            return View();
        }

        // POST: ServiceProviders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Spam]
        public ActionResult Create([Bind(Include = "Id,FirstName,MiddleName,LastName,MobileNumber,EmailId,Address_1,Address_2,City,State,Zip,ServiceID,SSN")] ServiceProvider serviceProvider)
        {
            if (ModelState.IsValid)
            {
                serviceProvider.EmailId = User.Identity.Name;
                ssnWebservice(serviceProvider).Wait();
                db.ServiceProviders.Add(serviceProvider);
                db.SaveChanges();
                return RedirectToAction("Details","ServiceProviders",new { id = serviceProvider.Id });
            }
            if (Spam.spamflag == 1)
            {
                provredirect = 1;
                return RedirectToAction("Index");
            }

            ViewBag.ServiceID = new SelectList(db.Services, "id", "ServiceName", serviceProvider.ServiceID);
            return View(serviceProvider);
        }

        private async System.Threading.Tasks.Task<bool> ssnWebservice(ServiceProvider sp)
        {
            long num;
            bool isnum = long.TryParse(sp.SSN, out num);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://restpolice.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string param = "api/Person/" + num;
                HttpResponseMessage response = await client.GetAsync(param).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Person person = await response.Content.ReadAsAsync<Person>();
                    if (person.CriminalRecords >= 1 || person.DrivingTickets > 5)
                    {
                        sp.Verified = false;
                    }
                    else
                    {
                        sp.Verified = true;
                    }
                }
                else
                {
                    sp.Verified = false;
                }
            }
            return true;
        }

        // GET: ServiceProviders/Edit/5
        [Authorize(Roles = "Provider, Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceProvider serviceProvider = db.ServiceProviders.Find(id);
            if (serviceProvider.EmailId != User.Identity.Name)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (serviceProvider == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceID = new SelectList(db.Services, "id", "ServiceName", serviceProvider.ServiceID);
            return View(serviceProvider);
        }

        // POST: ServiceProviders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,MiddleName,LastName,MobileNumber,EmailId,Address_1,Address_2,City,State,Zip,ServiceID")] ServiceProvider serviceProvider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceProvider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceID = new SelectList(db.Services, "id", "ServiceName", serviceProvider.ServiceID);
            return View(serviceProvider);
        }

        // GET: ServiceProviders/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceProvider serviceProvider = db.ServiceProviders.Find(id);
            if (serviceProvider == null)
            {
                return HttpNotFound();
            }
            return View(serviceProvider);
        }

        // POST: ServiceProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceProvider serviceProvider = db.ServiceProviders.Find(id);
            db.ServiceProviders.Remove(serviceProvider);
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

        
        public ActionResult SearchSchedule(string service, string Zip, DateTime? SDate, string Time)
        {
            bool filterflag = false;
            bool searchPerformed = false;

            var filterProviders = db.ServiceProviders.Include(s => s.service);
            if (!string.IsNullOrWhiteSpace(service) & string.IsNullOrWhiteSpace(Zip))
            {
                var selser = db.Services.Single(ss => ss.ServiceName == service);
                filterProviders = db.ServiceProviders.Where(ps => ps.ServiceID == selser.id);
                searchPerformed = true;
            }
            else
            if (string.IsNullOrWhiteSpace(service) & !string.IsNullOrWhiteSpace(Zip))
            {
                filterProviders = db.ServiceProviders.Where(fp2 => fp2.Zip == Zip);
                searchPerformed = true;
            }
            else
                if (!string.IsNullOrWhiteSpace(service) & !string.IsNullOrWhiteSpace(Zip))
            {
                var selser = db.Services.Single(ss => ss.ServiceName == service);
                filterProviders = db.ServiceProviders.Where(fp3 => fp3.Zip == Zip && fp3.ServiceID == selser.id);
                searchPerformed = true;
            }

            // filter = filterProviders;
            List<ServiceProvider> splist = new List<ServiceProvider>();
            if (SDate != null)
            {
                int count = 0;
                //  splist = filterProviders.ToList();
                foreach (ServiceProvider sp in filterProviders)
                {
                    if (sp.ScheduleList.Count != 0)
                    {
                        foreach (Schedule sc in sp.ScheduleList)
                        {
                            if (sc.scheduledate == SDate)
                            {
                                filterflag = true;
                                if ((!string.IsNullOrWhiteSpace(Time)) & (Time.Equals("8-10")))
                                {
                                    if (!sc.Sch_slot1)
                                    {
                                        splist.Add(sp);
                                        count = 1;
                                    }

                                }
                                else if ((!string.IsNullOrWhiteSpace(Time)) & (Time.Equals("10-12")))
                                {
                                    if (!sc.Sch_slot2)
                                    {
                                        splist.Add(sp);
                                        count = 1;
                                    }
                                }

                                else if ((!string.IsNullOrWhiteSpace(Time)) & (Time.Equals("1-3")))
                                {
                                    if (!sc.Sch_slot3)
                                    {
                                        splist.Add(sp);
                                        count = 1;
                                    }
                                }

                                else if ((!string.IsNullOrWhiteSpace(Time)) & (Time.Equals("3-5")))
                                {
                                    if (!sc.Sch_slot4)
                                    {
                                        splist.Add(sp);
                                        count = 1;
                                    }
                                }
                            }
                            else
                            {
                                splist.Add(sp);
                            }
                            if (count != 0)
                            {
                                searchPerformed = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        splist.Add(sp);
                        searchPerformed = true;

                    }

                }


            }


            if (SDate != null  && string.IsNullOrWhiteSpace(service))
            {
                //var serviceProviders = db.ServiceProviders.ToList();
                //return View(serviceProviders);
                searchPerformed = true;
            }

            if (SDate == null)
                SDate = DateTime.Now;
            slotDate = (DateTime)SDate;
            sTime = Time;

            if (searchPerformed)
            {
                if (splist.Count == 0 && !filterflag)
                    return View(filterProviders.ToList());
                else
                    return View(splist);
            }
            else
            {
                return View(new List<ServiceProvider>());

            }

        }

        /// <summary>
        /// Nikhil Gupta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BookService(int? id)
        {
            BucketList bk = new BucketList();
            bk.DateAdded = slotDate;
            bk.slotbooked = sTime;
            return RedirectToAction("BkBook", "BucketLists", new { id = id, DateAdded = bk.DateAdded, slotbooked = bk.slotbooked });
        }
    }
}
