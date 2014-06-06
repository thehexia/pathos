﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace pathos.Models
{
    enum Genres
    {
        Horror,
        Mystery,
        Sci_Fi,
        Romance,
        Fantasy,
        Poetry,
        Music,
        Satire,
        Comedy,
        Slice_of_Life
    }

    public class Project
    {
        public Project() {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                this.Author = HttpContext.Current.User.Identity.Name;
        }

        [Key]
        public int ProjectID { get; set; }

        [Required]
        [StringLength(140, MinimumLength = 0, ErrorMessage = "You have a maximum of 140 characters for your title.")]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [StringLength(140, MinimumLength = 0, ErrorMessage = "You have a maximum of 140 characters.")]
        public string Description { get; set; }

        public virtual ICollection<Chapter> Chapter { get; set; }
    }

    public class Chapter
    {
        public Chapter()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                this.Author = HttpContext.Current.User.Identity.Name;

            this.Location = "default";
        }

        [HiddenInput]
        [Key] 
        public int ChapterID { get; set; }

        [Required]
        [StringLength(140, MinimumLength = 0, ErrorMessage = "You have a maximum of 140 characters for your title.")]
        public string Title { get; set; }

        [Required]
        public int ProjectID { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(140, MinimumLength = 0, ErrorMessage = "You have a maximum of 140 characters.")]
        public string Description { get; set; }

        public virtual Project Project { get; set; }
    }

    public class ProjectsDBContext : DbContext
    {
        public ProjectsDBContext() : base("ProjectsDBContext") 
        {
        }
        
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}