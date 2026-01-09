using suitsTerminal.Interfaces;

namespace suitsTerminal;

public static class Extensions
{
    public static void GetSuitAttributes(this UnlockableSuit suit)
    {
        if (suit == null)
            return;

        if (((IUnlockableSuit)suit).Attributes == null)
            ((IUnlockableSuit)suit).Attributes = suit.gameObject.AddComponent<SuitAttributes>();
    }

    public static void ResetPosition(this UnlockableSuit suit)
    {
        if (suit == null) return;

        RackManager.SuitSpawn(suit);
    }
}
