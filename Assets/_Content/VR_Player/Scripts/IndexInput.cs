using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class IndexInput : MonoBehaviour
{

    public SteamVR_Action_Vector2 ThumbstickAction = null;
    public SteamVR_Action_Vector2 TrackpadAction = null;

    // Update is called once per frame
    void Update()
    {
        ThumbStick();
        Trackpad();
    }

    private void ThumbStick()
    {
        if (ThumbstickAction.axis == Vector2.zero)
            return;

        print("Thumbstick: " + ThumbstickAction.axis);
    }

    private void Trackpad()
    {
        if (TrackpadAction.axis == Vector2.zero)
            return;

        print("Thumbstick: " + TrackpadAction.axis);
    }

}
