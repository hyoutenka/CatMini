/// <summary>Builds attention from spaced, deliberate bait placements, independently of render frames.</summary>
public sealed class FishingRodAttentionTracker
{
    public const float MinimumDistance = 80f;
    public const float MinimumInterval = 0.2f;
    public const float MaximumInterval = 1.2f;
    public const int RequiredMoves = 3;

    public int AcceptedMoves { get; private set; }
    private bool hasPoint;
    private float previousX;
    private float previousY;
    private float elapsed;

    public void Tick(float dt)
    {
        if (!hasPoint || dt <= 0f) return;
        elapsed += dt;
        if (elapsed > MaximumInterval) Reset();
    }

    public bool RegisterPoint(float x, float y)
    {
        if (!hasPoint)
        {
            Anchor(x, y);
            return false;
        }

        float dx = x - previousX;
        float dy = y - previousY;
        if (dx * dx + dy * dy < MinimumDistance * MinimumDistance) return false;
        if (elapsed < MinimumInterval)
        {
            // Rapid teleporting does not establish a chase rhythm.
            Reset();
            Anchor(x, y);
            return false;
        }

        if (AcceptedMoves < RequiredMoves) AcceptedMoves++;
        Anchor(x, y);
        return AcceptedMoves >= RequiredMoves;
    }

    public void Reset()
    {
        hasPoint = false;
        previousX = previousY = elapsed = 0f;
        AcceptedMoves = 0;
    }

    private void Anchor(float x, float y)
    {
        hasPoint = true;
        previousX = x;
        previousY = y;
        elapsed = 0f;
    }
}
