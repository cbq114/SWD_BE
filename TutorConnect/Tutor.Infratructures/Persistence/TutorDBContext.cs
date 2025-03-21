using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Persistence
{
    public class TutorDBContext : DbContext
    {
        public TutorDBContext()
        {
        }

        public TutorDBContext(DbContextOptions<TutorDBContext> options) : base(options)
        {
        }

        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Certifications> Certifications { get; set; }
        public DbSet<FavoriteInstructorDetails> FavoriteInstructorDetails { get; set; }
        public DbSet<FavoriteInstructors> FavoriteInstructors { get; set; }
        public DbSet<Feedbacks> Feedbacks { get; set; }
        public DbSet<LessonAttendances> LessonAttendances { get; set; }
        public DbSet<Lessons> Lessons { get; set; }
        public DbSet<MessageContents> MessageContents { get; set; }
        public DbSet<MessageRoomMember> MessageRoomMember { get; set; }
        public DbSet<MessageRooms> MessageRooms { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<PayoutResponses> PayoutResponses { get; set; }
        public DbSet<Payouts> Payouts { get; set; }
        public DbSet<PromotionUsage> PromotionUsage { get; set; }
        public DbSet<Promotions> Promotions { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<Refunds> Refunds { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Languagues> Languagues { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<TutorAvailabilities> TutorAvailabilities { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<UpgradeRequest> upgradeRequests { get; set; }
        public DbSet<LessonAttendanceDetails> LessonAttendanceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserName);
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Profile)
                      .WithOne(p => p.User)
                      .HasForeignKey<Profile>(p => p.UserName);

                entity.HasOne(e => e.Wallet)
                      .WithOne(w => w.User)
                      .HasForeignKey<Wallet>(w => w.UserName);
            });

            // Profile
            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.ProfileId);
                entity.Property(e => e.Price)
                      .HasPrecision(18, 2);

                entity.HasOne(e => e.Subject)
                      .WithMany()
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
            });

            // Wallet
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.WalletId);
            });

            // Transactions
            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.HasOne(t => t.Wallet)
                      .WithMany(w => w.Transactions)
                      .HasForeignKey(t => t.walletId);
            });

            // Lessons
            modelBuilder.Entity<Lessons>(entity =>
            {
                entity.HasKey(e => e.LessonId);

                entity.HasOne(l => l.User)
                      .WithMany(u => u.Lessons)
                      .HasForeignKey(l => l.Instructor);

                // Thiết lập quan hệ bài học tiên quyết
                entity.HasOne(l => l.ParentLesson)
                      .WithMany(l => l.PrerequisiteLessons)
                      .HasForeignKey(l => l.ParentLessonId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // Bookings
            modelBuilder.Entity<Bookings>(entity =>
            {
                entity.HasKey(e => e.BookingId);
                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bookings)
                      .HasForeignKey(b => b.customer)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Lesson)
                      .WithMany(l => l.Bookings)
                      .HasForeignKey(b => b.LessonId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.TutorAvailability)
                      .WithMany(t => t.Bookings)
                      .HasForeignKey(b => b.AvailabilityId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // TutorAvailabilities
            modelBuilder.Entity<TutorAvailabilities>(entity =>
            {
                entity.HasKey(e => e.TutorAvailabilityId);
                entity.HasOne(t => t.User)
                      .WithMany(u => u.TutorAvailabilities)
                      .HasForeignKey(t => t.Instructor);
            });

            // Certifications
            modelBuilder.Entity<Certifications>(entity =>
            {
                entity.HasKey(e => e.CertificationId);
                entity.HasOne(c => c.profile)
                      .WithMany(p => p.Certifications)
                      .HasForeignKey(c => c.ProfileId);
            });

            // Subjects
            modelBuilder.Entity<Languagues>(entity =>
            {
                entity.HasKey(e => e.LanguageId);
                entity.HasOne(s => s.User)
                      .WithMany(u => u.Languagues)
                      .HasForeignKey(s => s.UserName);
            });

            // Promotions
            modelBuilder.Entity<Promotions>(entity =>
            {
                entity.HasKey(e => e.promotionId);
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Promotions)
                      .HasForeignKey(p => p.Instructor);

                entity.HasOne(p => p.Lesson)
                    .WithMany(l => l.Promotions)
                    .HasForeignKey(p => p.LessonId)
                    .OnDelete(DeleteBehavior.Restrict); // khong cho xoa lesson neu co promotion


                entity.Property(e => e.Discount)
                      .HasPrecision(18, 2);
            });

            // PromotionUsage
            modelBuilder.Entity<PromotionUsage>(entity =>
            {
                entity.HasKey(e => new { e.promotionId, e.bookingId });
                entity.HasOne(pu => pu.promotion)
                      .WithMany(p => p.PromotionUsages)
                      .HasForeignKey(pu => pu.promotionId);

                entity.HasOne(pu => pu.booking)
                      .WithMany(b => b.PromotionUsages)
                      .HasForeignKey(pu => pu.bookingId);
                entity.Property(e => e.DiscountAmount)
                      .HasPrecision(18, 2);
            });

            // Feedbacks
            modelBuilder.Entity<Feedbacks>(entity =>
            {
                entity.HasKey(e => e.FeedbackId);
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Feedbacks)
                      .HasForeignKey(f => f.username);

                entity.HasOne(f => f.Booking)
                      .WithMany(b => b.Feedbacks)
                      .HasForeignKey(f => f.BookingId);
            });

            // LessonAttendances
            modelBuilder.Entity<LessonAttendances>(entity =>
            {
                entity.HasKey(e => e.LessonAttendanceId);
                entity.HasOne(la => la.Booking)
                      .WithMany(b => b.LessonAttendances)
                      .HasForeignKey(la => la.BookingId);
            });
            // MessageRooms
            modelBuilder.Entity<MessageRooms>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(mr => mr.Creator)
                      .WithMany(u => u.MessageRooms)
                      .HasForeignKey(mr => mr.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh xóa user sẽ xóa hết phòng chat
            });

            // MessageRoomMember
            modelBuilder.Entity<MessageRoomMember>(entity =>
            {
                entity.HasKey(e => new { e.Username, e.MessageRoomId });

                entity.HasOne(mrm => mrm.User)
                      .WithMany(u => u.MessageRoomMembers)
                      .HasForeignKey(mrm => mrm.Username)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh cascade delete gây vòng lặp

                entity.HasOne(mrm => mrm.MessageRoom)
                      .WithMany(mr => mr.Members)
                      .HasForeignKey(mrm => mrm.MessageRoomId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa phòng chat thì xóa luôn member
            });

            // MessageContents
            modelBuilder.Entity<MessageContents>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(mc => mc.User)
                      .WithMany(u => u.MessageContents)
                      .HasForeignKey(mc => mc.Username)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh xóa user sẽ xóa hết tin nhắn

                entity.HasOne(mc => mc.MessageRoom)
                      .WithMany(mr => mr.Messages)
                      .HasForeignKey(mc => mc.MessageRoomId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa phòng chat thì xóa luôn tin nhắn
            });

            // Payments
            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.HasOne(p => p.Booking)
                      .WithMany(b => b.Payments)
                      .HasForeignKey(p => p.BookingId);
            });

            // Refunds
            modelBuilder.Entity<Refunds>(entity =>
            {
                entity.HasKey(e => e.RefundId);
                entity.HasOne(r => r.Booking)
                      .WithMany(b => b.Refunds)
                      .HasForeignKey(r => r.BookingId);

                entity.Property(e => e.Amount)
                      .HasPrecision(18, 2);
            });

            // Payouts
            modelBuilder.Entity<Payouts>(entity =>
            {
                entity.HasKey(e => e.PayoutId);
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Payouts)
                      .HasForeignKey(p => p.UserName);
            });

            // PayoutResponses
            modelBuilder.Entity<PayoutResponses>(entity =>
            {
                entity.HasKey(e => e.PayoutResponseId);
                entity.HasOne(pr => pr.Payout)
                      .WithMany(p => p.PayoutResponses)
                      .HasForeignKey(pr => pr.PayoutId);
            });

            // FavoriteInstructors
            modelBuilder.Entity<FavoriteInstructors>(entity =>
            {
                entity.HasKey(e => e.FavoriteInstructorId);
                entity.HasOne(fi => fi.User)
                      .WithOne(u => u.FavoriteInstructors)
                      .HasForeignKey<FavoriteInstructors>(fi => fi.UserName)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // FavoriteInstructorDetails configuration
            modelBuilder.Entity<FavoriteInstructorDetails>(entity =>
            {
                entity.HasKey(e => new { e.FavoriteInstructorId, e.tutor });

                entity.HasOne(fid => fid.user)
                      .WithMany()
                      .HasForeignKey(fid => fid.tutor)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fid => fid.FavoriteInstructor)
                      .WithMany(fi => fi.FavoriteInstructorDetails)
                      .HasForeignKey(fid => fid.FavoriteInstructorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UpgradeRequest
            modelBuilder.Entity<UpgradeRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status)
                      .HasDefaultValue(RequestStatus.Pending);
                entity.Property(e => e.RequestedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
            modelBuilder.Entity<UpgradeRequest>()
                    .HasOne(ur => ur.User)
                    .WithMany(u => u.UpgradeRequests)
                    .HasForeignKey(ur => ur.UserName)
                    .OnDelete(DeleteBehavior.Restrict);
            // LessonAttendanceDetails
            modelBuilder.Entity<LessonAttendanceDetails>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(lad => lad.LessonAttendance)
                      .WithMany(la => la.AttendanceDetails)
                      .HasForeignKey(lad => lad.LessonAttendanceId);

                entity.HasOne(lad => lad.User)
                      .WithMany(u => u.AttendanceDetails)
                      .HasForeignKey(lad => lad.ParticipantUsername);
            });
        }
    }
}
