using System.Web;
using System.Web.Mvc;

namespace TeachGetTwseStockPrice
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
