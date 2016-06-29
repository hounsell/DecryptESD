using System.Web.Mvc;

namespace CryptoKeySite
{
   public class FilterConfig
   {
      public static void RegisterGlobalFilters(GlobalFilterCollection filters) { filters.Add(new HandleErrorAttribute()); }
   }
}