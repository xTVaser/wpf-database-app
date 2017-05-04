namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Listing")]
    public partial class Listing
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Listing()
        {
            Features = new HashSet<Feature>();
            Offers = new HashSet<Offer>();
        }

        public int id { get; set; }

        public int address_id { get; set; }

        public int seller_id { get; set; }

        [Column(TypeName = "money")]
        public decimal asking_price { get; set; }

        public byte num_bedrooms { get; set; }

        public byte num_bathrooms { get; set; }

        public byte num_stories { get; set; }

        public bool has_garage { get; set; }

        [Column(TypeName = "date")]
        public DateTime? year_built { get; set; }

        public float? square_footage { get; set; }

        public float? lot_size { get; set; }

        [Column(TypeName = "image")]
        public byte[] display_picture { get; set; }

        public DateTime date_listed { get; set; }

        public virtual Client Client { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feature> Features { get; set; }

        public virtual StreetAddress StreetAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
