using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderController : MonoBehaviour
{
    public Slider slider;

    public void IncreaseToMax()
    {
        slider.value = slider.maxValue;
    }

    public void DecreaseToMin()
    {
        slider.value = slider.minValue;
    }

    public void SetMaxValue(float val)
    {
        slider.maxValue = val;
    }

    public void SetValue(float val)
    {
        slider.value = val;
    }

    public float GetValue()
    {
        return slider.value;
    }

    private void Awake()
    {
        slider = this.GetComponent<Slider>();
    }
}
