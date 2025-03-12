// filepath: /d:/Dev/Backend/ASP.Net Core/MvcRecordStore/MvcRecordStore/ViewComponents/FormatPriceVC.cs
using Microsoft.AspNetCore.Mvc;
using MvcRecordStore.Models.ViewModels;

namespace MvcRecordStore.ViewComponents
{
    public class FormatPriceVC : ViewComponent
    {
        public IViewComponentResult Invoke(int index, FormatPriceVM? formatPrice)
        {
            ViewBag.Index = index;
            if (formatPrice != null)
            {   
                return View(formatPrice);
            }

            var model = new FormatPriceVM();
            return View(model);
        }
    }
}