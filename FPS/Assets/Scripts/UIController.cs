using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text overheatedMessage;
    public Slider weaponTempSlider;
    public GameObject crosshair;

    public GameObject deathScreen;
    public TMP_Text deathText;

    public Slider healthSlider;

    private void Awake()
    {
        instance = this;
    }
}
