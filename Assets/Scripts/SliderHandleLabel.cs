using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderHandleLabel : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    public void setText(Slider slider)
    {
        label.text = $"{slider.value}";
    }
}
