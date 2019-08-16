using MCP_WEB.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MCP_WEB.Models;

namespace MCP_WEB.Helper
{
   public static class LogPrint
    {
        public static bool Log_Print(string RowNumber,string string_process, string token, string username, NittanDBcontext _context)
        {            
            var Log_Select_Print = _context.Log_Select_Print.Where(w => w.opt == string_process);

            var flag = false;
            if (RowNumber != null)
            {
                flag = true;
                if (Log_Select_Print.Count() > 0)
                {
                    var oldipaddress = Log_Select_Print.Where(w => w.opt == string_process && w.username == username);
                    if (oldipaddress.Count() > 0)
                    {
                        var pp_update = _context.Log_Select_Print.FirstOrDefault(f => f.opt == string_process && f.username == username);
                        pp_update.name = RowNumber;
                        pp_update.token = token;
                        pp_update.username = username;
                        _context.Log_Select_Print.Update(pp_update);
                    }
                    else
                    {
                        var pp_insert = new Log_Select_Print
                        {
                            opt = string_process,
                            name = RowNumber,
                            token = token,
                            username = username
                        };
                        _context.Log_Select_Print.Add(pp_insert);

                    }
                    _context.SaveChanges();
                }
                else
                {
                    var pp_insert = new Log_Select_Print
                    {
                        opt = string_process,
                        name = RowNumber,
                        token = token,
                        username = username
                    };
                    _context.Log_Select_Print.Add(pp_insert);
                    _context.SaveChanges();
                }
            }
            return flag;
        }

    }
}
