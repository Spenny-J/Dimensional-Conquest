using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*TODO:
 * If dropped on ground deactivate
 */

    public enum BladeState { Activated, Constant, Deactivated, Off}

public class BladeWeaponController : MonoBehaviour
{
    private BladeState _currentState = BladeState.Off;
    private Vector3 _bladeStartPos;

    public Transform BladeObject;
    public float MaxLength;
    public float GrowthRate;

    void Start()
    {
        if (BladeObject)
            _bladeStartPos = BladeObject.localPosition;
    }

    void Update()
    {
        if (_currentState != BladeState.Off)
            UpdateBlade();
    }

    public void ActivateBlade()
    {
        _currentState = BladeState.Activated;
    }

    public void DeactivateBlade()
    {
        _currentState = BladeState.Deactivated;
    }

    public void ToggleBlade()
    {
        if(_currentState == BladeState.Off)
        {
            _currentState = BladeState.Activated;
        }
        else if(_currentState == BladeState.Constant)
        {
            _currentState = BladeState.Deactivated;
        }
    }

    private void UpdateBlade()
    {
        if (_currentState == BladeState.Activated && BladeObject.localScale.y <= MaxLength)
        {
            if(!BladeObject.gameObject.activeSelf)
                BladeObject.gameObject.SetActive(true);
            BladeObject.localScale = new Vector3(BladeObject.localScale.x, BladeObject.localScale.y + GrowthRate * Time.deltaTime, BladeObject.localScale.z);
            BladeObject.localPosition = new Vector3(BladeObject.localPosition.x, BladeObject.localPosition.y + GrowthRate/2 * Time.deltaTime, BladeObject.localPosition.z);
        }
        else if (_currentState == BladeState.Activated && BladeObject.localScale.y >= MaxLength)
        {
            _currentState = BladeState.Constant;
            BladeObject.localScale = new Vector3(BladeObject.localScale.x, MaxLength, BladeObject.localScale.z);
            BladeObject.localPosition = new Vector3(_bladeStartPos.x, _bladeStartPos.y + MaxLength / 2, _bladeStartPos.z);
        }
        if (_currentState == BladeState.Deactivated && BladeObject.localScale.y > 0.1f)
        {
            BladeObject.localScale = new Vector3(BladeObject.localScale.x, BladeObject.localScale.y - GrowthRate * Time.deltaTime, BladeObject.localScale.z);
            BladeObject.localPosition = new Vector3(BladeObject.localPosition.x, BladeObject.localPosition.y - GrowthRate / 2 * Time.deltaTime, BladeObject.localPosition.z);
        }
        else if (_currentState == BladeState.Deactivated && BladeObject.localScale.y <= 0.1f)
        {
            _currentState = BladeState.Off;
            if (BladeObject.gameObject.activeSelf)
                BladeObject.gameObject.SetActive(false);
        }



    }
}
