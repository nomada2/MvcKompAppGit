﻿using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using MvcKompApp.DAL;
using MvcKompApp.Filters;
using MvcKompApp.Infrastructure;
using MvcKompApp.Models;
using MvcKompApp.Validation;
using System;

namespace MvcKompApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new LogonAuthorize());
            filters.Add(new HandleErrorAttribute());
        }

        void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null) return;

            // DO SOMETHING WITH ERROR

            Server.ClearError();

            Response.Redirect("Home/Error");
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                null,
                "Employee/Page{page}",
                new { controller = "Employee", action = "Index", id = UrlParameter.Optional },
                new { page = @"\d+" });

            routes.MapRoute(
                "Employee",
                "Employee/{action}/{id}",
                new { controller = "Employee", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new
                {
                    httpMethod = new HttpMethodConstraint("GET", "POST")
                }
                // Parameter defaults
            );

        }

        protected void Application_Start()
        {
#if DEBUG
            Database.SetInitializer<TaskDBContext>(new TaskDbContextInitializer());
            Database.SetInitializer<SchoolContext>(new SchoolInitializer());
          //  Database.SetInitializer<ImageContext>(new ImageInitializer());
            Database.SetInitializer<MovieContext>(new MovieInitializer());
#endif
            //ModelBinders.Binders.Add(typeof(Appointment), new ValidatingModelBinder());

            //ModelValidatorProviders.Providers.Clear();
            //ModelValidatorProviders.Providers.Add(new CustomValidationProvider());

            //HtmlHelper.ClientValidationEnabled = true;
            //HtmlHelper.UnobtrusiveJavaScriptEnabled = true;

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.RegisterAdapter(
                typeof(RemoteUID_Attribute),
                typeof(RemoteValidator));

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

            ModelBinders.Binders.Add(typeof(Appointment), new ValidatingModelBinder());

        }
    }
}