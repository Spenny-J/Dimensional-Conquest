using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


/* TODO:
 * - Check if one hand is grabbing
 * - Set the latest grabbed hand as the main hand
 * - Automatically ungrab the first hand (or figure out what the inbetween of the two offsets would be)
 * - Update playspace position
 */

[RequireComponent(typeof(VRPlayerController))]
public class ClimbController : MonoBehaviour
{
    //Need info on hands and "ground" which is the playerrig
    private VRPlayerController _playerController;
    private GameObject _cameraRig;

    private ClimbHand _currentHand = null;

    private bool _climbing = false;

    [SerializeField] private LayerMask GrabMask;

    [SerializeField] private float ClimbingSpeed = 45f;


    private void Start()
    {
        _playerController = gameObject.GetComponent<VRPlayerController>();
        _cameraRig = _playerController.GroundCheckerObj;
    }

    private void Update()
    {
        if (_currentHand)
            _climbing = true;
        else
            _climbing = false;
    }

    private void FixedUpdate()
    {
        if(_currentHand && _climbing)
        {
            UpdateClimbPosition(_currentHand);
        }
    }

    private void UpdateClimbPosition(ClimbHand climbHand)
    {
        //This will be weird: _cameraRig.GetComponent<Rigidbody>().MovePosition(_cameraRig.transform.position + climbHand.ControllerDelta * ClimbingSpeed * Time.fixedDeltaTime);
        Vector3 movement = Vector3.zero;
        movement += climbHand.ControllerDelta * ClimbingSpeed;
        //print(climbHand.gameObject+ " " + climbHand.ControllerDelta);
        _cameraRig.GetComponent<Rigidbody>().MovePosition(_cameraRig.transform.position + movement * Time.fixedDeltaTime);
        //_cameraRig.GetComponent<Rigidbody>().velocity = (_cameraRig.transform.position + climbHand.ControllerDelta * ClimbingSpeed * Time.fixedDeltaTime);
    }

    public void StartClimbObject(GameObject climbHand)
    {
        _currentHand = climbHand.GetComponent<ClimbHand>();
        if (_cameraRig.GetComponent<Rigidbody>().useGravity)
        {
            _cameraRig.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _cameraRig.GetComponent<Rigidbody>().useGravity = false;
        }
        if (_playerController.CanMove)
            _playerController.CanMove = false;
    }

    public void DropHand (GameObject climbHand)
    {
        if (_currentHand == climbHand.GetComponent<ClimbHand>())
        {
            _currentHand = null;
            _cameraRig.GetComponent<Rigidbody>().useGravity = true;
            _playerController.CanMove = true;
        }
    }
}