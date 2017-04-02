namespace RealEstateApp.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StreetAddress
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StreetAddress()
        {
            listings = new HashSet<Listing>();
            offices = new HashSet<Office>();
        }

        public int id { get; set; }

        public int address_num { get; set; }

        [Required]
        [StringLength(255)]
        public string street { get; set; }

        [Required]
        [StringLength(25)]
        public string street_type { get; set; }

        [Required]
        [StringLength(2)]
        public string province_short { get; set; }

        [Required]
        [StringLength(6)]
        public string postal_code { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Listing> listings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Office> offices { get; set; }
    }
}