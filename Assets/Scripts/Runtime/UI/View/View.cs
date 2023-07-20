using UnityEngine;

namespace PaperSouls.Runtime.UI.View
{
    internal abstract class View : MonoBehaviour
    {
        /// <summary>
        /// Initialzes a view. 
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Hides a view. 
        /// </summary>
        public virtual void Hide() => gameObject.SetActive(false);

        /// <summary>
        /// Displays a view. 
        /// </summary>
        public virtual void Show() => gameObject.SetActive(true);
    }
}
