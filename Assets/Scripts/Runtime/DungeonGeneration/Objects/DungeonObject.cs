using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    internal class DungeonObject
    {
        public int ID;
        public GameObject GameObject;
        public float Proability;
        public virtual Vector3 Size { get; set; }
        public virtual Vector3 Position { get; set; }

        public DungeonObject(GameObject gameObject, float proability, int id)
        {
            ID = id;
            GameObject = gameObject;
            Size = GetSizeOfObject();
            Proability = proability;
        }

        public DungeonObject(GameObject gameObject, int id) : this(gameObject, 1.0f, id) { }

        public DungeonObject(int id) : this(null, id) { }

        public DungeonObject() { }

        /// <summary>
        /// Gets the size of a room in world space 
        /// </summary>
        protected Vector3 GetSizeOfObject()
        {
            if (GameObject == null) return Vector3.zero;

            Vector3 center = Vector3.zero;
            int childCount = 0;

            Renderer renderer = GameObject.transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                center += renderer.bounds.center;
                childCount += 1;
            }

            foreach (Transform child in GameObject.transform)
            {
                renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    center += renderer.bounds.center;
                    childCount += 1;
                }
            }

            center /= childCount;


            Bounds bounds = new(center, Vector3.zero);

            renderer = GameObject.transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            foreach (Transform child in GameObject.transform)
            {
                renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return bounds.size;
        }

        /// <summary>
        /// Manually set the size of the object.
        /// </summary>
        public void SetSize(Vector3 size)
        {
            this.Size = size;
        }

        /// <summary>
        /// Computes the size of the object in unity coordinates
        /// </summary>
        public void CalculateSize()
        {
            this.Size = GetSizeOfObject();
        }
    }
}
