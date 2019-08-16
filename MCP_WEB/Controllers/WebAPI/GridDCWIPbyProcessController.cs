using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using MCP_WEB.Data;
using MCP_WEB.Helper;
using MCP_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace MCP_WEB.Controllers.WebAPI
{
    //[Route("api/[controller]")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    //[ApiController]
    public class GridDCWIPbyProcessController : Controller
    {
        private NittanDBcontext _context;
        public GridDCWIPbyProcessController(NittanDBcontext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var pdr = _context.VW_MFC_ProductionDailyReport1;
            return Json(DataSourceLoader.Load(pdr, loadOptions));
        }

        [HttpGet]
        public IActionResult Get_m_MachineMaster(DataSourceLoadOptions loadOptions)
        {
            var mm = _context.m_MachineMaster;
            return Json(DataSourceLoader.Load(mm, loadOptions));
        }

        [HttpGet]
        public IActionResult Get_m_Processmaster(DataSourceLoadOptions loadOptions)
        {
            var pm = _context.m_ProcessMaster.Where(w => w.SysemProcessFLag == "Y");
            return Json(DataSourceLoader.Load(pm, loadOptions));
        }

        [HttpPost]
        public IActionResult Filter(DataSourceLoadOptions loadOptions,string Process)
        {
           
            if (Process == null) { Process = ""; }
            
            DataTable dt = new DataTable();
            var msgj = "";

            try
            {
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "m_sp_rpt009_DCWIPbyProcess";
                    cmd.CommandType = CommandType.StoredProcedure;                    
                    cmd.Parameters.Add(new SqlParameter("@ProcessCode", SqlDbType.NVarChar) { Value = Process });
                   

                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }

                    var DbReader = cmd.ExecuteReader();

                    if (DbReader.HasRows)
                    {
                        dt.Load(DbReader);

                        if (dt.Columns.Count > 1)
                        {
                            dt.Columns.Add("SqlStatus", typeof(System.String));
                            dt.Columns.Add("SqlErrtext", typeof(System.String));
                            foreach (DataRow dr in dt.Select())
                            {
                                dr["SqlStatus"] = "Success";
                                dr["SqlErrtext"] = "";
                            }
                        }
                        else
                        {
                            dt.Columns.Add("SqlStatus", typeof(System.String));
                            dt.Columns.Add("SqlErrtext", typeof(System.String));
                            foreach (DataRow dr in dt.Select())
                            {
                                dr["SqlStatus"] = "NoData";
                                dr["SqlErrtext"] = "";
                            }
                        }

                    }
                    cmd.Connection.Close();
                }
            }
            catch (SqlException ex)
            {
                msgj = ex.Message;
                dt.Columns.Add("SqlStatus", typeof(System.String));
                dt.Columns.Add("SqlErrtext", typeof(System.String));
                foreach (DataRow dr in dt.Select())
                {
                    dr["SqlStatus"] = "Error";
                    dr["SqlErrtext"] = msgj;
                }

            }          

            return Json(dt);
            //return Json(DataSourceLoader.Load(mm, loadOptions));
        }

        [HttpPost]
        public IActionResult Print(string RowNumber)
        {
            string token = Request.HttpContext.Session.Id.ToString();
            string username = Request.HttpContext.User.Claims.FirstOrDefault().Value.ToString();

            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ip = heserver.AddressList[1].ToString();
            IPHostEntry heserver1 = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = token;

            var Log_Select_Print = _context.Log_Select_Print.Where(w => w.opt == "PrintDCWIPbyProcessRealtime");

            var flag = false;
            flag = LogPrint.Log_Print(RowNumber, "PrintDCWIPbyProcessRealtime", token, username, _context);
            return Json(flag);
        }
    }
}