using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Ray ray;
    public float damage;
    public float lifeTime = 2;
    public float speed = 10;
    private float time;

    private List<string> tagsToIgnore;

    public void AddTagsToIgnore(List<string> tags)
    {
        tagsToIgnore.AddRange(tags);
    }

    public void AddTagsToIgnore(string tag)
    {
        tagsToIgnore.Add(tag);
    }

    public void ResetTagsToIgnore()
    {
        tagsToIgnore = new();
    }

    public List<string> GetTagsToIgnore()
    {
        return tagsToIgnore;
    }

    public void SetRay(Ray ray)
    {
        this.ray = ray;
    }

    private bool IgnoreCollider(GameObject other)
    {
        return tagsToIgnore.Contains(other.tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IgnoreCollider(other.gameObject)) return;
        IDamageable damageable = other.transform.GetComponent<IDamageable>();
        if (damageable != null) damageable.Damage(damage);
        Destroy(transform.gameObject);
    }

    private void Awake()
    {
        time = Time.time;
        tagsToIgnore = new();
        tagsToIgnore.Add("Item");
    }
    void FixedUpdate()
    {
        if (Time.time - time > lifeTime)
        {
            Destroy(transform.gameObject);
        } else
        {
            transform.position = ray.origin + ray.direction * (Time.time - time) * speed;
        }
    }

}
