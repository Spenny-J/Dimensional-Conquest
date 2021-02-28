using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
 * - Get controller delta to offset our playspace 
 */

public class ClimbHand : MonoBehaviour
{
    public Vector3 ControllerDelta = Vector3.zero;

    private Vector3 _lastPosition = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;

    private void Start()
    {
        _lastPosition = transform.localPosition;
        _currentPosition = transform.localPosition;
    }

    private void Update()
    {
        _currentPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        ControllerDelta = _lastPosition - _currentPosition;
        _lastPosition = _currentPosition;
    }

}
