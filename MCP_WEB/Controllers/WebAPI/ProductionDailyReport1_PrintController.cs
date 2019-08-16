using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCP_WEB.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MCP_WEB.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using System.Net;
using MCP_WEB.Helper;

namespace MCP_WEB.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    //[ApiController]
    public class ProductionDailyReport1_PrintController : Controller
    {
        private NittanDBcontext _context;
        public ProductionDailyReport1_PrintController(NittanDBcontext context)
        {
            _context = context;
        }

        private string GetUserName()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var c = claims.FirstOrDefault();

            return c.Value;
        }

        [HttpPost]
        public IActionResult PrintProductionDailyReport1(string RowNumber, string fromdate,string todate)
        {
            string token = Request.HttpContext.Session.Id.ToString();
            string username = Request.HttpContext.User.Claims.FirstOrDefault().Value.ToString();

            var Log_Select_Print = _context.Log_Select_Print.Where(w => w.opt == "PrintProduction1");

            var flag = false;
            flag = LogPrint.Log_Print(RowNumber, "PrintProduction1", token, username, _context);
           
            return Json(flag);
            

        }
    }
}