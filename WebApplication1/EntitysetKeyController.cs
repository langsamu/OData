﻿namespace WebApplication1
{
    using System.Linq.Expressions;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Routing;

    public class EntitysetKeyController : BaseController
    {
        //http://localhost:2933/House('1AFu55Hs')
        //http://localhost:2933/House('1AFu55Hs')?$select=HouseName
        [HttpGet]
        [EnableQuery(AllowedQueryOptions = System.Web.OData.Query.AllowedQueryOptions.Select)]
        public IHttpActionResult Default()
        {
            var path = this.Request.Properties["System.Web.OData.Path"] as ODataPath;
            Expression expression = GenerateExpression(path, Request.RequestUri.Query);
            object result = GenerateODataResult(expression);

            return Ok(result);
        }
        
    }
}
