using System;

static class Program
{
    private static int assertions;

    private static void Equal(FishingRodMovementTracker.Result expected, FishingRodMovementTracker.Result actual, string scenario)
    {
        assertions++;
        if (expected != actual) throw new Exception($"{scenario}: expected {expected}, got {actual}");
    }

    private static FishingRodMovementTracker.Result Click(FishingRodMovementTracker tracker, float x, float y)
    {
        tracker.Begin(x, y);
        return tracker.Complete(x, y);
    }

    private static void Check(bool condition, string scenario)
    {
        assertions++;
        if (!condition) throw new Exception(scenario);
    }

    private static bool Place(FishingRodAttentionTracker tracker, float x, float y, float delay = 0.25f)
    {
        tracker.Tick(delay);
        return tracker.RegisterPoint(x, y);
    }

    private static void VerifyAttention()
    {
        var attention = new FishingRodAttentionTracker();
        Check(!attention.RegisterPoint(0, 0), "First placement only anchors attention");
        Check(!Place(attention, 100, 0), "One move cannot create interest");
        Check(!Place(attention, 0, 0), "Two moves cannot create interest");
        Check(Place(attention, 100, 0), "Three paced moves establish interest");
        Check(Place(attention, 0, 0), "Continued paced movement maintains attention");
        Check(!Place(attention, 100, 0, 0.05f), "Fast teleport breaks attention");
        Check(attention.AcceptedMoves == 0, "Fast clicking clears buildup");
        Check(!Place(attention, 0, 0), "Must rebuild after fast input");
        attention.Tick(1.21f);
        Check(attention.AcceptedMoves == 0, "Long pause clears buildup");
        Check(!attention.RegisterPoint(100, 0), "Post-pause placement starts over");
        for (int i = 0; i < 10; i++) Check(!Place(attention, 100, 0), "Same point cannot build attention");
        attention.Reset();
        attention.RegisterPoint(0, 0);
        Check(!Place(attention, 79, 0), "Substantial distance is required");
        Check(attention.AcceptedMoves == 0, "Short movement does not count");
        Check(!Place(attention, 80, 0), "Exactly 80 counts as first move");
        Check(attention.AcceptedMoves == 1, "Distance boundary is inclusive");
        attention.Reset();
        Check(!attention.RegisterPoint(200, 0), "Reset discards old anchor");
        Check(!Place(attention, 0, 0, 0.2f), "Minimum interval counts first move");
        Check(attention.AcceptedMoves == 1, "Minimum interval is inclusive");
        Check(!Place(attention, 200, 0, 1.2f), "Maximum interval counts second move");
        Check(attention.AcceptedMoves == 2, "Maximum interval is inclusive");
        Check(Place(attention, 0, 0), "Interest returns only after rebuilding");
        foreach (int frames in new[] { 1, 5, 15, 30 })
        {
            attention.Reset();
            attention.RegisterPoint(0, 0);
            for (int i = 0; i < 3; i++)
            {
                for (int frame = 0; frame < frames; frame++) attention.Tick(0.25f / frames);
                bool ready = attention.RegisterPoint(i % 2 == 0 ? 100 : 0, 0);
                Check(ready == (i == 2), "Attention must not depend on frame sampling");
            }
        }
    }

    public static void Main()
    {
        var clicks = new FishingRodMovementTracker();
        var drag = new FishingRodMovementTracker();
        Equal(FishingRodMovementTracker.Result.Anchor, Click(clicks, 0, 0), "Initial click establishes A");
        drag.Begin(0, 0);
        Equal(Click(clicks, 100, 0), drag.Complete(100, 0), "A-B drag equals A click then B click");
        Equal(FishingRodMovementTracker.Result.Anchor, drag.Complete(100, 0), "Release cannot score twice");

        Equal(FishingRodMovementTracker.Result.CloseMove, Click(clicks, 110, 0), "First short move");
        drag.Begin(100, 0);
        Equal(FishingRodMovementTracker.Result.CloseMove, drag.Complete(110, 0), "First short drag");
        drag.Begin(110, 0);
        var secondClose = Click(clicks, 115, 0);
        Equal(FishingRodMovementTracker.Result.RepeatedCloseMove, secondClose, "Second short move is penalized");
        Equal(secondClose, drag.Complete(115, 0), "Repeated short click and drag agree");
        Equal(FishingRodMovementTracker.Result.RepeatedCloseMove, Click(clicks, 116, 0), "Further short moves remain penalized");
        Equal(FishingRodMovementTracker.Result.Move, Click(clicks, 136, 0), "Exactly 20 is a valid move and resets streak");
        Equal(FishingRodMovementTracker.Result.CloseMove, Click(clicks, 140, 0), "Short streak restarts after valid move");

        clicks.Reset();
        Equal(FishingRodMovementTracker.Result.Anchor, Click(clicks, 300, 200), "New session has no previous anchor");
        Equal(FishingRodMovementTracker.Result.CloseMove, Click(clicks, 300, 200), "First repeated same-position click");
        Equal(FishingRodMovementTracker.Result.RepeatedCloseMove, Click(clicks, 300, 200), "Second same-position repeat");
        Equal(FishingRodMovementTracker.Result.Move, Click(clicks, 312, 216), "Diagonal distance of 20 is valid");

        drag.Reset();
        drag.Begin(0, 0);
        Equal(FishingRodMovementTracker.Result.CloseMove, drag.Complete(1, 0), "First gesture can be a short drag");
        drag.Begin(1, 0);
        drag.Reset();
        Equal(FishingRodMovementTracker.Result.Anchor, drag.Complete(100, 0), "Reset cancels pending drag");
        VerifyAttention();
        Console.WriteLine($"PASS: {assertions} fishing-rod gesture and attention assertions.");
    }
}
