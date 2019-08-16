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
    public class GridWIPProcessBalanceController : Controller
    {
        private NittanDBcontext _context;
        public GridWIPProcessBalanceController(NittanDBcontext context)
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
        public IActionResult Filter(DataSourceLoadOptions loadOptions,string fromdate, string Process, string machinemaste)
        {
            if (fromdate == null) { fromdate = "201903"; }
            if (Process == null) { Process = ""; }
            if (machinemaste == null) { machinemaste = ""; }
            //fromdate = fromdate.Replace("-","");
            //fromdate = fromdate.Substring(0, 6);
            DataTable dt = new DataTable();
            var msgj = "";

            try
            {
                using (var cmd = _context.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = "m_sp_rpt003_WIPProcessBalance";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@period", SqlDbType.Int) { Value = Convert.ToInt32(fromdate) });
                    cmd.Parameters.Add(new SqlParameter("@ProcessCode", SqlDbType.NVarChar) { Value = Process });
                    cmd.Parameters.Add(new SqlParameter("@MCCode", SqlDbType.NVarChar) { Value = machinemaste });

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
        public IActionResult Print(string RowNumber, string fromdate)
        {
            string token = Request.HttpContext.Session.Id.ToString();
            string username = Request.HttpContext.User.Claims.FirstOrDefault().Value.ToString();           

            var Log_Select_Print = _context.Log_Select_Print.Where(w => w.opt == "PrintWIPProcessBalance");

            var flag = false;
            flag = LogPrint.Log_Print(RowNumber, "PrintWIPProcessBalance", token, username, _context);
           
            return Json(flag);
        }
    }
}