using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using CryptoKeySite.Models;

namespace CryptoKeySite.Controllers
{
   public class WebController : Controller
   {
      [Route("~/")]
      public async Task<ActionResult> Index()
      {
         MongoRepository<CryptoKey> ckr = new MongoRepository<CryptoKey>();
         return View(await ckr.SelectDescending(ck => ck.FirstBuild));
      }

      [Route("~/xml/")]
      public async Task<ActionResult> XmlFeed()
      {
         MongoRepository<CryptoKey> ckr = new MongoRepository<CryptoKey>();
         List<CryptoKey> results = await ckr.SelectDescending(ck => ck.FirstBuild);

         Response.ContentType = "text/xml";
         XDocument xdoc = new XDocument(new XElement("keys",
            from r in results
            select new XElement("key", new XAttribute("build", r.FirstBuild), new XAttribute("value", r.KeyBase64))));

         xdoc.Save(Response.OutputStream);

         return new EmptyResult();
      }

      [Route("~/add/"), Authorize]
      public ActionResult Add() => View();

      [Route("~/add/"), Authorize, HttpPost]
      public async Task<ActionResult> Add(CryptoKey ck)
      {
         if (ModelState.IsValid)
         {
            ck.Id = Guid.NewGuid();
            MongoRepository<CryptoKey> ckr = new MongoRepository<CryptoKey>();
            await ckr.Insert(ck);

            return RedirectToAction(nameof(Index));
         }

         return View(ck);
      }

      [Route("~/delete/{id}/"), Authorize]
      public async Task<ActionResult> Delete(Guid id)
      {
         MongoRepository<CryptoKey> ckr = new MongoRepository<CryptoKey>();
         await ckr.DeleteById(id);

         return RedirectToAction(nameof(Index));
      }

      [Route("~/logout/")]
      public ActionResult Logout()
      {
         FormsAuthentication.SignOut();
         return RedirectToAction(nameof(Index));
      }

      [Route("~/login/")]
      public ActionResult Login() => View();

      [Route("~/login/"), HttpPost]
      public ActionResult Login(LoginViewModel lvm)
      {
         if (ModelState.IsValid)
         {
            bool authSuccess = Membership.ValidateUser(lvm.Username, lvm.Password);

            if (authSuccess)
            {
               int expiryLength = lvm.RememberMe
                  ? 129600
                  : 60;

               FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(lvm.Username, true, expiryLength);
               string encryptedTicket = FormsAuthentication.Encrypt(ticket);

               HttpCookie cookieTicket = new HttpCookie(FormsAuthentication.FormsCookieName)
               {
                  Value = encryptedTicket,
                  Expires = DateTime.Now.AddMinutes(expiryLength),
                  Path = FormsAuthentication.FormsCookiePath
               };
               Response.Cookies.Add(cookieTicket);

               string returnUrl = string.IsNullOrEmpty(Request.QueryString["ReturnUrl"])
                  ? "~/"
                  : Request.QueryString["ReturnUrl"];

               return Redirect(returnUrl);
            }
         }

         return View(lvm);
      }

      [Route("~/register/")]
      public ActionResult Register()
      {
         if (!bool.Parse(ConfigurationManager.AppSettings["EnableRegistrations"]))
         {
            return RedirectToAction(nameof(Index));
         }
         return View();
      }

      [Route("~/register/"), HttpPost]
      public ActionResult Register(RegisterViewModel rvm)
      {
         if (!bool.Parse(ConfigurationManager.AppSettings["EnableRegistrations"]))
         {
            return RedirectToAction(nameof(Index));
         }

         if (ModelState.IsValid)
         {
            MembershipCreateStatus status;
            MembershipUser mu = Membership.CreateUser(rvm.Username, rvm.Password, rvm.Username, "?", "!", true, out status);

            if (status == MembershipCreateStatus.Success)
            {
               mu.IsApproved = true;
               Membership.UpdateUser(mu);
               return RedirectToAction(nameof(Index));
            }
         }

         return View(rvm);
      }
   }
}