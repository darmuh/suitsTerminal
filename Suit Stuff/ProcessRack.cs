using static suitsTerminal.Misc;
using UnityEngine;

namespace suitsTerminal
{
    internal class ProcessRack
    {
        internal static void ProcessHiddenSuit(AutoParentToShip component)
        {
            _ = component.gameObject.GetComponent<SuitInfo>() ?? component.gameObject.AddComponent<SuitInfo>();

            //suitsTerminal.X("processhiddensuit method");
            component.GetComponent<SuitInfo>().suitTag = "hidden";
            component.disableObject = true;
            component.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            component.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        internal static void ProcessHangingSuit(AutoParentToShip component)
        {
            //suitsTerminal.X("processhangingsuit");
            component.disableObject = false;
            component.overrideOffset = true;

            float offsetModifier = 0.18f;

            component.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * offsetModifier * (float)reorderSuits;
            component.rotationOffset = new Vector3(0f, 90f, 0f);
            reorderSuits++;
        }

        internal static void ProcessVisibleSuit(AutoParentToShip component, int normSuit)
        {
            //suitsTerminal.X("processvisiblesuit");
            _ = component.gameObject.GetComponent<SuitInfo>() ?? component.gameObject.AddComponent<SuitInfo>();
            component.GetComponent<SuitInfo>().suitTag = "hanging";
            component.overrideOffset = true;

            float offsetModifier = 0.18f;

            component.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) + StartOfRound.Instance.rightmostSuitPosition.forward * offsetModifier * (float)normSuit;
            component.rotationOffset = new Vector3(0f, 90f, 0f);

            showSuit++;
        }
    }
}
