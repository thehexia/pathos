using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace pathos.Models
{
    public class Transaction
    {

        //default constructor
        public Transaction()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                this.owner = HttpContext.Current.User.Identity.Name;
            this.DateSold = DateTime.Now;
        }

        public Transaction(Chapter chapter)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                this.owner = HttpContext.Current.User.Identity.Name;

            this.ChapterID = chapter.ChapterID;
            this.price = chapter.Price;
            this.DateSold = DateTime.Now;
        }

        [Key]
        public int TransactionID { get; set; }

        //person who made the transcaction
        [Required]
        public string owner { get; set; }

        //content they purchased
        [Required]
        public int ChapterID { get; set; }

        //price
        [Required]
        public decimal price { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateSold { get; set; }
    }

}