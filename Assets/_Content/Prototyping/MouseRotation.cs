using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotation : MonoBehaviour
{
    [SerializeField] private bool XRotation = true;
    [SerializeField] private bool YRotaiton = true;

    [SerializeField] private float Speed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        float mouseX = 0;
        float mouseY = 0;

        if (XRotation)
            mouseX = Input.GetAxis("Mouse X");
        if (YRotaiton)
            mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(new Vector3(mouseY, mouseX, 0) * Time.deltaTime * Speed);
    }
}
