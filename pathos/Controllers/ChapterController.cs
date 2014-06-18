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
        private OwnershipChecker ownerCheck = new OwnershipChecker();

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

            var projectName = (from Projects in db.Projects
                               where Projects.ProjectID == id && Projects.Author == User.Identity.Name
                               select Projects).FirstOrDefault();


            ViewBag.ProjectID = id;

            if (projectName != null)
                ViewBag.Title = projectName.Title;
            else
                ViewBag.Title = "No such project exists.";

            return View(chapters.ToList());
        }

        //used for non-owners to preview the contents of a project
        [Authorize]
        public PartialViewResult ChapterCatalog(int projectID)
        {
            var chapters = from Chapters in db.Chapters
                           where Chapters.ProjectID == projectID
                           select Chapters;

            return PartialView(chapters.ToList());
        }

        //used alongside edit and create to upload pdfs for chapters
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        //upload file with specific name
        //used specifically in cases where user wants to edit and add a new file
        // not to be exposed to users
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        private void UploadChapter(string pathname, HttpPostedFileBase file)
        {
            var username = User.Identity.Name;
            string dirname = "";

            if (file != null && file.ContentLength > 0)
            {
                dirname = "~/UserContent/" + username + "/";
                //check if the folder exists. if not make it.
                bool dirExists = System.IO.Directory.Exists(Server.MapPath(dirname));
                if (!dirExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(dirname));
                }
                //then save the file after the directory is made
                file.SaveAs(Server.MapPath(pathname));
            }
        }

        //
        // GET: /Chapter/Details/5
        // we dont care if no-owners can see details about a chapter
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(Chapter chapter, HttpPostedFileBase file)
        {
            //validate project ID
            //confirm the chapter has the project ID of a project belonging to the user
            if (!ownerCheck.IsValidProjectOwner(User.Identity.Name, chapter.ProjectID) || chapter.ProjectID < 0)
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own the project you're trying to create a chapter for." });
            }

            //check if a file has been uploaded. if not redirect to error
            if(file == null) 
            {
                return RedirectToAction("Error", new { projectID = chapter.ProjectID, errorMsg = "You must upload a file with the chapter." });
            }

            //double check author
            chapter.Author = User.Identity.Name;

            //re-write LastModified
            chapter.LastModified = DateTime.Now;

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
            //confirm the chapter has the same author as the user
            if (!ownerCheck.IsValidChapterOwner(User.Identity.Name, id))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own this chapter." });
            }

            Chapter chapter = db.Chapters.Find(id);

            try
            {
                var projectList = from Projects in db.Projects
                                  where Projects.Author == User.Identity.Name
                                  select Projects;

                ViewBag.ProjectID = new SelectList(projectList, "ProjectID", "Title", chapter.ProjectID);
            }
            catch (Exception ex) { /*ignore*/ }

            return View(chapter);
        }

        //
        // POST: /Chapter/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Chapter chapter, HttpPostedFileBase file)
        {
            //Chapter old = db.Chapters.Find(chapter.ChapterID);
            
            //confirm the chapter has the project ID of a project belonging to the user
            if(!ownerCheck.IsValidChapterOwner(User.Identity.Name, chapter.ChapterID))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own the project you're trying to edit a chapter for." });
            }

            //re-write LastModified
            chapter.LastModified = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Chapters.Attach(chapter);
                var entry = db.Entry(chapter);
                entry.State = EntityState.Modified;

                entry.Property(e => e.Author).IsModified = false;
                entry.Property(e => e.Location).IsModified = false;
                
                db.SaveChanges();

                //if successful then upload new file
                UploadChapter(chapter.Location, file);

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
            if (!ownerCheck.IsValidChapterOwner(User.Identity.Name, id))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own the chapter you're trying to delete." });
            }

            Chapter chapter = db.Chapters.Find(id);
            return View(chapter);
        }

        //
        // POST: /Chapter/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            Chapter chapter = db.Chapters.Find(id);

            Console.WriteLine(chapter.Location);

            if (System.IO.File.Exists(Server.MapPath(chapter.Location)))
            {
                //delete the file off the server
                System.IO.File.Delete(Server.MapPath(chapter.Location));
            }

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

        [Authorize]
        public ActionResult Reader()
        {
            return View();
        }

        
        //
        // TODO: Implement transaction check for ownership
        //
        [Authorize]
        private bool OwnsCopy(int id)
        {
            if(ownerCheck.IsValidChapterOwner(User.Identity.Name, id))
                return true;
            
            //check for ownership in DB

            //if we get here then all conditions failed
            return false;
        }

        [Authorize]
        public FileResult DisplayPDF(string path, int id)
        {
            if (OwnsCopy(id) && System.IO.File.Exists(Server.MapPath(path)))
            {
                return File(Server.MapPath(path), "application/pdf");
            }
            else
            {
                return File(Server.MapPath("~/UserContent/null.pdf"), "application/pdf");
            }
        }
    }
}