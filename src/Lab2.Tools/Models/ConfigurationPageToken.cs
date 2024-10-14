namespace Lab2.Tools.Models;

public record ConfigurationPageToken(long Id)
{
    public static readonly ConfigurationPageToken Empty = new(0);
}