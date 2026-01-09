using InjectionLibrary.Attributes;

[assembly: RequiresInjections]

namespace suitsTerminal.Interfaces;

[InjectInterface(typeof(UnlockableSuit))]
public interface IUnlockableSuit
{
    //lets us create a patch for spawn of the suit
    void Start();

    //suitsTerminal attributes
    SuitAttributes Attributes { get; set; }
}
