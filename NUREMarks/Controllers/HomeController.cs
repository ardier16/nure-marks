using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace NUREMarks.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

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

        [HttpGet]
        public IActionResult TimeTable(string group)
        {
            string groups = GetHtml("http://cist.nure.ua/ias/app/tt/P_API_GROUP_JSON");
            int idx = groups.IndexOf(group);
            string id = groups.Substring(idx - 19, 10).Split(':').Last();

            string url = "http://cist.nure.ua/ias/app/tt/P_API_EVENT_JSON?timetable_id=" + id + "&time_from=1486000000&time_to=1499590100";

            ViewBag.Text = GetHtml(url);
            ViewBag.Group = group;

            return View();
        }

        private string GetHtml(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        return content.ReadAsStringAsync().Result;
                    }
                }
            }
        }
    }
}
