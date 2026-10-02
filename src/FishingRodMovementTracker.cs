/// <summary>Scores completed fishing-rod gestures equally, regardless of their sampled path.</summary>
public sealed class FishingRodMovementTracker
{
    public enum Result { Anchor, Move, CloseMove, RepeatedCloseMove }

    public const float CloseDistance = 20f;
    private bool hasPoint;
    private bool gestureActive;
    private bool firstGesture;
    private float previousX;
    private float previousY;
    private int consecutiveCloseMoves;

    public void Begin(float x, float y)
    {
        gestureActive = true;
        firstGesture = !hasPoint;
        if (firstGesture)
        {
            previousX = x;
            previousY = y;
        }
    }

    public Result Complete(float x, float y)
    {
        if (!gestureActive) return Result.Anchor;
        gestureActive = false;
        float dx = x - previousX;
        float dy = y - previousY;
        float distanceSquared = dx * dx + dy * dy;
        previousX = x;
        previousY = y;
        hasPoint = true;

        // Placing the first point is not a repeated, zero-distance move.
        if (firstGesture && distanceSquared <= 0.0001f) return Result.Anchor;
        if (distanceSquared < CloseDistance * CloseDistance)
        {
            consecutiveCloseMoves++;
            return consecutiveCloseMoves >= 2 ? Result.RepeatedCloseMove : Result.CloseMove;
        }

        consecutiveCloseMoves = 0;
        return Result.Move;
    }

    public void Reset()
    {
        hasPoint = false;
        gestureActive = false;
        firstGesture = false;
        consecutiveCloseMoves = 0;
        previousX = previousY = 0f;
    }
}
