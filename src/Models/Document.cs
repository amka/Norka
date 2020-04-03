using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Muon.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
    }
}