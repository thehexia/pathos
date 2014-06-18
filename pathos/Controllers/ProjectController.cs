using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pathos.Models;

namespace pathos.Controllers
{ 
    public class ProjectController : Controller
    {
        private ProjectsDBContext db = new ProjectsDBContext();
        private OwnershipChecker ownerCheck = new OwnershipChecker();
        //
        // GET: /Project/
        [Authorize]
        public ViewResult Index()
        {
            var username = User.Identity.Name;
            var myprojects = from Projects in db.Projects
                             where Projects.Author == username
                             select Projects;

            return View(myprojects.ToList());
        }

        //for viewing work related to a profile
        [Authorize]
        public PartialViewResult Portfolio(string username)
        {
            var projects = from Projects in db.Projects
                           where Projects.Author == username
                           select Projects;

            return PartialView(projects.ToList());
        }

        //
        // GET: /Project/Details/5
        [Authorize]
        public ViewResult Details(int id)
        {
            Project project = db.Projects.Find(id);
            return View(project);
        }

        //
        // GET: /Project/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Project/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Project project)
        {
            project.Author = User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(project);
        }
        
        //
        // GET: /Project/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            //confirm the project has the same author as the user
            if (!ownerCheck.IsValidProjectOwner(User.Identity.Name, id))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own this project." });
            }

            Project project = db.Projects.Find(id);

            //list of genres
            ViewBag.Genres = new SelectList(Enum.GetValues(typeof(Genres)).Cast<Genres>());

            return View(project);
        }

        //
        // POST: /Project/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Project project)
        {
            //confirm the project has the same author as the user
            if (!ownerCheck.IsValidProjectOwner(User.Identity.Name, project.ProjectID))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own this project." });
            }

            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        //
        // GET: /Project/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            //confirm the project has the same author as the user
            if (!ownerCheck.IsValidProjectOwner(User.Identity.Name, id))
            {
                return RedirectToAction("Error", new { projectID = -1, errorMsg = "You do not own this project." });
            }

            Project project = db.Projects.Find(id);
            return View(project);
        }

        //
        // POST: /Project/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Error(int projectID, string errorMsg)
        {
            ViewBag.PreviousID = projectID;
            ViewBag.ErrorMsg = errorMsg;
            return View();
        }
    }
}