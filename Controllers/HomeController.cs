using AppSoftDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
namespace AppSoftDoc.Controllers
    {
    public class HomeController : Controller
        {
        //static string cadena = "Data Source=DESKTOP-P07JQ4C;Initial Catalog=GamesClub;Integrated Security=True";


        private readonly ILogger<HomeController> _logger;



        public HomeController(ILogger<HomeController> logger)
            {
            _logger = logger;
            }

        public IActionResult Index()
            {
            return View();
            }

        public IActionResult SobreNosotros()
            {
            return View();
            }

        public IActionResult Contacto()
            {
            return View();
            }


      //  [HttpPost]  
        public IActionResult Login()
            {
           
            return View();


            }
   


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
