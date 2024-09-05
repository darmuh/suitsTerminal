

namespace suitsTerminal.EventSub
{
    internal class TerminalDisable
    {
        internal static void OnTerminalDisable()
        {
            if (suitsTerminal.OpenBodyCams)
                OpenBodyCams.ResidualCamsCheck();
        }
    }
}
