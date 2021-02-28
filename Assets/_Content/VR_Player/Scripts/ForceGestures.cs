using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGestures : MonoBehaviour
{

    [SerializeField] private LayerMask InteractableLayer;


    [SerializeField] private float ForcePushAmount = 30f;
    [SerializeField] private Vector3 ForcePushBoxSize = new Vector3(2f, 5f, 40f);

    /// <summary>
    /// Force push objects in front of user away from user
    /// </summary>
    public void ForcePush()
    {
        Collider[] AllObjectsInCollider = Physics.OverlapBox(transform.position + transform.forward * (ForcePushBoxSize.z / 2f), ForcePushBoxSize / 2, transform.rotation);
        foreach (Collider obj in AllObjectsInCollider)
        {
            Vector3 _objDirection = obj.transform.position - transform.position;

            float objDistance = Vector3.Distance(transform.position, obj.transform.position);

            RaycastHit _hit;
            if (Physics.Raycast(transform.position, _objDirection, out _hit, objDistance, InteractableLayer, QueryTriggerInteraction.Ignore))
            {
                if(obj.GetComponent<Rigidbody>() != null)
                    obj.GetComponent<Rigidbody>().AddForce((transform.up * 5) + transform.forward * ForcePushAmount, ForceMode.VelocityChange);
            }
        }
    }

    public float ForceWavePushAmount = 1000f;
    public float ForceWavePushRadius = 5f;
    /// <summary>
    /// Force wave in all directions
    /// </summary>
    public void ForceWave()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ForceWavePushRadius, InteractableLayer);

        foreach (Collider pushedObject in colliders)
        {
            pushedObject.GetComponent<Rigidbody>().AddExplosionForce(ForceWavePushAmount, Vector3.up, ForceWavePushRadius);
        }
    }

    private Rigidbody[] objects = new Rigidbody[0]; //Debug
    private Vector3[] positions = new Vector3[0]; //Debug
    private bool GrabObjectsPositions = false;
    /// <summary>
    /// Debug for reseting all objects with a rigidbody, specifically for retesting force gesture values
    /// </summary>
    public void ResetObjects()
    {
        if(objects.Length <= 0)
            objects = GameObject.FindObjectsOfType<Rigidbody>();
        if (positions.Length <= 0)
            positions = new Vector3[objects.Length];
        if (!GrabObjectsPositions)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                positions[i] = objects[i].transform.position;
            }
            GrabObjectsPositions = true;
        }
        else
        {
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].velocity = Vector3.zero;
                objects[i].position = positions[i];
            }
        }

    }

    public bool DrawDebugGizmos = true;
    private void OnDrawGizmosSelected()
    {
        if(DrawDebugGizmos)
        {
            Gizmos.DrawWireCube(transform.position + transform.forward * (ForcePushBoxSize.z / 2f), ForcePushBoxSize);
        }
    }
}
