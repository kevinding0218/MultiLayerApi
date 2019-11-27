using Microsoft.EntityFrameworkCore;
using MyMusic.Core.Models;
using MyMusic.Data.Configurations;

namespace MyMusic.Data
{
    public class MyMusicDbContext : DbContext
    {
        /*
        Note that if you do not plan to manage or fetch data individually from a table, 
        you do not necessarily need to add a DbSet for it, 
        Entity Framework will create that table as long as the model has any relation with the other models.
        */
        public DbSet<Music> Musics { get; set; }
        public DbSet<Artist> Artists { get; set; }

        public MyMusicDbContext(DbContextOptions<MyMusicDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .ApplyConfiguration(new MusicConfiguration());

            builder
                .ApplyConfiguration(new ArtistConfiguration());
        }
    }
}