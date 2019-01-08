﻿namespace OData
{
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.AspNet.OData.Routing.Conventions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Routing;
    using System.Collections.Generic;

    public class CustomUrlHelper : UrlHelper
    {
        string _externalAPIAddress;

        public CustomUrlHelper(ActionContext actionContext, string ExternalAPIAddress) : base(actionContext)
        {
            _externalAPIAddress = ExternalAPIAddress;
        }

        public override string Link(string routeName, object values)
        {
            var link = base.Link(routeName, values);

            if (routeName == "odata")
            {
                string ExternalAPIAddress = _externalAPIAddress;
                if (_externalAPIAddress.EndsWith("/"))
                    ExternalAPIAddress = ExternalAPIAddress.Substring(0, ExternalAPIAddress.Length - 1);  // Do not end with slash
                var newLink = link.Replace($"https://{HttpContext.Request.Host}", ExternalAPIAddress);
                return newLink;
            }

            return link;
        }
    }

    internal class DefaultMetadataRoutingConvention : IODataRoutingConvention
    {
        string _externalAPIAddress;
        public DefaultMetadataRoutingConvention(string ExternalAPIAddress)
        {
            _externalAPIAddress = ExternalAPIAddress;
        }
        private MetadataRoutingConvention _defaultRouting = new MetadataRoutingConvention();

        public IEnumerable<ControllerActionDescriptor> SelectAction(RouteContext routeContext)
        {
            var helper = new CustomUrlHelper(new ActionContext(routeContext.HttpContext, routeContext.RouteData, new ActionDescriptor()), _externalAPIAddress);
            routeContext.HttpContext.Request.ODataFeature().UrlHelper = helper;
            return _defaultRouting.SelectAction(routeContext);
        }
    }
}