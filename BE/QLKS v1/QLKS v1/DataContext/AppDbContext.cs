using Microsoft.EntityFrameworkCore;
using QLKS_v1.Entities;
using System;
using System.Data;

namespace QLKS_v1.DataContext
{
    public class AppDbContext:DbContext
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ComfirmEmail> comfirmEmails { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UserStatus> usersStatus { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Bill> bills { get; set; }
        public DbSet<BillService> billServices { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<CustomerFeedback> customerFeedbacks { get; set; }
        public DbSet<Equipment> equipments { get; set; }
        public DbSet<New> news { get; set; }
        public DbSet<Promotion> promotions { get; set; }
        public DbSet<Room> rooms { get; set; }
        public DbSet<RoomImage> roomImages { get; set; }
        public DbSet<RoomType> roomTypes { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<ServiceType> serviceTypes { get; set; }
        public DbSet<StaffShift> staffShifts { get; set; }
        public DbSet<RoomBookingBill> roomBookingBills { get; set; }
        public DbSet<RoomNameBill> roomNameBills { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=KEN\\KEN; database = KS_v3; integrated security = sspi; encrypt = true; trustservercertificate = true; MultipleActiveResultSets=true");
        }


    }
}
