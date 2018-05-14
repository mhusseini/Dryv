using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DryvDemo.Models
{
    public class MainMenuModel
    {
        public string Action { get; set; }
        public string Area { get; set; }
        public string Color { get; set; }
        public string Controller { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}