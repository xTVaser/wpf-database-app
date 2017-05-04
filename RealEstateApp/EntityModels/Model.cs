namespace RealEstateApp.EntityModels {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext {
        public Model()
            : base("name=EntityModel") {
        }

        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Commission> Commissions { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<Listing> Listings { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Office> Offices { get; set; }
        public virtual DbSet<StreetAddress> StreetAddresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Administrator>()
                .Property(e => e.salary)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Agent>()
                .Property(e => e.phone_number)
                .IsFixedLength();

            modelBuilder.Entity<Agent>()
                .Property(e => e.commission_balance)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Client>()
                .Property(e => e.client_type)
                .IsFixedLength();

            modelBuilder.Entity<Client>()
                .Property(e => e.phone_number)
                .IsFixedLength();

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Listings)
                .WithRequired(e => e.Client)
                .HasForeignKey(e => e.seller_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.Offers)
                .WithRequired(e => e.Client)
                .HasForeignKey(e => e.client_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Commission>()
                .Property(e => e.amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Employee>()
                .Property(e => e.employee_type)
                .IsFixedLength();

            modelBuilder.Entity<Employee>()
                .Property(e => e.security_question)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.security_answer)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .HasOptional(e => e.Administrator)
                .WithRequired(e => e.Employee);

            modelBuilder.Entity<Employee>()
                .HasOptional(e => e.Agent)
                .WithRequired(e => e.Employee);

            modelBuilder.Entity<Feature>()
                .Property(e => e.heading)
                .IsUnicode(false);

            modelBuilder.Entity<Feature>()
                .Property(e => e.body)
                .IsUnicode(false);

            modelBuilder.Entity<Listing>()
                .Property(e => e.asking_price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Listing>()
                .HasMany(e => e.Features)
                .WithRequired(e => e.Listing)
                .HasForeignKey(e => e.listing_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Listing>()
                .HasMany(e => e.Offers)
                .WithRequired(e => e.Listing)
                .HasForeignKey(e => e.listing_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Offer>()
                .Property(e => e.amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Office>()
                .Property(e => e.phone_number)
                .IsFixedLength();

            modelBuilder.Entity<Office>()
                .Property(e => e.fax_number)
                .IsFixedLength();

            modelBuilder.Entity<Office>()
                .HasMany(e => e.Employees)
                .WithRequired(e => e.Office)
                .HasForeignKey(e => e.office_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StreetAddress>()
                .Property(e => e.province_short)
                .IsFixedLength();

            modelBuilder.Entity<StreetAddress>()
                .Property(e => e.postal_code)
                .IsFixedLength();

            modelBuilder.Entity<StreetAddress>()
                .HasMany(e => e.Listings)
                .WithRequired(e => e.StreetAddress)
                .HasForeignKey(e => e.address_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StreetAddress>()
                .HasMany(e => e.Offices)
                .WithRequired(e => e.StreetAddress)
                .HasForeignKey(e => e.address_id)
                .WillCascadeOnDelete(false);
        }
    }
}
