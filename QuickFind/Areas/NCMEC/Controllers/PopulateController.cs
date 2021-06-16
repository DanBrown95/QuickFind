using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickFind.Areas.NCMEC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace QuickFind.Areas.NCMEC.Controllers
{
    [Area("ncmec")]
    public class PopulateController : Controller
    {
        public async Task<IActionResult> List()
        {
            var repo = new NCMECRepository();
            var children = await repo.PopulateAsync("IA");
            return View(children);
        }
    }
}