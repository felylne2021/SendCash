using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SendCash.Models;

namespace SendCash.Controllers
{
    public class TransactionsController : Controller
    {
        private SendCashEntities db = new SendCashEntities();

        // GET: Transactions
        public ActionResult Index()
        {
            return View();
        }

        // GET: Transactions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionHeader transactionHeader = db.TransactionHeaders.Find(id);
            if (transactionHeader == null)
            {
                return HttpNotFound();
            }
            return View(transactionHeader);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountId", "AccountNumber");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionId,SenderId,TransactionDt")] TransactionHeader transactionHeader)
        {
            if (ModelState.IsValid)
            {
                db.TransactionHeaders.Add(transactionHeader);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SenderId = new SelectList(db.Accounts, "AccountId", "AccountNumber", transactionHeader.SenderId);
            return View(transactionHeader);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionHeader transactionHeader = db.TransactionHeaders.Find(id);
            if (transactionHeader == null)
            {
                return HttpNotFound();
            }
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountId", "AccountNumber", transactionHeader.SenderId);
            return View(transactionHeader);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionId,SenderId,TransactionDt")] TransactionHeader transactionHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactionHeader).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SenderId = new SelectList(db.Accounts, "AccountId", "AccountNumber", transactionHeader.SenderId);
            return View(transactionHeader);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransactionHeader transactionHeader = db.TransactionHeaders.Find(id);
            if (transactionHeader == null)
            {
                return HttpNotFound();
            }
            return View(transactionHeader);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TransactionHeader transactionHeader = db.TransactionHeaders.Find(id);
            db.TransactionHeaders.Remove(transactionHeader);
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
