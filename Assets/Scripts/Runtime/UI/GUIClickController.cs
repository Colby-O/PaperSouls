using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace PaperSouls.Runtime.UI
{
    public class GUIClickController : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnLeft;
        public UnityEvent OnRight;
        public UnityEvent OnMiddle;

        /// <summary>   
        /// Determines what action the user performed while hovering over a UI element
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeft.Invoke();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRight.Invoke();
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                OnMiddle.Invoke();
            }
        }
    }
}
