using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ControlsLIB;
using Valve.VR;

public class VRPlayerController : MonoBehaviour
{

    public MoveInDirection MoveDirection = MoveInDirection.DefaultAxis;
    [SerializeField] private SteamVR_Action_Vector2 ThumbstickAction = null; //Will want to add treadmill stuff
    [SerializeField] private SteamVR_Action_Vector2 TrackpadAction = null;
    [SerializeField] private SteamVR_Action_Boolean JumpAction = null;
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public GameObject Collider;
    public GameObject GroundCheckerObj;
    public GameObject RightHand;
    public GameObject LeftHand;
    public bool CanMove = true;

    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;
    private Vector3 _floorPosition;



    void Start()
    {
        _body = transform.GetComponentInParent<Rigidbody>();
        _groundChecker = GroundCheckerObj.transform;
    }

    void Update()
    {
        //Check to see if we are grounded
        _floorPosition = GroundCheckerObj.transform.position;
        _isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, _groundChecker.position.y, transform.position.z), GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if(CanMove && !_isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.down, out hit, 
                transform.position.y - GroundCheckerObj.transform.position.y, Ground, QueryTriggerInteraction.Ignore))
            {
                _isGrounded = true;
            }
        }

        UpdateColliderSize(_isGrounded);

        if (CanMove)
        {
            UpdateInput();
        }
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdateInput()
    {
        //Inputs based on keyboard
        _inputs = Vector3.zero;
        _inputs.x = Input.GetAxis("Horizontal");
        _inputs.z = Input.GetAxis("Vertical");

        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

        //Overriting inputs based on VR controls
        if (ThumbstickAction.axis != Vector2.zero)
        {
            _inputs.x = ThumbstickAction.axis.x;
            _inputs.z = ThumbstickAction.axis.y;
        }

        if (TrackpadAction.axis != Vector2.zero)
        {
            _inputs.x = TrackpadAction.axis.x;
            _inputs.z = TrackpadAction.axis.y;
        }

        if (Input.GetButtonDown("Jump") && _isGrounded) //PC jumping
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        if (JumpAction.stateDown && _isGrounded) //VR jumping
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }

    private void UpdatePosition()
    {
        if (MoveDirection == MoveInDirection.DefaultAxis)
            _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
        if (MoveDirection == MoveInDirection.HMD)
        {
            Vector3 _hmdDir = transform.TransformDirection(_inputs);
            _hmdDir.y = 0f;
            _body.MovePosition(_body.position + _hmdDir * Speed * Time.fixedDeltaTime);
        }
        if (MoveDirection == MoveInDirection.LeftHand)
        {
            Vector3 _leftHandDir = LeftHand.transform.TransformDirection(_inputs);
            _leftHandDir.y = 0f;
            _body.MovePosition(_body.position + _leftHandDir * Speed * Time.fixedDeltaTime);
        }
        if (MoveDirection == MoveInDirection.RightHand)
        {
            Vector3 _rightHandDir = RightHand.transform.TransformDirection(_inputs);
            _rightHandDir.y = 0f;
            _body.MovePosition(_body.position + _rightHandDir * Speed * Time.fixedDeltaTime);
        }
    }

    void UpdateColliderSize(bool _onGround = true)
    {
        if (_onGround)
        {
            float _midPoint = (_floorPosition.y + gameObject.transform.position.y) / 2;
            Collider.transform.position = new Vector3(gameObject.GetComponent<Transform>().position.x, _midPoint, gameObject.GetComponent<Transform>().position.z);
            Collider.GetComponent<CapsuleCollider>().height = Mathf.Abs(_floorPosition.y - gameObject.transform.position.y);
        }
        else
        {
            Collider.transform.position = transform.position;
            Collider.GetComponent<CapsuleCollider>().height = .5f;
        }
        //Have a capsule collider here Collider
        //Position of the floor on Y axis. _floorPosition
        //Position of headset on the Y Axis.
      
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * (transform.position.y - GroundCheckerObj.transform.position.y));
    }
}
