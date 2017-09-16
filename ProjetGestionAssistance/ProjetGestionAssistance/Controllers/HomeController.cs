using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjetGestionAssistance.Controllers
{
    public class HomeController : Controller
    {
        const string SessionId = "_Id";

        //Controlleur par défaut quand on accède on site
        //Modification temporaire pour simuler un login
        //On vérifie si le SessionId est existant
        //Si le SessionId est différent de null, l'utilsateur est déja connecté et on le dirige vers Index
        //Sinon, on le redige vers l'action Login dans le controlleur CompteController
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32(SessionId) != null)
                return View();
            else
                return RedirectToAction("Login", "Compte");
        }

        //Ces actions ont été crées par défaut, ils ne sont pas en utilisation
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
