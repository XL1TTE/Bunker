namespace Bunker.Domain.Identity;

public record struct Stats(int TotalGames, int Wins, int Losses);

public static class StatsFactory
{
    extension(Stats)
    {
        public static Stats Create()
        {
            return new Stats(0, 0, 0);
        }

        public static Stats Restore(int totalGames, int wins, int losses)
        {
            if (totalGames < 0 || wins < 0 || losses < 0)
                throw new ArgumentException("Stats cannot be negative");
                
            return new Stats(totalGames, wins, losses);
        }
    }
}
