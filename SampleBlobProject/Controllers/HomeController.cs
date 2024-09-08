using Microsoft.AspNetCore.Mvc;
using SampleBlobProject.Models;
using SampleBlobProject.Services;
using System.Diagnostics;

namespace SampleBlobProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContainerService _containerService;

        public HomeController(IContainerService containerService, ILogger<HomeController> logger)
        {
            _containerService = containerService;
			_logger = logger;

		}

		private readonly ILogger<HomeController> _logger;

        public async Task<IActionResult> Index()
        {
            return View(await _containerService.GetAllContainerAndBlobs());
        }

        public IActionResult Privacy()
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
