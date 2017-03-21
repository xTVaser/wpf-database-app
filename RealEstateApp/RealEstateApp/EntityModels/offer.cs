namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("offer")]
    public partial class offer
    {
        public int id { get; set; }

        public int client_id { get; set; }

        public int listing_id { get; set; }

        [Column(TypeName = "money")]
        public decimal amount { get; set; }

        public DateTime date_offered { get; set; }

        public virtual client client { get; set; }

        public virtual listing listing { get; set; }
    }
}
