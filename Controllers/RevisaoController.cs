using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppShowDoMilhao.Controllers
{
    public class RevisaoController : Controller
    {
        // GET: RevisaoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RevisaoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RevisaoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RevisaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RevisaoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RevisaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RevisaoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RevisaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
