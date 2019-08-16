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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCP_WEB.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VW_MFC_Isonite_SummaryController : Controller
    {
        private readonly NittanDBcontext _context;

        public VW_MFC_Isonite_SummaryController(NittanDBcontext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult GetIsonite_Summary(DataSourceLoadOptions loadOptions)
        {
            var view = _context.VW_MFC_Isonite_Summary.Select(x => new
            {
                x.IsoniteCode,
                x.JigNo,
                x.Column,
                x.Row,
                x.BarCode,
                x.SeriesLot,
                x.Model,
                x.WStatus,
                x.QtyOrder,
                x.WIPLeftQty,
                x.IsoniteQty
            });

            return Json(DataSourceLoader.Load(view, loadOptions));

        }

        [HttpDelete]
        public IActionResult Delete(string IsoniteCode)
        {
            
            if (IsoniteCode != "") {
                var model = _context.Isonite.FirstOrDefault(item => item.IsoniteCode == IsoniteCode);
                _context.Isonite.Remove(model);
                _context.SaveChanges();

                var model_1 = _context.Isonite_Line.Where(item => item.IsoniteCode == IsoniteCode);

                foreach (var r in model_1) {

                    var wo_routing = _context.WoRouting.Where(x => x.ProcessCode == "ISONITE" && x.BarCode == r.BarCode).FirstOrDefault();
                    if(wo_routing != null)
                    {
                        wo_routing.QTYinProcess = 0;
                        _context.SaveChanges();
                    }
                    

                }

                _context.Isonite_Line.RemoveRange(model_1);
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