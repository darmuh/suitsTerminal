using UnityEngine;
using static suitsTerminal.Misc;

namespace suitsTerminal
{
    internal class ProcessRack
    {
        internal static void ProcessHiddenSuit(AutoParentToShip component)
        {
            //Plugin.X("processhiddensuit method");
            component.disableObject = true;
            component.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            component.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        internal static void ProcessVisibleSuit(AutoParentToShip component, int suitNumber)
        {
            //Plugin.X("processvisiblesuit");
            component.overrideOffset = true;

            float offsetModifier = 0.18f;

            component.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * offsetModifier * suitNumber;
            component.rotationOffset = new Vector3(0f, 90f, 0f);

            suitsOnRack++;
        }
    }
}
