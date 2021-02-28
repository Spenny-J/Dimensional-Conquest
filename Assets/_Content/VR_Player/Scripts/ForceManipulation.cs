using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/*We want to do:
 * Unity spherecast
 *  - Cast until we hit 1 object
 *  - Highlight that object
 *  - If grab action is performed we force grab the object
 */

public class ForceManipulation : MonoBehaviour
{
    public SteamVR_Input_Sources HandSource = SteamVR_Input_Sources.LeftHand;
    private Rigidbody ForceTarget;

    public float CastRadius = .2f;
    [SerializeField] float _throwSpeedThreshold = 0.035f;
    public float ForceGrabPower = 250f;

    public bool CanGrab = true;
    public bool isGrabbing { get; private set; } = false;

    public Vector3 ForcePosition;

    [SerializeField] private LayerMask GrabLayer;
    private LayerMask _castLayer = ~0;
    private float _maxDistance = 50f;
    private float _objDistance = 0f;
    private bool _isGrabbed = false;
    private bool _handClosed = false;
    private Vector3 _castDirection;
    //private Vector3 _currentHandPosition;
    private Vector3 _currentHandLocalPos;
    //private Vector3 _handPositionLastFrame;
    private Vector3 _handLocalPositionLastFrame;
    [SerializeField] private GameObject _hmd;
    private float _hitDitance = 2f; //Delete as is debug

    private void Start()
    {
        if (!_hmd)
            _hmd = GameObject.Find("Camera");
    }

    private void Update()
    {
        if (SteamVR_Actions.default_GrabGrip.GetState(HandSource))
            _handClosed = true;
        else
            _handClosed = false;

        //_currentHandPosition = transform.position;
        _currentHandLocalPos = transform.localPosition;

        if (HandSource == SteamVR_Input_Sources.LeftHand)
            _castDirection = transform.right;
        else if (HandSource == SteamVR_Input_Sources.RightHand)
            _castDirection = -transform.right;
    }

    private void FixedUpdate()
    {
        if (CanGrab)
        {
            if (!_handClosed)
            {
                if (ForceTarget)
                    DropObject();
                RaycastHit _hit;
                if (Physics.SphereCast(transform.position, CastRadius, _castDirection, out _hit, _maxDistance, GrabLayer, QueryTriggerInteraction.Ignore))
                {
                    if (_hit.transform.gameObject)
                    {
                        RaycastHit _safetyHit; //Double check that we hit it the object by actually doing a raycast
                        Vector3 _objDirection = _hit.transform.position - transform.position;
                        if (Physics.Raycast(transform.position, _objDirection, out _safetyHit, _maxDistance, _castLayer, QueryTriggerInteraction.Ignore))
                        {
                            if (_safetyHit.transform.GetComponent<InteractableObject>() && !_safetyHit.transform.GetComponent<InteractableObject>().Climbable)
                            {
                                ForceTarget = _hit.transform.GetComponent<Rigidbody>();
                                ForceTarget.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue; //Debug
                                _hitDitance = _hit.distance; //Debug delete please
                            }
                        }
                    }
                }
                else
                {
                    if (ForceTarget != null)
                        ForceTarget.transform.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.white; //Debuggin
                    ForceTarget = null;
                }
            }
            else
            {
                if (ForceTarget)
                {
                    ForcePosition = transform.position + _castDirection * _objDistance;
                    if (!ForceTarget.GetComponent<InteractableObject>().IsForceGrabbing(this))
                        ForceTarget.GetComponent<InteractableObject>().ForceGrabMe(this);
                    ObjectThrow();
                    if (_objDistance <= 0f && ForceTarget)
                        _objDistance = Vector3.Magnitude(transform.position - ForceTarget.transform.position);
                    isGrabbing = true;
                }
            }
        }
        else
        {
            if(ForceTarget != null)
            {
                DropObject();
                ForceTarget = null;
            }
        }
        //_handPositionLastFrame = _currentHandPosition;
        _handLocalPositionLastFrame = _currentHandLocalPos;
    }

    //private void UpdateTargetPos()
    //{
        //point + direction * length == point?
        //_ForcePosition.y = transform.position.y;
        //ForceTarget.velocity = (((_ForcePosition - ForceTarget.transform.position) * 250f) / ForceTarget.mass) * Time.fixedDeltaTime;
    //}

    private void ObjectThrow()
    {
        //Want to calcuate the velocity of the controller (this object)
        float _speed = Vector3.Magnitude(_currentHandLocalPos - _handLocalPositionLastFrame);
        //Then check if we are closer to the HMD or further if we get past the minimum velocity
        //if past min velocity
        if (_speed > _throwSpeedThreshold)
        {
            float _currentHandDist = Vector3.Magnitude(_currentHandLocalPos - _hmd.transform.localPosition);
            float _lastHandDist = Vector3.Magnitude(_handLocalPositionLastFrame - _hmd.transform.localPosition);
            if(_currentHandDist > _lastHandDist)
            {
                //Throw object away from you
                ForceTarget.AddForce(_hmd.transform.forward * 20f, ForceMode.Impulse);
                print("Force Push!");
                DropObject(true);
            }
            else
            {
                //Throw object toward you
                ForceTarget.AddForce(-_hmd.transform.forward * 20f, ForceMode.Impulse);
                print("Force Pull!");
                DropObject(true);
            }
        }
        //then check distance between hand in current frame and hand last frame
        //if distance is greater = push away
        //if distance is less than = pull toward
        //Just so we only have to do the distance check once to save on performance
        //Throw the object toward, or away from the users HMD or hand
    }

    private void DropObject(bool DropAll = false)
    {
        if (ForceTarget && ForceTarget.GetComponent<InteractableObject>().IsForceGrabbing(this))
            ForceTarget.GetComponent<InteractableObject>().ForceUngrabMe(this, DropAll);
        _objDistance = 0f;
        isGrabbing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + _castDirection * _hitDitance);
        Gizmos.DrawWireSphere(transform.position + _castDirection * _hitDitance, CastRadius);
    }

}
