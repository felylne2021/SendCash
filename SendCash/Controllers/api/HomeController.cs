using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SendCash.Models;
using SendCash.ViewModels;

namespace SendCash.Controllers.api {
    public class HomeController : ApiController {
        private SendCashEntities db;

        public HomeController() {
            db = new SendCashEntities();
        }

        public IHttpActionResult Login(LoginViewModel user) {

            ValidateTokenHandler v = new ValidateTokenHandler();

            Account account = db.Accounts.FirstOrDefault(x => x.AccountName == user.AccountName && x.AccountNumber == user.AccountNumber);
            if (account == null) {

                return Json("Your credentials are incorrect.");
            }

            //return v.GetToken(user);
            return null;
        }
    }
}