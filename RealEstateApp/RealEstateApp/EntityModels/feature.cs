namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("feature")]
    public partial class Feature
    {
        public int id { get; set; }

        public int listing_id { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string heading { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string body { get; set; }

        public virtual Listing listing { get; set; }
    }
}
