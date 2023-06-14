using UnityEngine;

namespace PaperSouls.Runtime.UI
{
    public class OverheadHealthBar : MonoBehaviour
    {
        private Vector3 _localScale;
        private GameObject _healthBarPrefab;
        private GameObject _healthBar;
        private GameObject _parnet;

        /// <summary>   
        /// Initalizes The Health Bar and instantiates. 
        /// </summary>
        public void HealthBarInit(Vector3 position, Quaternion rotation, GameObject _healthBarPrefab, GameObject _parnet)
        {
            this._healthBarPrefab = _healthBarPrefab;
            this._parnet = _parnet;
            _healthBar = Instantiate(this._healthBarPrefab, position, rotation);
            _healthBar.transform.parent = this._parnet.transform;
            _localScale = _healthBar.transform.localScale;
        }

        /// <summary>   
        /// Update the health value
        /// </summary>
        public void UpdateHealthBar(float healthPercentage)
        {
            _localScale.x *= healthPercentage;
            _healthBar.transform.localScale = _localScale;
        }

        /// <summary>   
        /// Rotates the sprite towards Canera.
        /// TODO: just inherit from Billboard???
        /// </summary>
        private void RotateTowardsCamera()
        {
            Quaternion fromRotate = _healthBar.transform.rotation;
            Quaternion toRotate = Quaternion.Euler(fromRotate.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, fromRotate.eulerAngles.z);

            _healthBar.transform.rotation = toRotate;
        }

        private void Update()
        {
            RotateTowardsCamera();
        }
    }
}