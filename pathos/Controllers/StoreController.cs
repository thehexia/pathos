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
    public class StoreController : Controller
    {
        private ProjectsDBContext db = new ProjectsDBContext();

        public ActionResult Index() 
        { 
            return View(); 
        }

        public ActionResult BookCart()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult SearchAll(string type, string query)
        {
            List<Project> result = new List<Project>();

            if (type == "All" || type == "Title")
            {
                //search title for query
                result.AddRange(SearchByTitle(query));
            }
            if (type == "All" || type == "Author")
            {
                //search author name for query
                result.AddRange(SearchByAuthor(query));
            }
            if (type == "All" || type == "Description")
            {
                //search description for query
                result.AddRange(SearchByDescription(query));
            }

            return View(result);
        }

        public List<Project> SearchByTitle(string query)
        {
            query = query.ToLower();
            var result = from Projects in db.Projects
                         where Projects.Title.ToLower().Contains(query)
                         select Projects;

            return result.ToList();
        }

        public List<Project> SearchByDescription(string query)
        {
            query = query.ToLower();
            var result = from Projects in db.Projects
                         where Projects.Description.ToLower().Contains(query)
                         select Projects;

            return result.ToList();
        }

        public List<Project> SearchByAuthor(string query)
        {
            query = query.ToLower();
            var result = from Projects in db.Projects
                         where Projects.Author.ToLower().Contains(query)
                         select Projects;

            return result.ToList();
        }

        //TODO: add genre to projects
        public List<Project> SearchByGenre(string query)
        {
            return new List<Project>();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}