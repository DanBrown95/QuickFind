using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickFind.Areas.FBI.Repositories;

namespace QuickFind.Areas.FBI.Controllers
{
    [Area("fbi")]
    public class PopulateController : Controller
    {
        public async Task<IActionResult> List()
        {
            var repo = new FBIRepository();
            var children = await repo.PopulateAsync();
            return View(children);
        }
    }
}