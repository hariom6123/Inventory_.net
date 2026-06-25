using System.Diagnostics;

namespace InventoryManagement.Web.Controllers
{
    /// <summary>
    /// Default controller used by the MVC error pipeline.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Default request — returns the Error view with request id.
        /// </summary>
        [Route("Error")]
        public IActionResult Index()
        {
            ViewBag.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        /// <summary>
        /// Returns the not-found view.
        /// </summary>
        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            ViewBag.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View("NotFound");
        }
    }
}