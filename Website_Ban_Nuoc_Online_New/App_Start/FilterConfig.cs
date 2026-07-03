using System.Web;
using System.Web.Mvc;

namespace Website_Ban_Nuoc_Online_New
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
