using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MCP_WEB.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCP_WEB.Controllers.FrontEnd
{
    [Authorize]
    public class UserMasterController : Controller
    {
        private NittanDBcontext _context;
        
        public UserMasterController(NittanDBcontext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var userLogin = claims.FirstOrDefault();
            
            var useradmin = _context.m_UserMaster.Where(f => f.UserName == userLogin.Value && f.UserRoll == "ADMIN").ToList();
            ViewBag.adminstatus = useradmin.Count;

            var p = _context.s_GlobalPams.SingleOrDefault(x => x.parm_key == "DateTimeFormat");
            ViewBag.GlobalDtFormat = p.param_value.ToString();
            return View();
        }
    }
}