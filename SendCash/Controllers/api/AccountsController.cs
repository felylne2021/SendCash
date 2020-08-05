using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SendCash.Models;

namespace SendCash.Controllers.api {
    public class AccountsController : ApiController {
        private SendCashEntities db ;

        public AccountsController() {
            db = new SendCashEntities();
        }

        // GET: api/Accounts
        public IHttpActionResult GetCustomers() {
            var accounts = db.Accounts.Include(b => b.Bank);

            return Json(accounts.Select(acc => new{
                acc.AccountName,
                acc.AccountNumber,
                acc.Bank.BankName,
                acc.AccountId
            }).ToList());

        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> GetAccount(long id) {
            Account account = await db.Accounts.FindAsync(id);
            if (account == null) {
                return NotFound();
            }

            return Ok(account);
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAccount(long id, Account account) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != account.AccountId) {
                return BadRequest();
            }

            db.Entry(account).State = EntityState.Modified;

            try {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!AccountExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> PostAccount(Account account) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = account.AccountId }, account);
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Account))]
        public async Task<IHttpActionResult> DeleteAccount(long id) {
            Account account = await db.Accounts.FindAsync(id);
            if (account == null) {
                return NotFound();
            }

            db.Accounts.Remove(account);
            await db.SaveChangesAsync();

            return Ok(account);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(long id) {
            return db.Accounts.Count(e => e.AccountId == id) > 0;
        }
    }
}