using System.Threading.Tasks;
using QuickFind.Areas.IowaOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace QuickFind.Areas.IowaOnline.Controllers
{
    [Area("iowaonline")]
    public class PopulateController : Controller
    {
        public async Task<IActionResult> List()
        {
            var repo = new IowaOnlineRepository();
            var children = await repo.PopulateAsync();
            return View(children);
        }
    }
}