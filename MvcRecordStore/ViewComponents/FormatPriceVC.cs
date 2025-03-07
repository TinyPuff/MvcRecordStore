using Microsoft.AspNetCore.Mvc;
using MvcRecordStore.Models.ViewModels;

namespace MvcRecordStore.ViewComponents;

public class FormatPriceVC : ViewComponent
{
    public IViewComponentResult Invoke(int index)
    {
        var model = new FormatPriceVM();
        ViewBag.Index = index;
        return View(model);
    }
}