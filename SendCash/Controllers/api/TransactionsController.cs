using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SendCash.Models;
using SendCash.ViewModels;

namespace SendCash.Controllers.api
{
    public class TransactionsController : ApiController
    {
        private SendCashEntities db = new SendCashEntities();

        // GET: api/Transactions
        public IHttpActionResult GetTransactions() {

            var tList = db.ViewAllTransactions().ToList();
            return Ok(tList);
        }

    // GET: api/Transactions/5
    [ResponseType(typeof(TransactionHeader))]
        public IHttpActionResult GetTransactionHeader(long id)
        {
            TransactionHeader transactionHeader = db.TransactionHeaders.Find(id);
            if (transactionHeader == null)
            {
                return NotFound();
            }

            return Ok(transactionHeader);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransactionHeader(long id, TransactionHeader transactionHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transactionHeader.TransactionId)
            {
                return BadRequest();
            }

            db.Entry(transactionHeader).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionHeaderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [ResponseType(typeof(TransactionHeader))]
        public IHttpActionResult PostTransactionHeader(TransactionHeader transactionHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TransactionHeaders.Add(transactionHeader);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = transactionHeader.TransactionId }, transactionHeader);
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(TransactionHeader))]
        public IHttpActionResult DeleteTransactionHeader(long id)
        {
            TransactionHeader transactionHeader = db.TransactionHeaders.Find(id);
            if (transactionHeader == null)
            {
                return NotFound();
            }

            db.TransactionHeaders.Remove(transactionHeader);
            db.SaveChanges();

            return Ok(transactionHeader);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionHeaderExists(long id)
        {
            return db.TransactionHeaders.Count(e => e.TransactionId == id) > 0;
        }
    }
}