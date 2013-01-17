﻿using System.Linq;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http.Routing;
using WebApiDemo.Models;

namespace WebApiDemo
{
    public class LinkBuilderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var response = actionExecutedContext.Response;
            var urlHelper = actionExecutedContext.ActionContext.ControllerContext.Url;

            Tag tag = null;
            Drill drill = null;
            HttpResourceList<Tag> tags = null;
            HttpResourceList<Drill> tagDrills = null;

            if (response.TryGetContentValue(out tag))
            {
                BuildTagLinks(urlHelper, tag);
            }
            else if (response.TryGetContentValue(out drill))
            {
                BuildDrillLinks(urlHelper, drill);
            }
            else if (response.TryGetContentValue(out tags))
            {
                tags.SelfLink = urlHelper.ApiLink("tags").ToString();
                tags.Items.ForEach(t => BuildTagLinks(urlHelper, t));
            }
            else if (response.TryGetContentValue(out tagDrills))
            {
                tagDrills.SelfLink = urlHelper.ApiLink(tagDrills.Items.First().Id, "tagdrills", "ChildApiRoute").ToString();
                tagDrills.Links.Add(new Link { LinkName = "add-drill", Href = urlHelper.ApiLink("drills").ToString() });
                tagDrills.Items.ForEach(d => BuildDrillLinks(urlHelper, d));
            }
        }

        private static void BuildDrillLinks(UrlHelper urlHelper, Drill drill)
        {
            drill.SelfLink = urlHelper.ApiLink(drill.Id, "drills").ToString();
        }

        private static void BuildTagLinks(UrlHelper urlHelper, Tag tag)
        {
            tag.SelfLink = urlHelper.ApiLink(tag.Id, "tags").ToString();
            tag.Links.Add(new Link { LinkName = "tagdrills", Href = urlHelper.ApiLink(tag.Id, "tagdrills", "ChildApiRoute").ToString() });
        }
    }
}