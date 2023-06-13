using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public readonly float turnSpeed = 10.0f;
    public Transform rotateTarget;

    protected void RotateTowardsCamera()
    {
        Vector3 lookDirection = (transform.position - rotateTarget.position).normalized;
        Quaternion freeRotation = Quaternion.LookRotation(lookDirection, transform.up);

        float newRotationY = transform.eulerAngles.y;
        float differneceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;

        if (Mathf.Abs(differneceRotation) > 0) newRotationY = freeRotation.eulerAngles.y;

        Vector3 newRotation = new(0, newRotationY, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRotation), turnSpeed * Time.deltaTime);
    }

    public virtual void Awake()
    {
        rotateTarget = Camera.main.transform;
    }

    public virtual void LateUpdate()
    {
        RotateTowardsCamera();
    }
}
