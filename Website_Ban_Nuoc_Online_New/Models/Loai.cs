using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Website_Ban_Nuoc_Online.Models
{
    public class Loai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLoai { get; set; }

        [Required, StringLength(100)]
        public string TenLoai { get; set; }

        // Navigation
        public virtual ICollection<Mon> Mons { get; set; }

        public Loai()
        {
            Mons = new HashSet<Mon>();
        }
    }
}