using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadHealthBar : MonoBehaviour
{
    private Vector3 localScale;
    private GameObject healthBarPrefab;
    private GameObject healthBar;
    private GameObject parnet;

    public void HealthBarInit(Vector3 position, Quaternion rotation, GameObject healthBarPrefab, GameObject parnet)
    {
        this.healthBarPrefab = healthBarPrefab;
        this.parnet = parnet;
        healthBar = Instantiate(healthBarPrefab, position, rotation);
        healthBar.transform.parent = parnet.transform;
        localScale = healthBar.transform.localScale;
    }

    public void UpdateHealthBar(float healthPercentage)
    {
        localScale.x *= healthPercentage;
        healthBar.transform.localScale = localScale;
    }

    void RotateTowardsCamera()
    {
        Quaternion fromRotate = healthBar.transform.rotation;
        Quaternion toRotate = Quaternion.Euler(fromRotate.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, fromRotate.eulerAngles.z);

        healthBar.transform.rotation = toRotate;
    }

    private void Update()
    {
        RotateTowardsCamera();
    }
}