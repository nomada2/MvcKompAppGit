﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MvcKompApp.Models;
using MvcKompMvc4.ServiceControllers.Shared;
using MvcKompMvc4.ServiceControllers.Home;
using MvcKomp3.Infrastructure;
using System.Security.Principal;

namespace MvcKompApp.Controllers
{
    public class HomeController : Controller
    {
        IHomeService _service;

        public HomeController()
        {
            _service = new HomeService();
        }


        public ActionResult Index([ModelBinder(typeof(PrincipalModelBinder))] IPrincipal user)
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PartialViewRefreshHandler()
        {
            return View();
        }

        public ActionResult RefreshPartial(FormCollection form)
        {
            string name = form["Name"];

            ViewBag.Name = name == null ? "No one, just refreshing" : name;
            ViewBag.DateTime = DateTime.Now.ToString();

            return PartialView("_DateTimePartial");
        }

        public ActionResult WebGridIndex()
        {
            var mostPopular = GetFavouriteGiveNames();
            return View(mostPopular);
        }

        private List<FavouriteGivenName> GetFavouriteGiveNames()
        {
            return _service.GetFavouriteGiveNames();
        }

        [HttpGet]
        public JsonResult EfficientWebGridPaging(int? page)
        {
            var mostPopular = GetFavouriteGiveNames();
            int skip = page.HasValue ? page.Value - 1 : 0;
            var data = mostPopular.OrderBy(o => o.Id).Skip(skip * 5).Take(5).ToList();
            var grid = new WebGrid(data);
            var htmlString = grid.GetHtml(tableStyle: "webGrid",
                                          headerStyle: "header",
                                          alternatingRowStyle: "alt",
                                          htmlAttributes: new { id = "DataTable" });
            return Json(new
            {
                Data = htmlString.ToHtmlString(),
                Count = mostPopular.Count() / 5
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContactUs");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUsInput input)
        {
            // Validate the model being submitted
            if (!ModelState.IsValid)
            {
                // If the incoming request is an Ajax Request 
                // then we just return a partial view (snippet) of HTML 
                // instead of the full page
                if (Request.IsAjaxRequest())
                    return PartialView("_ContactUs", input);

                return View(input);
            }

            // TODO: A real app would send some sort of email here

            if (Request.IsAjaxRequest())
            {
                // Same idea as above
                return PartialView("_ThanksForFeedback", input);
            }

            // A standard (non-Ajax) HTTP Post came in
            // set TempData and redirect the user back to the Home page
            TempData["Message"] = string.Format("Thanks for the feedback, {0}! We will contact you shortly.", input.Name);
            return RedirectToAction("Index");
        }

        public ActionResult TestRadio()
        {
            List<aTest> list = new List<aTest>();
            list.Add(new aTest() { ID = 1, Name = "test1" });
            list.Add(new aTest() { ID = 2, Name = "test2" });
            SelectList sl = new SelectList(list, "ID", "Name");

            var model = new RadioModel();
            model.TestRadioList = sl;

            //model.TestRadio = "2";  // Set a default value for the first radio button

            return View(model);
        }

        public ActionResult GetDogListJson()
        {
            List<Models.Dog> dogs = new List<Models.Dog>()
			{
				new Models.Dog {ID = 1, Name = "Mardy", Age = 13, Gender = "Female", Handedness = "None", SpayedNeutered=true, Notes="Beautiful Irish Setter."},
				new Models.Dog {ID = 2, Name = "Izzi", Age = 9, Gender = "Female", Handedness = "Left", SpayedNeutered=true, Notes="Karelian Bear Dog, but not trained for field work."},
				new Models.Dog {ID = 3, Name = "Jewel", Age = 10, Gender = "Female", Handedness = "None", SpayedNeutered=true, Notes="Basenji/Doberman mix with short hair. Why isn't she in Africa where it is warm?"},
				new Models.Dog {ID = 4, Name = "Copper", Age = 3, Gender = "Male", Handedness = "None", SpayedNeutered=true},
				new Models.Dog {ID = 5, Name = "Onyx", Age = 4, Gender = "Female", Handedness = "None", SpayedNeutered=true, Notes="Underweight, suffering from a severe bowel disorder."},
				new Models.Dog {ID = 6, Name = "Raja", Age = 14, Gender = "Female", Handedness = "Right", SpayedNeutered=true, Notes="Older than we first thought, but still loves to run."}
			};
            return Json(dogs, JsonRequestBehavior.AllowGet);
        }
    }
}
