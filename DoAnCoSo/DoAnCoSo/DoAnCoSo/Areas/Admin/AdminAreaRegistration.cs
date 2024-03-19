using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using DoAnCoSo;
namespace DoAnCoSo.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {  
            
            get
            {
                return "Admin";
            }
        }
        
        public override void RegisterArea(AreaRegistrationContext context)
        {
         
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "DoAnCoSo.Areas.Admin.Controllers" }

            );
        }
    }
}