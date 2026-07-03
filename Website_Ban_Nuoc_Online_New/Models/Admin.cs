using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(120)]
        public string FullName { get; set; }

        [Required, StringLength(255)]
        public string Password { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string CCCD { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}