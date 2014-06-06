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

        public ActionResult Search(string query)
        {
            //search title for query

            //search author name for query

            //search description for query

            return View();
        }

        public List<Project> SearchByTitle(string query)
        {
            return new List<Project>();
        }

        public List<Project> SearchByDescription(string query)
        {
            return new List<Project>();
        }

        public List<Project> SearchByAuthor(string query)
        {
            return new List<Project>();
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