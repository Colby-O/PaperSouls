using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;

namespace PaperSouls.Runtime.Weapons
{
    internal class Projectile : MonoBehaviour
    {
        [HideInInspector] public Ray ProjectilePath;
        public float Damage = 10;
        public float Speed = 10;

        [SerializeField] private float _lifeTime = 2;

        private float _time;

        // Projectile won't interact with objects with tags in this List. 
        private List<string> _tagsToIgnore;

        /// <summary>
        /// Appends a list of tags to the tags to ignore list. 
        /// </summary>
        public void AddTagsToIgnore(List<string> tags)
        {
            _tagsToIgnore.AddRange(tags);
        }

        /// <summary>
        /// Appends a tags to the tags to ignore list. 
        /// </summary>
        public void AddTagsToIgnore(string tag)
        {
            _tagsToIgnore.Add(tag);
        }

        /// <summary>
        /// Resets the tags to ignore List to an empty List. 
        /// </summary>
        public void ResetTagsToIgnore()
        {
            _tagsToIgnore = new();
        }

        /// <summary>
        /// Getter for the tags to ignore list. 
        /// </summary>
        public List<string> GetTagsToIgnore()
        {
            return _tagsToIgnore;
        }

        /// <summary>
        /// Sets the ray that defines the path the bullet will follow
        /// </summary>
        public void SetRay(Ray ray)
        {
            ProjectilePath = ray;
        }

        /// <summary>
        /// Check if the GameObject is ignored or not
        /// </summary>
        private bool IgnoreCollider(GameObject other)
        {
            return _tagsToIgnore.Contains(other.tag);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IgnoreCollider(other.gameObject)) return;
            IDamageable damageable = other.transform.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(Damage);
            Destroy(transform.gameObject);
        }

        private void Awake()
        {
            _time = Time.time;
            _tagsToIgnore = new();
            _tagsToIgnore.Add("Item");
        }
        void FixedUpdate()
        {
            if (Time.time - _time > _lifeTime)
            {
                Destroy(transform.gameObject);
            }
            else
            {
                transform.position = ProjectilePath.origin + ProjectilePath.direction * (Time.time - _time) * Speed;
            }
        }
    }
}
