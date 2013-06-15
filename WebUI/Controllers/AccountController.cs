using System.Web.Mvc;
using WebUI.Infrastructure.Abstract;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private IAuthProvider authProvider;

        public AccountController (IAuthProvider auth)
        {
            authProvider = auth;
        }

       public ViewResult LogOn()
       {
           return View();
       }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model,string returnUrl)
        {
            if(ModelState.IsValid)
            {
                if(authProvider.Authenticate(model.UserName, model.Password))
                {
                    var red = Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                    return red;
                }
                else
                {
                    ModelState.AddModelError("", "Неправильное имя или пароль");
                    return View(); 
                }
                
            }
            else
            {
                return View();
            }
        }
    }
}
