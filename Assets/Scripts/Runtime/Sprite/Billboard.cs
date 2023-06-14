using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Sprite
{
    public class Billboard : MonoBehaviour
    {
        private readonly float _turnSpeed = 10.0f;
        public Transform RotateTarget { get; set; }

        /// <summary>
        /// Rotates the Billbaord towards the target
        /// </summary>
        protected void RotateTowardsCamera()
        {
            Vector3 lookDirection = (transform.position - RotateTarget.position).normalized;
            Quaternion freeRotation = Quaternion.LookRotation(lookDirection, transform.up);

            float newRotationY = transform.eulerAngles.y;
            float differneceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;

            if (Mathf.Abs(differneceRotation) > 0) newRotationY = freeRotation.eulerAngles.y;

            Vector3 newRotation = new(0, newRotationY, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(newRotation), _turnSpeed * Time.deltaTime);
        }

        public virtual void Awake()
        {
            RotateTarget = Camera.main.transform;
        }

        public virtual void LateUpdate()
        {
            RotateTowardsCamera();
        }
    }
}
