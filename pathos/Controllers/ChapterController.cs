using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using pathos.Models;

namespace pathos.Controllers
{ 
    public class ChapterController : Controller
    {
        private ProjectsDBContext db = new ProjectsDBContext();

        //
        // GET: /Chapter/

        public ActionResult Index()
        {
            var chapters = db.Chapters.Include(c => c.Project);
            return View(chapters.ToList());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var username = User.Identity.Name;
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);
                string pathname = "~/UserContent/" + username + "/";
                //check if the folder exists. if not make it.
                bool exists = System.IO.Directory.Exists(Server.MapPath(pathname));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(pathname));
                }
                //then save the file after the directory is made
                var path = Path.Combine(Server.MapPath("~/UserContent/" + username + "/"), fileName);
                file.SaveAs(path);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadChapter()
        {
            return View();
        }

        //
        // GET: /Chapter/Details/5

        public ViewResult Details(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            return View(chapter);
        }

        //
        // GET: /Chapter/Create

        public ActionResult Create()
        {
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Title");
            return View();
        } 

        //
        // POST: /Chapter/Create

        [HttpPost]
        public ActionResult Create(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Title", chapter.ProjectID);
            return View(chapter);
        }
        
        //
        // GET: /Chapter/Edit/5
 
        public ActionResult Edit(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Title", chapter.ProjectID);
            return View(chapter);
        }

        //
        // POST: /Chapter/Edit/5

        [HttpPost]
        public ActionResult Edit(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chapter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Title", chapter.ProjectID);
            return View(chapter);
        }

        //
        // GET: /Chapter/Delete/5
 
        public ActionResult Delete(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            return View(chapter);
        }

        //
        // POST: /Chapter/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Chapter chapter = db.Chapters.Find(id);
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}