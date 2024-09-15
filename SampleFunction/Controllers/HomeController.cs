using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SampleFunction.Models;
using System.Diagnostics;

namespace SampleFunction.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		static readonly HttpClient client = new HttpClient();

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		//http://localhost:7018/api/OnSalesUploadWriteToQueue

		[HttpPost]
		public async Task<IActionResult> Index(SalesRequest salesRequest)
		{
			salesRequest.Id = Guid.NewGuid().ToString();

			using (var content = new StringContent(JsonConvert.SerializeObject(salesRequest),
				System.Text.Encoding.UTF8, "application/json"))
			{
				// call our function and pass the content

				HttpResponseMessage response = await client.PostAsync("http://localhost:7069/api/OnSalesUploadWriteToQueue", content);

				string retValue = response.Content.ReadAsStringAsync().Result;
			}

			return RedirectToAction(nameof(Index));
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
