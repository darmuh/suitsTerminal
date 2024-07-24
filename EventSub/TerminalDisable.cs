using OpenLib.Menus;
using System;
using System.Collections.Generic;
using System.Text;

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
