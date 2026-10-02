public static class WhistleRoundRules
{
    public const int AttemptsPerRound = 5;
    public const int MinimumSuccesses = 2;
    public const int MaximumRounds = 5;
    public const float SuccessGain = 17.5f;
    public const float PerfectBonus = 10f;
    public const float PerfectGain = SuccessGain + PerfectBonus;
    public const float FailedPenalty = -10f;

    public static float Score(int successes)
    {
        if (successes < 0 || successes > AttemptsPerRound)
            throw new System.ArgumentOutOfRangeException(nameof(successes));
        if (successes == AttemptsPerRound) return PerfectGain;
        return successes >= MinimumSuccesses ? SuccessGain : FailedPenalty;
    }

    public static bool CanReachTarget(float interest, int completedRounds, float target)
    {
        int remainingRounds = System.Math.Max(0, MaximumRounds - completedRounds);
        return interest + remainingRounds * PerfectGain >= target;
    }
}
