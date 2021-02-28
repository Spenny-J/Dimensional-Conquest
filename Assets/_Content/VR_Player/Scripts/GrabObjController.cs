using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/* TODO:
 * Check if the object is climbable
 * Add a climbable hand instead of a grabbable hand to the object
 */

public class GrabObjController : MonoBehaviour
{
    public float DistToPickup = 0.15f;
    public bool CanGrab = true;
    public bool isGrabbing { get; private set; } = false;
    private bool _handClosed = false;
    [SerializeField] private ClimbController PlayerClimbController = null;
    [SerializeField] private LayerMask _pickupLayer;

    public SteamVR_Input_Sources HandSource = SteamVR_Input_Sources.LeftHand;

    private Rigidbody holdingTarget;

    private void Update()
    {
        if (SteamVR_Actions.default_GrabGrip.GetState(HandSource))
            _handClosed = true;
        else
            _handClosed = false;
    }

    private void FixedUpdate()
    {
        if (CanGrab)
        {
            if (!_handClosed)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, DistToPickup, _pickupLayer);
                if (colliders.Length > 0)
                {
                    if (colliders[0].transform.root.GetComponent<Rigidbody>())
                        holdingTarget = colliders[0].transform.root.GetComponent<Rigidbody>();
                    else
                        holdingTarget = colliders[0].transform.parent.GetComponent<Rigidbody>();
                }
                else
                {
                    if (holdingTarget && holdingTarget.GetComponent<InteractableObject>().IsGrabbing(this))
                        UnGrabObject(PlayerClimbController);
                    holdingTarget = null;
                }

                if (holdingTarget)
                    UnGrabObject(PlayerClimbController);

            }
            else
            {
                if (holdingTarget && !holdingTarget.GetComponent<InteractableObject>().IsGrabbing(this))
                    GrabObject(PlayerClimbController);
            }
        }
        else
        {
            if (holdingTarget)
                UnGrabObject(PlayerClimbController);
        }
    }


    private void GrabObject(ClimbController _climbController = null)
    {
        if (holdingTarget.GetComponent<InteractableObject>().Climbable)
            holdingTarget.GetComponent<InteractableObject>().GrabMe(this, _climbController);
        else
            holdingTarget.GetComponent<InteractableObject>().GrabMe(this);
        isGrabbing = true;
    }

    private void UnGrabObject(ClimbController _climbController = null)
    {
        if (holdingTarget && holdingTarget.GetComponent<InteractableObject>().IsGrabbing(this)
            && holdingTarget.GetComponent<InteractableObject>().Climbable)
            holdingTarget.GetComponent<InteractableObject>().UngrabMe(this, _climbController);
        else if (holdingTarget && holdingTarget.GetComponent<InteractableObject>().IsGrabbing(this)
            && !holdingTarget.GetComponent<InteractableObject>().Climbable)
            holdingTarget.GetComponent<InteractableObject>().UngrabMe(this);
        isGrabbing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DistToPickup);
    }
}

//        if (!_handClosed)
//        {
//            Collider[] colliders = Physics.OverlapSphere(transform.position, DistToPickup, _pickupLayer);
//            if (colliders.Length > 0)
//                holdingTarget = colliders[0].transform.root.GetComponent<Rigidbody>();
//            else
//                holdingTarget = null;
//        }
//        else
//        {
//            if(holdingTarget)
//            {
//                //move to hand
//                holdingTarget.velocity = (transform.position - holdingTarget.transform.position) / Time.fixedDeltaTime;

//                //rotate properly
//                holdingTarget.maxAngularVelocity = 20f;
//                Quaternion _deltaRot = transform.rotation * Quaternion.Inverse(holdingTarget.transform.rotation);
//Vector3 _eulerRot = new Vector3(Mathf.DeltaAngle(0, _deltaRot.eulerAngles.x),
//    Mathf.DeltaAngle(0, _deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, _deltaRot.eulerAngles.z));
////_eulerRot *= 0.95f; //Don't know if we want or need this value
//_eulerRot *= Mathf.Deg2Rad;
//                holdingTarget.angularVelocity = _eulerRot / Time.fixedDeltaTime;
//            }
//        }
