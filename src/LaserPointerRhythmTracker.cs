/// <summary>Tracks the left-right sweep rhythm of the laser pointer without Unity input or geometry.</summary>
public sealed class LaserPointerRhythmTracker
{
    public const float MinimumSweepDistance = 40f;
    public const float MaximumSweepTime = 1.2f;
    public const float RhythmDuration = 1.2f;
    public const float MaximumPauseTime = 0.6f;

    private float previousMoveX;
    private float rhythmTime;
    private float sweepDistance;
    private float pauseTime;
    private float sweepTime;
    private int reversals;

    public bool IsActivelyShaking { get; private set; }
    public bool CanHoldInterest => rhythmTime > 0f && pauseTime <= MaximumPauseTime;

    public void Advance(float dt)
    {
        IsActivelyShaking = false;
        rhythmTime = System.Math.Max(0f, rhythmTime - dt);
        sweepTime += dt;
    }

    public void Observe(float moveX, float moveY, bool inFront, float dt)
    {
        bool horizontal = System.Math.Abs(moveX) > 0.001f &&
            System.Math.Abs(moveX) >= System.Math.Abs(moveY);
        if (inFront && horizontal)
        {
            pauseTime = 0f;
            if (previousMoveX * moveX < 0f)
            {
                if (sweepDistance >= MinimumSweepDistance && sweepTime <= MaximumSweepTime)
                    reversals++;
                else
                    reversals = 0;
                if (reversals >= 2) rhythmTime = RhythmDuration;
                sweepDistance = 0f;
                sweepTime = 0f;
            }
            sweepDistance += System.Math.Abs(moveX);
            previousMoveX = moveX;
            IsActivelyShaking = rhythmTime > 0f;
            return;
        }

        pauseTime += dt;
        if (!inFront || pauseTime > MaximumPauseTime) ClearRhythm();
    }

    public void HitBoundary()
    {
        ClearRhythm();
        IsActivelyShaking = false;
    }

    public void Reset()
    {
        ClearRhythm();
        pauseTime = 0f;
        IsActivelyShaking = false;
    }

    private void ClearRhythm()
    {
        rhythmTime = 0f;
        reversals = 0;
        sweepDistance = 0f;
        sweepTime = 0f;
        previousMoveX = 0f;
    }
}
