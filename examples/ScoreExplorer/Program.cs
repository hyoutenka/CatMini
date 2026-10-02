using System;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run --project examples/ScoreExplorer -- 2 5 0 3");
    Console.WriteLine("Each number is successful inputs (0-5) in one whistle round.");
    return;
}
if (args.Length > WhistleRoundRules.MaximumRounds)
{
    Console.Error.WriteLine($"At most {WhistleRoundRules.MaximumRounds} rounds are allowed.");
    Environment.ExitCode = 1;
    return;
}

float interest = 30f;
for (int round = 0; round < args.Length; round++)
{
    if (!int.TryParse(args[round], out int successes) ||
        successes < 0 || successes > WhistleRoundRules.AttemptsPerRound)
    {
        Console.Error.WriteLine($"Round {round + 1}: enter a whole number from 0 to {WhistleRoundRules.AttemptsPerRound}.");
        Environment.ExitCode = 1;
        return;
    }

    float change = WhistleRoundRules.Score(successes);
    interest = Math.Clamp(interest + change, 0f, 100f);
    Console.WriteLine($"Round {round + 1}: {successes}/5, {change:+0.0;-0.0;0.0}, interest {interest:0.0}/100");
    if (interest >= 100f)
    {
        Console.WriteLine("Success: interest reached 100.");
        return;
    }
    if (interest <= 0f || !WhistleRoundRules.CanReachTarget(interest, round + 1, 100f))
    {
        Console.WriteLine(round + 1 == WhistleRoundRules.MaximumRounds
            ? "Failed: the five rounds ended below 100."
            : "Called game: the remaining rounds cannot reach 100.");
        return;
    }
}

Console.WriteLine("Ready for the next round.");
