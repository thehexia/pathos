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
                           where Chapters.ProjectID == id && Chapters.Author == User.Identity.Name
                           select Chapters;

            ViewBag.ProjectID = id;
            return View(chapters.ToList());
        }

        [Authorize]
        [HttpPost]
        private string UploadChapter(HttpPostedFileBase file, string filetype)
        {
            var username = User.Identity.Name;
            string pathname = "";
            //append the filetype to the filename
            string filename = Path.GetRandomFileName() + ".pdf";

            if (file != null && file.ContentLength > 0)
            {
                pathname = "~/UserContent/" + username + "/";
                //check if the folder exists. if not make it.
                bool dirExists = System.IO.Directory.Exists(Server.MapPath(pathname));
                if (!dirExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(pathname));
                }
                //check if the filename existed already. this should not happen but it might
                bool fileExists = System.IO.File.Exists(pathname + filename);
                if(fileExists) {
                    return "error";
                }
                //then save the file after the directory is made
                var path = Path.Combine(Server.MapPath("~/UserContent/" + username + "/"), filename);
                file.SaveAs(path);
            }

            return pathname + filename;
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
        public ActionResult Create(int id)
        {
            ViewBag.ProjectID = id;
            return View();
        } 

        //
        // POST: /Chapter/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Chapter chapter, HttpPostedFileBase file)
        {
            //validate project ID
            //confirm the chapter has the project ID of a project belonging to the user
            if (!IsValidProjectOwner(chapter.ProjectID) || chapter.ProjectID < 0)
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own the project you're trying to create a chapter for." });
            }

            //check if a file has been uploaded. if not redirect to error
            if(file == null) 
            {
                return RedirectToAction("Error", new { projectID = chapter.ProjectID, errorMsg = "You must upload a file with the chapter." });
            }

            chapter.Author = User.Identity.Name;
            if (ModelState.IsValid)
            {
                //check for correct file type
                string filetype = Path.GetExtension(file.FileName);
                if ( filetype != ".pdf")
                {
                    return RedirectToAction("Error", new { projectID = chapter.ProjectID, errorMsg = "You may only upload pdf files." } );
                }

                //upload the file
                string filename = chapter.Title + ".pdf";
                chapter.Location = UploadChapter(file, filetype);
                if (chapter.Location == "error")
                {
                    return RedirectToAction("Error", new { projectID = chapter.ProjectID, errorMsg = "Our file upload system has goofed while uploading your file. Please try to upload again." });
                }
                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = chapter.ProjectID });  
            }

            ViewBag.ProjectID = chapter.ProjectID;
            return View(chapter);
        }

        public ActionResult Error(int projectID, string errorMsg)
        {
            ViewBag.PreviousID = projectID;
            ViewBag.ErrorMsg = errorMsg;
            return View();
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
            Chapter old = db.Chapters.Find(chapter.ChapterID);
            
            //confirm the chapter has the project ID of a project belonging to the user
            if(!IsValidProjectOwner(chapter.ProjectID))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own the project you're trying to create a chapter for." });
            }
            if (ModelState.IsValid)
            {
                old.Description = chapter.Description;
                old.Price = chapter.Price;
                old.ProjectID = chapter.ProjectID;
                old.Project = chapter.Project;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = chapter.ProjectID });
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
            return RedirectToAction("Index", new { id = chapter.ProjectID });
        }

        [Authorize]
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //confirms that the project belongs to the user
        [Authorize]
        private bool IsValidProjectOwner(int projectID)
        {
            string username = User.Identity.Name;
            Project project = db.Projects.Find(projectID);
            if (project == null)
            {
                return false;
            }
            if (project.Author == username)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}