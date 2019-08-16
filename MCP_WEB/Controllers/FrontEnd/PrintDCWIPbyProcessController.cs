﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCP_WEB.Data;
using MCP_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MCP_WEB.Controllers.FrontEnd
{
    public class PrintDCWIPbyProcessController : Controller
    {
        private NittanDBcontext _context;
        public PrintDCWIPbyProcessController(NittanDBcontext context)
        {
            _context = context;
        }
       public DCWIPbyProcess DCWIPbyProcess;
        public async Task<ActionResult>Index(string Process)
        {
            string token = Request.HttpContext.Session.Id.ToString();
            string username = Request.HttpContext.User.Claims.FirstOrDefault().Value.ToString();

            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ip = heserver.AddressList[1].ToString();

            if (Process == null) { Process = ""; }
            ViewBag.Process = Process;
            var p = _context.s_GlobalPams.SingleOrDefault(x => x.parm_key == "DateFormat");

            var Log_Select_Print =  _context.Log_Select_Print.Where(w => w.opt == "PrintDCWIPbyProcessRealtime" && w.username == username);
                      
                var logprint = Log_Select_Print.ToList();//เอาชุดข้อมูลของ RowNumber ที่เลือกไปมาwhere contain
                var RowNumbers = logprint[0].name.ToString();
                int [] array = RowNumbers.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

            ViewBag.GlobalDtFormat = p.param_value.ToString();

            var viewbagfromdate = Convert.ToString(DateTime.Now);

            ViewBag.fromdate = Convert.ToDateTime(viewbagfromdate).ToString(p.param_value);

            //ViewBag.fromdate = Convert.ToDateTime(DateTime.Now).ToString(p.param_value);
            //ViewBag.todate = Convert.ToDateTime(PrintFGMovement[0].todate).ToString(p.param_value);

            var list_DCWIPbyProcess = new List<DCWIPbyProcess>();
            DataTable dt = new DataTable();
            var msgj = "";
            int rownumber=0;           
            string model = "";
            string fcode = "";           
            string uom = "";
            string material1 = "";
            int qtyorder = 0;
            decimal wip = 0;
            decimal ng = 0;
            decimal nc = 0;
            decimal wiprate = 0;
            decimal ngrate = 0;
            decimal ncrate = 0;
            string serieslot = "";
            string barcode = "";
            string processcode = "";
            DateTime asof = DateTime.Now ;
            int qtymove = 0;           
         
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
                          
                            foreach (DataRow dr in dt.Select())
                            {

                                if (dr["RowNumber"] != System.DBNull.Value)
                                {
                                    rownumber =  Convert.ToInt32(dr["RowNumber"]);
                                }                                

                                if (dr["Model"] != System.DBNull.Value)
                                {
                                    model = (string)dr["Model"];
                                }

                                if (dr["FCode"] != System.DBNull.Value)
                                {
                                    fcode = (string)dr["FCode"];
                                }

                                if (dr["ProcessCode"] != System.DBNull.Value)
                                {
                                    processcode = (string)dr["ProcessCode"];
                                }

                                if (dr["Barcode"] != System.DBNull.Value)
                                {
                                    barcode = (string)dr["Barcode"];
                                }

                                if (dr["QtyOrder"] != System.DBNull.Value)
                                {
                                    qtyorder = (int)dr["QtyOrder"];
                                }

                                if (dr["WIP"] != System.DBNull.Value)
                                {
                                    wip = (decimal)dr["WIP"];
                                }

                                if (dr["NG"] != System.DBNull.Value)
                                {
                                    ng = (decimal)dr["NG"];
                                }

                                if (dr["NC"] != System.DBNull.Value)
                                {
                                    nc = (decimal)dr["NC"];
                                }

                                if (dr["WIPRate"] != System.DBNull.Value)
                                {
                                    wiprate = (decimal)dr["WIPRate"];
                                }

                                if (dr["NGRate"] != System.DBNull.Value)
                                {
                                    ngrate = (decimal)dr["NGRate"];
                                }

                                if (dr["NCRate"] != System.DBNull.Value)
                                {
                                    ncrate = (decimal)dr["NCRate"];
                                }

                                DCWIPbyProcess = new DCWIPbyProcess
                                {                                    
                                    RowNumber = rownumber,                                                                    
                                    FCode = fcode,
                                    Model = model,                                   
                                    ProcessCode = processcode,
                                    Barcode = barcode,
                                    QtyOrder = qtyorder,
                                    WIP = wip,
                                    NG = ng,
                                    NC = nc,
                                    WIPRate = wiprate,
                                    NGRate = ngrate,
                                    NCRate = ncrate,

                                };

                                list_DCWIPbyProcess.Add(DCWIPbyProcess);
                            }
                        }
                        else
                        {
                           
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
            
           var list = list_DCWIPbyProcess.Where(w => array.Contains(w.RowNumber)).ToList();          
            //Thread.Sleep(5000);
            return View(list);            
        }
    }
}