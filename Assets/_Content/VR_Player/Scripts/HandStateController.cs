using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlsLIB;
using Valve.VR;

public class HandStateController : MonoBehaviour
{
    private GrabObjController _grabObjController = null;
    private ForceManipulation _forceManipulation = null;
    private ForceGestureHand _forceGesture = null;
    private ClimbHand _climbHand = null;

    private HandAction _currentHandAction = HandAction.Idle;

    public SteamVR_Input_Sources HandSource = SteamVR_Input_Sources.LeftHand;

    // Start is called before the first frame update
    void Awake()
    {
        _grabObjController = gameObject.GetComponent<GrabObjController>();
        _forceManipulation = gameObject.GetComponent<ForceManipulation>();
        _forceGesture = gameObject.GetComponent<ForceGestureHand>();
        _climbHand = gameObject.GetComponent<ClimbHand>();
        HandSource = _grabObjController.HandSource;
    }

    // Update is called once per frame
    void Update()
    {
        if (_forceGesture.ActiveGesture)
            _currentHandAction = HandAction.ForceAbility;
        if (_grabObjController.isGrabbing)
            _currentHandAction = HandAction.Grabbed;
        if (_forceManipulation.isGrabbing)
            _currentHandAction = HandAction.ForceGrabbed;
        if (!_grabObjController.isGrabbing && !_forceManipulation.isGrabbing && !_forceGesture.ActiveGesture)
            _currentHandAction = HandAction.Idle;

    }

    private void UpdateGrabAbilities()
    {
        if(_currentHandAction == HandAction.Idle)
        {
            if(!_grabObjController.CanGrab)
                _grabObjController.CanGrab = true;
            if (!_forceManipulation.CanGrab)
                _forceManipulation.CanGrab = true;
            if (!_forceGesture.CanGesture)
                _forceGesture.CanGesture = true;
        }
        if (_currentHandAction == HandAction.Grabbed)
        {
            if (_forceManipulation.CanGrab)
                _forceManipulation.CanGrab = false;
            if (_forceGesture.CanGesture)
                _forceGesture.CanGesture = false;
        }
        if(_currentHandAction == HandAction.ForceAbility)
        {
            if (_grabObjController.CanGrab)
                _grabObjController.CanGrab = false;
            if (_forceManipulation.CanGrab)
                _forceManipulation.CanGrab = false;
        }
        if(_currentHandAction == HandAction.ForceGrabbed)
        {
            if (_grabObjController.CanGrab)
                _grabObjController.CanGrab = false;
            if (_forceGesture.CanGesture)
                _forceGesture.CanGesture = false;
        }
                
    }
}
