using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

/*TODO:
 * Move all of the climbable script stuff to this
 * 
 * 
 */

public class InteractableObject : MonoBehaviour
{

    public UnityEvent onUseObjectStateDown;

    private ForceManipulation _mainForceHand;
    private ForceManipulation _offForceHand;
    private GrabObjController _mainHandGrab;
    private GrabObjController _offHandGrab;
    private GrabObjController _mainHandClimb;

    private Rigidbody _grabbingTarget;

    [SerializeField] private SteamVR_Action_Boolean UseObject = null;
    private SteamVR_Input_Sources _mainHandInput = SteamVR_Input_Sources.Head;

    public bool Climbable = false;
    [Tooltip("Use for objects that we only want to force grab")] public bool PickupAble = true;
    public bool CanUseWhileForceGrabbed = true;

    private bool invoked = false;

    private void Awake()
    {
        _grabbingTarget = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_mainHandGrab && _mainHandInput != _mainHandGrab.HandSource)
            _mainHandInput = _mainHandGrab.HandSource;
        if (_mainForceHand && _mainHandInput != _mainForceHand.HandSource)
            _mainHandInput = _mainForceHand.HandSource;
        if (!_mainHandGrab && !_mainForceHand)
            _mainHandInput = SteamVR_Input_Sources.Head;
        if (invoked == false && SteamVR_Actions.default_InteractObject.GetState(_mainHandInput) && (_mainHandGrab || (_mainForceHand && CanUseWhileForceGrabbed))) //Use action on triggle press
        {
            onUseObjectStateDown.Invoke();
            invoked = true;
        }
        else if (!SteamVR_Actions.default_InteractObject.GetState(_mainHandInput) && invoked == true)
            invoked = false;

    }

    private void FixedUpdate()
    {
        if (_mainHandGrab || _offHandGrab)
            UpdateGrabPosition();
        if (_mainForceHand || _offForceHand)
            UpdateForceGrabPosition();
    }

    private void UpdateGrabPosition()
    {
        if(_mainHandGrab && !_offHandGrab) //Move to mainhand using velocity
        {
            //Move to mainhand
            _grabbingTarget.velocity = (_mainHandGrab.transform.position - transform.position) / Time.fixedDeltaTime;

            //rotate properly
            _grabbingTarget.maxAngularVelocity = 20f;
            Quaternion _deltaRot = _mainHandGrab.transform.rotation * Quaternion.Inverse(transform.rotation);
            Vector3 _eulerRot = new Vector3(Mathf.DeltaAngle(0, _deltaRot.eulerAngles.x),
                Mathf.DeltaAngle(0, _deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, _deltaRot.eulerAngles.z));
            _eulerRot *= 0.95f; //Don't know if we want or need this value
            _eulerRot *= Mathf.Deg2Rad;
            _grabbingTarget.angularVelocity = _eulerRot / Time.fixedDeltaTime;
        }
        if(_mainHandGrab && _offHandGrab)
        {
            //Move to mainhand
            _grabbingTarget.velocity = (_mainHandGrab.transform.position - transform.position) / Time.fixedDeltaTime;

            //LookRotation toward the second hand, but use the upwards of the main hand
            //Quaternion _rotationDirection = Quaternion.LookRotation(_offHandGrab.transform.position, _mainHandGrab.transform.position);

            ////rotate toward the second hand
            //_grabbingTarget.maxAngularVelocity = 20f;
            //Quaternion _deltaRot = _rotationDirection * Quaternion.Inverse(transform.rotation);
            //Vector3 _eulerRot = new Vector3(Mathf.DeltaAngle(0, _deltaRot.eulerAngles.x),
            //    Mathf.DeltaAngle(0, _deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, _deltaRot.eulerAngles.z));
            ////_eulerRot *= 0.95f; //Don't know if we want or need this value
            //_eulerRot *= Mathf.Deg2Rad;
            //_grabbingTarget.angularVelocity = _eulerRot / Time.fixedDeltaTime;
            transform.LookAt(_offHandGrab.transform.position);
        }
    }

    private void UpdateForceGrabPosition()
    {
        if(_mainForceHand && !_offForceHand)
        {
            _grabbingTarget.velocity = (((_mainForceHand.ForcePosition - transform.position) * _mainForceHand.ForceGrabPower) / _grabbingTarget.mass) * Time.fixedDeltaTime;
        }
        if(_mainForceHand && _offForceHand)
        {
            //Take _mainhand pos and _offhand pos and find the position in the middle
            Vector3 _midPoint = (_mainForceHand.ForcePosition + _offForceHand.ForcePosition) / 2;
            //Add force grab power together

            float _sumForcePower = (_mainForceHand.ForceGrabPower + _offForceHand.ForceGrabPower);
            if (_sumForcePower * (_grabbingTarget.mass / 4f) > 1f)
                _sumForcePower = _sumForcePower * (_grabbingTarget.mass / 4f);
            //Do velocity thing
            _grabbingTarget.velocity = (((_midPoint - transform.position) * _sumForcePower) / _grabbingTarget.mass) * Time.fixedDeltaTime;
        }
    }

    public void ForceGrabMe(ForceManipulation _hand)
    {
        if (!_mainForceHand)
            _mainForceHand = _hand;
        else
            _offForceHand = _hand;
    }

    public void ForceUngrabMe(ForceManipulation _hand, bool UnGrabAll = false)
    {

        print("help");
        if (UnGrabAll == true)
        {
            _mainForceHand = null;
            _offForceHand = null;
        }
        if (_mainForceHand == _hand)
            _mainForceHand = null;
        if (_offForceHand == _hand)
            _offForceHand = null;
        if (!_mainForceHand && _offForceHand)
        {
            _mainForceHand = _offForceHand;
            _offForceHand = null;
        }
    }

    public void GrabMe(GrabObjController _hand, ClimbController _climbController = null)
    {
        if (Climbable)
        {
            _mainHandClimb = _hand;
            _climbController.StartClimbObject(_hand.gameObject);
            return;
        }
        if (!PickupAble)
            return;
        if (!_mainHandGrab)
            _mainHandGrab = _hand;
        else
            _offHandGrab = _hand;
    }

    public void UngrabMe(GrabObjController _hand, ClimbController _climbController = null)
    {
        if (Climbable)
        {
            _mainHandClimb = null;
            _climbController.DropHand(_hand.gameObject);
            return;
        }

        if (_mainHandGrab == _hand)
            _mainHandGrab = null;
        if (_offHandGrab == _hand)
            _offHandGrab = null;
    }

    public bool IsGrabbing(GrabObjController _hand)
    {
        if (_mainHandGrab == _hand || _offHandGrab == _hand || _mainHandClimb == _hand)
            return true;
        else
            return false;
    }

    public bool IsForceGrabbing(ForceManipulation _hand)
    {
        if (_mainForceHand == _hand || _offForceHand == _hand)
            return true;
        else
            return false;
    }

}
