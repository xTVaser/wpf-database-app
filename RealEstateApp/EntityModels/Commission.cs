namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Commission")]
    public partial class Commission
    {
        public int id { get; set; }

        public int payable_to { get; set; }

        [Column(TypeName = "money")]
        public decimal amount { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string reason { get; set; }

        public DateTime date_payed_out { get; set; }
    }
}
