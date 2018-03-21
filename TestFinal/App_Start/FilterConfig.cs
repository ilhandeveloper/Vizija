using System.Web;
using System.Web.Mvc;

namespace TestFinal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //Ukljucivanje filtera za handlovanje gresaka
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }
    }
}
