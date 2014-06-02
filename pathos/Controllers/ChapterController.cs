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
        // needs project id
        [Authorize]
        public ActionResult Index(int id)
        {
            //var chapters = db.Chapters.Include(c => c.Project);
            //get the chapters included in a project

            var chapters = from Chapters in db.Chapters
                           where Chapters.ProjectID == id
                           select Chapters;

            ViewBag.ProjectID = id;
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
        [Authorize]
        public ActionResult UploadChapter()
        {
            return View();
        }

        //
        // GET: /Chapter/Details/5
        [Authorize]
        public ViewResult Details(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            return View(chapter);
        }

        //
        // GET: /Chapter/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Title");
            return View();
        } 

        //
        // POST: /Chapter/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Chapter chapter)
        {
            chapter.Author = User.Identity.Name;
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            ViewBag.ProjectID = new SelectList(db.Projects, "ProjectID", "Title", chapter.ProjectID);
            return View(chapter);
        }

        //
        // POST: /Chapter/Edit/5
        [Authorize]
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            return View(chapter);
        }

        //
        // POST: /Chapter/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Chapter chapter = db.Chapters.Find(id);
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}