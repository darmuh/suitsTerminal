using InjectionLibrary.Attributes;
using suitsTerminal.Suits;

[assembly: RequiresInjections]

namespace suitsTerminal.Interfaces;

[InjectInterface(typeof(UnlockableSuit))]
public interface IUnlockableSuit
{
    //lets us create a patch for spawn of the suit
    [HandleErrors(InjectionLibrary.ErrorHandlingStrategy.Ignore)] //it's okay if someone else injects this method
    void Start();

    //suitsTerminal attributes
    [HandleErrors(InjectionLibrary.ErrorHandlingStrategy.LogWarning)] //should never happen but who knows
    SuitAttributes SuitsTerminalAttributes { get; set; }
}
