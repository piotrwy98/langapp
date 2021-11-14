using LangApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LangApp.WebApi.Api.Repositories.Db
{
    public partial class LangAppContext : DbContext
    {
        public static string ConnectionString { get; set; }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryName> CategoryNames { get; set; }
        public virtual DbSet<FavouriteWord> FavouriteWords { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageName> LanguageNames { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<PartOfSpeech> PartOfSpeeches { get; set; }
        public virtual DbSet<PartOfSpeechName> PartOfSpeechNames { get; set; }
        public virtual DbSet<SelectedCategory> SelectedCategories { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Word> Words { get; set; }

        public LangAppContext(DbContextOptions<LangAppContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.ToTable("answer");

                entity.HasIndex(e => new { e.SessionId, e.NumberInSession }, "session_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CorrectAnswer)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("correct_answer")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.Duration)
                    .HasColumnType("time")
                    .HasColumnName("duration");

                entity.Property(e => e.NumberInSession)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("number_in_session");

                entity.Property(e => e.QuestionType)
                    .IsRequired()
                    .HasColumnType("enum('CLOSED','OPEN','PRONUNCIATION')")
                    .HasColumnName("question_type");

                entity.Property(e => e.SessionId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("session_id");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value")
                    .UseCollation("utf8mb4_bin");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("image_path")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasColumnType("enum('A','B','C')")
                    .HasColumnName("level");
            });

            modelBuilder.Entity<CategoryName>(entity =>
            {
                entity.ToTable("category_name");

                entity.HasIndex(e => e.CategoryId, "category_id");

                entity.HasIndex(e => new { e.LanguageId, e.CategoryId }, "language_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("category_id");

                entity.Property(e => e.LanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("language_id");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value")
                    .UseCollation("utf8mb4_bin");
            });

            modelBuilder.Entity<FavouriteWord>(entity =>
            {
                entity.ToTable("favourite_word");

                entity.HasIndex(e => e.FirstTranslationId, "first_translation_id");

                entity.HasIndex(e => e.SecondTranslationId, "second_translation_id");

                entity.HasIndex(e => new { e.UserId, e.FirstTranslationId, e.SecondTranslationId }, "user_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CreationDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FirstTranslationId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("first_translation_id");

                entity.Property(e => e.SecondTranslationId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("second_translation_id");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("language");

                entity.HasIndex(e => e.Code, "code")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("code");

                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("image_path")
                    .UseCollation("utf8mb4_bin");
            });

            modelBuilder.Entity<LanguageName>(entity =>
            {
                entity.ToTable("language_name");

                entity.HasIndex(e => e.LanguageId, "language_id");

                entity.HasIndex(e => new { e.SourceLanguageId, e.LanguageId }, "source_language_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.LanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("language_id");

                entity.Property(e => e.SourceLanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("source_language_id");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("news");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasColumnName("content");

                entity.Property(e => e.CreationDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("title");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<PartOfSpeech>(entity =>
            {
                entity.ToTable("part_of_speech");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");
            });

            modelBuilder.Entity<PartOfSpeechName>(entity =>
            {
                entity.ToTable("part_of_speech_name");

                entity.HasIndex(e => e.LanguageId, "language_id");

                entity.HasIndex(e => new { e.PartOfSpeechId, e.LanguageId }, "part_of_speech_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.LanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("language_id");

                entity.Property(e => e.PartOfSpeechId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("part_of_speech_id");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<SelectedCategory>(entity =>
            {
                entity.ToTable("selected_category");

                entity.HasIndex(e => e.CategoryId, "category_id");

                entity.HasIndex(e => new { e.SessionId, e.CategoryId }, "session_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("category_id");

                entity.Property(e => e.SessionId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("session_id");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("session");

                entity.HasIndex(e => e.FirstLanguageId, "first_language_id");

                entity.HasIndex(e => e.SecondLanguageId, "second_language_id");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.FirstLanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("first_language_id");

                entity.Property(e => e.SecondLanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("second_language_id");

                entity.Property(e => e.StartDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date_time");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("enum('LEARN','TEST')")
                    .HasColumnName("type");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("translation");

                entity.HasIndex(e => new { e.LanguageId, e.WordId }, "language_id")
                    .IsUnique();

                entity.HasIndex(e => e.WordId, "word_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.LanguageId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("language_id");

                entity.Property(e => e.Phonetic)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("phonetic")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("value")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.WordId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("word_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Email, "email")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "login")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("password")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.RegisterDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("register_date_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnType("enum('USER','ADMIN')")
                    .HasColumnName("role");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("username")
                    .UseCollation("utf8mb4_bin");
            });

            modelBuilder.Entity<Word>(entity =>
            {
                entity.ToTable("word");

                entity.HasIndex(e => e.CategoryId, "category_id");

                entity.HasIndex(e => e.PartOfSpeechId, "part_of_speech_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("category_id");

                entity.Property(e => e.ImagePath)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("image_path")
                    .UseCollation("utf8mb4_bin");

                entity.Property(e => e.PartOfSpeechId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("part_of_speech_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
