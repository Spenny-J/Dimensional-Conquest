using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDistance : MonoBehaviour
{

    

    //test 1-----------
    private bool PrintLocal = false;
    public GameObject testObject;
    //-----------------

    //test 2 ---------- Same as gesture check for force use
    //unreal values
    public GameObject gripObj = null;
    public GameObject ungripObj = null;
    //real values
    private Vector3 gripPosition;
    private Quaternion gripRotation;
    private Vector3 unGripPosition;
    private Quaternion ungripRotation;
    //------------------

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gripPosition = gripObj.transform.localPosition;
        unGripPosition = ungripObj.transform.localPosition;
        Test2();
    }

    private void Test2()
    {
        Vector3 _PositionChecker = new Vector3(transform.localPosition.x, gripPosition.y, transform.localPosition.z);
        float _gripDistance = Vector3.Distance(_PositionChecker, gripPosition);
        float _unGripDistance = Vector3.Distance(_PositionChecker, unGripPosition);
        Vector3 _gripDirection = (gripPosition - _PositionChecker).normalized;
        _gripDirection = transform.InverseTransformDirection(_gripDirection);
        Vector3 _unGripDirection = (unGripPosition - gripPosition).normalized;
        _unGripDirection = gripObj.transform.InverseTransformDirection(_unGripDirection);

        if (PrintLocal)
            print("Grip Direction: " + _gripDirection);
        else
            print("Ungrip Direction: " + _unGripDirection); //This is what we will want to use
        if (Input.GetKeyDown(KeyCode.Space))
            PrintLocal = !PrintLocal;


    }


    private void InitialTest() //test 1 to get local direction
    {
        //Vector3 _PositionChecker = new Vector3(transform.localPosition.x, _gripPosition.y, transform.localPosition.z);
        //float _gripDistance = Vector3.Distance(_PositionChecker, _gripPosition);
        //float _unGripDistance = Vector3.Distance(_PositionChecker, _unGripPosition);
        //Vector3 _gripDirection = (_gripPosition - _PositionChecker).normalized;
        //Vector3 _unGripDirection = (_unGripPosition - _gripPosition).normalized;
        //print("Direction: " + (testObject.transform.localPosition - transform.localPosition).normalized);
        Vector3 _dir = (testObject.transform.localPosition - transform.localPosition).normalized;
        Vector3 _localDirection = transform.rotation * _dir;
        Vector3 _inverseLocal = transform.InverseTransformDirection(_dir);

        if (PrintLocal)
            print("localDirection: " + _localDirection);
        else
            print("inverse local: " + _inverseLocal); //This is what we will want to use
        if (Input.GetKeyDown(KeyCode.Space))
            PrintLocal = !PrintLocal;
    }
}
