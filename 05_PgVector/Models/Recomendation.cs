using Pgvector;

namespace _05_PgVector.Models;

public class Recomendation
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Vector Embedding { get; set; } = null!;
}