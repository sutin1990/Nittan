﻿using DevExtreme.AspNet.Data;
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
    public class GridDCWIPbyProcessHController : Controller
    {
        private NittanDBcontext _context;
        public GridDCWIPbyProcessHController(NittanDBcontext context)
        {
            this._context = context;
        }        

        [HttpGet]
        public IActionResult Get_m_Processmaster(DataSourceLoadOptions loadOptions)
        {
            var pm = _context.m_ProcessMaster.Where(w=>w.SysemProcessFLag=="Y");
            return Json(DataSourceLoader.Load(pm, loadOptions));
        }

        [HttpGet]
        public IActionResult Get_FCode_WeeklyPlan(DataSourceLoadOptions loadOptions)
        {
            var fc = _context.WeeklyPlan.Select(s=>new {s.Fcode,s.Model}).Distinct();
            return Json(DataSourceLoader.Load(fc, loadOptions));
        }

        [HttpGet]
        public IActionResult Get_Material_m_Resource(DataSourceLoadOptions loadOptions)
        {
            var mt = _context.m_Resource.Where(w=>w.ItemType=="R").Distinct();
            return Json(DataSourceLoader.Load(mt, loadOptions));
        }        

        [HttpPost]
        public IActionResult Filter(DataSourceLoadOptions loadOptions,string fromdate, string todate,string Process)
        {            
            DataTable dt = new DataTable();
            var msgj = "";
            if(fromdate == null) { fromdate = "2019-03-1"; }
            if(todate == null) { todate = ""; }
            if(Process == null) { Process = ""; }
            
            try
            {
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "m_sp_rpt010_DCWIPbyProcessH";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.NVarChar) { Value = fromdate });
                    cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.NVarChar) { Value = todate });
                    cmd.Parameters.Add(new SqlParameter("@ProcessCode", SqlDbType.NVarChar) { Value = Process});                   

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
        }

        [HttpPost]
        public IActionResult Print(string RowNumber,string fromdate, string todate)
        {
            string token = Request.HttpContext.Session.Id.ToString();
            string username = Request.HttpContext.User.Claims.FirstOrDefault().Value.ToString();

            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ip = heserver.AddressList[1].ToString();
            IPHostEntry heserver1 = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = token;

            //var Log_Select_Print = _context.Log_Select_Print.Where(w => w.opt == "PrintDCWIPbyProcessHistory");

            var flag = false;
            flag = LogPrint.Log_Print(RowNumber, "PrintDCWIPbyProcessHistory", token, username, _context);

            return Json(flag);
        }
    }
}