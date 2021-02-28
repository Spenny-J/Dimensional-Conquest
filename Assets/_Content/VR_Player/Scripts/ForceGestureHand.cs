using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ForceGestureHand : MonoBehaviour
{

    public SteamVR_Input_Sources _HandInput = SteamVR_Input_Sources.LeftHand;

    public bool CanGesture = true;
    public bool ActiveGesture = false;

    private bool _handClosed = false;

    public Vector3 HandClosedPosition = Vector3.zero;
    public Vector3 HandOpenPosition = Vector3.zero;

    [SerializeField] private float ForceTimer = 1f;
    private float _currentTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Actions.default_GrabGrip.GetState(_HandInput))
            _handClosed = true;
        else
            _handClosed = false;
        if (CanGesture)
        {
            if (!_handClosed)
            {
                if (HandClosedPosition == Vector3.zero && HandOpenPosition == Vector3.zero)
                    HandOpenPosition = gameObject.transform.localPosition;

                ActiveGesture = false;
                if (_currentTime != ForceTimer)
                    _currentTime = ForceTimer;
            }
            else
            {
                if (HandClosedPosition == Vector3.zero)
                    HandClosedPosition = gameObject.transform.localPosition;

                _currentTime -= Time.deltaTime;
                if (_currentTime <= 0)
                    ActiveGesture = true;
            }
        }
        else
            ActiveGesture = false;
    }

    public void ResetHand()
    {
        HandClosedPosition = Vector3.zero;
        HandOpenPosition = Vector3.zero;

    }
}
