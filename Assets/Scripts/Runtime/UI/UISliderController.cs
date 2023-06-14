using UnityEngine;
using UnityEngine.UI;

namespace PaperSouls.Runtime.UI
{
    public class UISliderController : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        /// <summary>   
        /// Update the slider values to it's maximum value.
        /// </summary>
        public void IncreaseToMax()
        {
            _slider.value = _slider.maxValue;
        }

        /// <summary>   
        /// Update the slider values to it's minimum value.
        /// </summary>
        public void DecreaseToMin()
        {
            _slider.value = _slider.minValue;
        }

        /// <summary>   
        /// Sets the maximum value for the slider
        /// </summary>
        public void SetMaxValue(float val)
        {
            _slider.maxValue = val;
        }

        /// <summary>   
        /// Sets the current slider value
        /// </summary>
        public void SetValue(float val)
        {
            _slider.value = val;
        }

        /// <summary>   
        /// Gets the current slider value
        /// </summary>
        public float GetValue()
        {
            return _slider.value;
        }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
    }
}
