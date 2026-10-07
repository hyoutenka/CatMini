using System;

static class Program
{
    private static int checks;

    private static void Check(bool condition, string scenario)
    {
        checks++;
        if (!condition) throw new Exception(scenario);
    }

    private static void Step(LaserPointerRhythmTracker tracker, float dt, float x, float y = 0f, bool inFront = true)
    {
        tracker.Advance(dt);
        tracker.Observe(x, y, inFront, dt);
    }

    private static void BuildRhythm(LaserPointerRhythmTracker tracker)
    {
        Step(tracker, 0.1f, 40f);
        Check(!tracker.IsActivelyShaking, "One sweep is not a rhythm");
        Step(tracker, 0.1f, -40f);
        Check(!tracker.IsActivelyShaking, "One reversal is not a rhythm");
        Step(tracker, 0.1f, 40f);
        Check(tracker.IsActivelyShaking, "Two valid reversals establish a rhythm");
    }

    private static void Main()
    {
        var tracker = new LaserPointerRhythmTracker();
        BuildRhythm(tracker);
        Check(tracker.CanHoldInterest, "Active rhythm can hold interest");

        Step(tracker, 0.5f, 0f);
        Check(!tracker.IsActivelyShaking, "A key-release pause does not earn interest");
        Check(tracker.CanHoldInterest, "A short pause preserves the rhythm");
        Step(tracker, 0.11f, 0f);
        Check(!tracker.CanHoldInterest, "A pause longer than 0.6 seconds clears the rhythm");
        Step(tracker, 0.1f, -40f);
        Check(!tracker.IsActivelyShaking, "One sweep after a long pause cannot restore interest");

        tracker.Reset();
        BuildRhythm(tracker);
        Step(tracker, 0.1f, 0f, 0f, false);
        Check(!tracker.CanHoldInterest, "Leaving the safe area clears the rhythm");

        tracker.Reset();
        BuildRhythm(tracker);
        tracker.HitBoundary();
        Check(!tracker.IsActivelyShaking && !tracker.CanHoldInterest,
            "Touching the body or ground boundary stops the rhythm");

        tracker.Reset();
        Step(tracker, 0.1f, 39f);
        Step(tracker, 0.1f, -40f);
        Step(tracker, 0.1f, 40f);
        Check(!tracker.IsActivelyShaking, "A sweep shorter than 40 does not count as a reversal");

        tracker.Reset();
        Step(tracker, 1.21f, 40f);
        Step(tracker, 0.01f, -40f);
        Step(tracker, 0.01f, 40f);
        Check(!tracker.IsActivelyShaking, "A slow sweep does not count as a reversal");

        tracker.Reset();
        Step(tracker, 0.1f, 0f, 40f);
        Step(tracker, 0.1f, -40f);
        Step(tracker, 0.1f, 40f);
        Check(!tracker.IsActivelyShaking, "Vertical movement does not build horizontal rhythm");

        tracker.Reset();
        Step(tracker, 1.2f, 40f);
        Step(tracker, 0f, -40f);
        Step(tracker, 1.2f, 40f);
        Check(tracker.IsActivelyShaking, "The 1.2-second sweep boundary is inclusive");

        foreach (int frames in new[] { 1, 4, 10 })
        {
            tracker.Reset();
            for (int sweep = 0; sweep < 3; sweep++)
            for (int frame = 0; frame < frames; frame++)
                Step(tracker, 0.2f / frames, (sweep % 2 == 0 ? 40f : -40f) / frames);
            Check(tracker.IsActivelyShaking, "Rhythm does not depend on frame sampling");
        }

        Console.WriteLine($"PASS: {checks} laser-pointer rhythm checks.");
    }
}
