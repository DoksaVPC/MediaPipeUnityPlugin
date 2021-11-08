using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandednessButton : SwitchButton
{
    [SerializeField]
    private GameObject handIndication;
    [SerializeField]
    private Handedness handedness;

    private enum Handedness
    {
        Left,
        Right
    }

    public void SwitchHand()
    {
        DisplayCurrentActive();
        DisplayOthersInactive();
        if (handedness == Handedness.Right)
        {
            handIndication.transform.localScale = new Vector3(-1, 1, 1);
            objectsPlacer.SetHandedness(ObjectsPlacer.Handedness.Right);
        }
        else
        {
            handIndication.transform.localScale = new Vector3(1, 1, 1);
            objectsPlacer.SetHandedness(ObjectsPlacer.Handedness.Left);
        }
    }

}
