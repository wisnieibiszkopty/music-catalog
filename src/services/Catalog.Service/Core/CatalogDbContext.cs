using Catalog.Service.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Service.Core;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) {}
    
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
}