using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using MCP_WEB.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace MCP_WEB.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ManageUserTransactionController : Controller
    {
        private readonly NittanDBcontext _context;
        public ManageUserTransactionController(NittanDBcontext context)
        {
            this._context = context;
        }
        [HttpGet]
        public IActionResult GetUser(DataSourceLoadOptions loadOptions) {
            var model = from a in _context.UserTransaction join b in _context.m_UserMaster 
                         on a.UserName equals b.UserName where b.UserRoll != "ADMIN" select new {
                             a.ID,a.UserName,a.DateExprie
                         };
            return Json(DataSourceLoader.Load(model, loadOptions));
        }

        [HttpDelete]
        public IActionResult Delete(string options)
        {
            if(options != "") {
                var model = _context.UserTransaction.FirstOrDefault(item => item.ID == Int32.Parse(options));

                _context.UserTransaction.Remove(model);
                _context.SaveChanges();
            }
            

            return Ok();
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}