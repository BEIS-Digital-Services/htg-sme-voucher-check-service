using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoucherCheckMicroService.common
{

    public class CanGenerateTestVoucherAttribute : ActionFilterAttribute
    {
        //private static bool? _canShowVouherGenerationEndpoint = null;

        //public static void Reset()
        //{
        //    // this is only needed for the unit tests :(
        //    _canShowVouherGenerationEndpoint = null;
        //}
        private bool canShowVouherGenerationEndpoint
        {
            get
            {
                bool value;
                return (bool.TryParse(Environment.GetEnvironmentVariable("SHOW_GENERATE_TEST_VOUCHER_ENDPOINT") ?? "false", out value) && value);         
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!canShowVouherGenerationEndpoint)
            {
                filterContext.Result = new NotFoundResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
