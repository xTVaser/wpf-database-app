namespace RealEstateApp.EntityModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=Database")
        {
        }

        public virtual DbSet<Administrator> administrators { get; set; }
        public virtual DbSet<Agent> agents { get; set; }
        public virtual DbSet<Client> clients { get; set; }
        public virtual DbSet<Commission> commissions { get; set; }
        public virtual DbSet<Employee> employees { get; set; }
        public virtual DbSet<Feature> features { get; set; }
        public virtual DbSet<Listing> listings { get; set; }
        public virtual DbSet<Offer> offers { get; set; }
        public virtual DbSet<Office> offices { get; set; }
        public virtual DbSet<StreetAddress> street_address { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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
                .HasMany(e => e.listings)
                .WithRequired(e => e.client)
                .HasForeignKey(e => e.seller_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.offers)
                .WithRequired(e => e.client)
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
                .HasOptional(e => e.administrator)
                .WithRequired(e => e.employee);

            modelBuilder.Entity<Employee>()
                .HasOptional(e => e.agent)
                .WithRequired(e => e.employee);

            modelBuilder.Entity<Feature>()
                .Property(e => e.heading)
                .IsUnicode(false);

            modelBuilder.Entity<Feature>()
                .Property(e => e.body)
                .IsUnicode(false);

            modelBuilder.Entity<Listing>()
                .HasMany(e => e.features)
                .WithRequired(e => e.listing)
                .HasForeignKey(e => e.listing_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Listing>()
                .HasMany(e => e.offers)
                .WithRequired(e => e.listing)
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
                .HasMany(e => e.employees)
                .WithRequired(e => e.office)
                .HasForeignKey(e => e.office_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StreetAddress>()
                .Property(e => e.province_short)
                .IsFixedLength();

            modelBuilder.Entity<StreetAddress>()
                .Property(e => e.postal_code)
                .IsFixedLength();

            modelBuilder.Entity<StreetAddress>()
                .HasMany(e => e.listings)
                .WithRequired(e => e.street_address)
                .HasForeignKey(e => e.address_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StreetAddress>()
                .HasMany(e => e.offices)
                .WithRequired(e => e.street_address)
                .HasForeignKey(e => e.address_id)
                .WillCascadeOnDelete(false);
        }
    }
}
