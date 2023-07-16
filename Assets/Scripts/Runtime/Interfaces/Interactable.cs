using UnityEngine.Events;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Interfaces
{
    internal interface IInteractable
    {
        /// <summary>
        /// Callback to run once an interaction is complete.
        /// </summary>
        public UnityAction<IInteractable> OnInteractionComplete { get; set; }

        /// <summary>
        /// Interaction with a Interactor
        /// </summary>
        public void Interact(Interactor interactor, out bool successful);

        /// <summary>
        /// Method to be called once Interaction is complete
        /// </summary>
        public void EndInteraction();
    }
}
