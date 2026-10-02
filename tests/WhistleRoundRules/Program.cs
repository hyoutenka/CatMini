using System;

static class Program
{
    static int checks;
    static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new Exception(message);
    }

    static void Main()
    {
        for (int hits = 0; hits <= 5; hits++)
        {
            float expected = hits == 5 ? 27.5f : hits >= 2 ? 17.5f : -10f;
            Check(WhistleRoundRules.Score(hits) == expected, $"Score for {hits} hits");
        }
        foreach (int invalid in new[] { -1, 6 })
        {
            bool rejected = false;
            try { WhistleRoundRules.Score(invalid); }
            catch (ArgumentOutOfRangeException) { rejected = true; }
            Check(rejected, $"Score rejects {invalid} hits");
        }
        Check(30f + 4 * WhistleRoundRules.Score(2) == 100f, "Four success rounds reach exactly 100");
        Check(30f + 3 * WhistleRoundRules.Score(5) >= 100f, "Three perfect rounds win early");
        Check(WhistleRoundRules.CanReachTarget(45f, 3, 100f), "Equality must allow continued play");
        Check(!WhistleRoundRules.CanReachTarget(44.5f, 3, 100f), "Just below the maximum calls the game");
        Check(!WhistleRoundRules.CanReachTarget(10f, 2, 100f), "Two failed rounds call the game");
        Check(WhistleRoundRules.CanReachTarget(20f, 1, 100f), "One failed round can be recovered");
        Check(30f + WhistleRoundRules.Score(0) + WhistleRoundRules.Score(5)
            + 3 * WhistleRoundRules.Score(2) == 100f, "One perfect recovers one failure with three successes");
        Check(WhistleRoundRules.CanReachTarget(72.5f, 4, 100f), "Three successes and one failure allow a final perfect");

        // Independently enumerate all possible future round outcomes to verify the cutoff.
        for (int completed = 0; completed <= 5; completed++)
        for (int halfPoints = 0; halfPoints <= 200; halfPoints++)
        {
            float interest = halfPoints * .5f;
            bool possible = CanWinByPlaying(interest, 5 - completed);
            Check(WhistleRoundRules.CanReachTarget(interest, completed, 100f) == possible,
                $"Reachability at {interest} after {completed} rounds");
        }
        Console.WriteLine($"PASS: {checks} whistle scoring and called-game checks.");
    }

    static bool CanWinByPlaying(float interest, int remaining)
    {
        if (interest >= 100f) return true;
        if (remaining == 0) return false;
        foreach (float score in new[] { -10f, 17.5f, 27.5f })
            if (CanWinByPlaying(Math.Max(0f, interest + score), remaining - 1)) return true;
        return false;
    }
}
