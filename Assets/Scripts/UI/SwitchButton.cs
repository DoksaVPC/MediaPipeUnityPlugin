using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using LeTai.TrueShadow;

public class SwitchButton : MonoBehaviour
{
    [SerializeField]
    private Font medium;
    [SerializeField]
    private Font bold;
    [SerializeField]
    private protected ObjectsPlacer objectsPlacer;
    [SerializeField]
    private Gradient2 buttonGradient;
    [SerializeField]
    private Image buttonStroke;
    [SerializeField]
    private TrueShadow dropShadow;
    [SerializeField]
    private Text buttonText;

    [SerializeField]
    private Image[] otherButtonsStrokes;
    [SerializeField]
    private Gradient2[] otherButtonsGradients;
    [SerializeField]
    private TrueShadow[] otherButtonsDropShadow;
    [SerializeField]
    private Text[] otherButtonsText;

    [SerializeField]
    private bool isActive = false;

    private Color grey = new Color(1f, 1f, 1f, 0.3f);
    private Color white = new Color(1f, 1f, 1f, 1f);
    private Color strongShadow = new Color(0.07f, 0.07f, 0.07f, 0.3f);
    private Color lightShadow = new Color(0.07f, 0.07f, 0.07f, 0.15f);

    // Start is called before the first frame update
    void Start()
    {
        if (isActive)
        {
            DisplayCurrentActive();
            DisplayOthersInactive();
        }
    }

    private protected void DisplayCurrentActive()
    {
        buttonGradient.enabled = true;
        buttonStroke.color = white;
        dropShadow.Color = strongShadow;
        buttonText.font = bold;
    }

    private protected void DisplayOthersInactive()
    {
        foreach (Image buttonStroke in otherButtonsStrokes)
        {
            buttonStroke.color = grey;
        }

        foreach (Gradient2 buttonGradient in otherButtonsGradients)
        {
            buttonGradient.enabled = false;
        }

        foreach (TrueShadow dropShadow in otherButtonsDropShadow)
        {
            dropShadow.Color = lightShadow;
        }

        foreach (Text buttonText in otherButtonsText)
        {
            buttonText.font = medium;
        }
    }
}
