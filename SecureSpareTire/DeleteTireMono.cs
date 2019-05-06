using UnityEngine;

namespace TommoJProductions.SecureSpareTire
{
    /// <summary>
    /// Represents logic to delete dup tire apon wheel enable.
    /// </summary>
    internal class DeleteTireMono : MonoBehaviour
    {
        // Written, 18.03.2019

        /// <summary>
        /// Occurs as this compoenent starts.
        /// </summary>
        private void Start()
        {
            // Written, 05.05.2019

            if (!this.gameObject.activeInHierarchy)
                this.destoryTire();
        }
        /// <summary>
        /// Occurs on gameobject disable. (Wheel)
        /// </summary>
        private void OnDisable()
        {
            // Written, 05.05.2019

            this.destoryTire();    
        }
        /// <summary>
        /// destorys the tire.
        /// </summary>
        private void destoryTire()
        {
            // Written, 06.05.2019

            // Destorying last child gameobject. (assuming its the tire gameobject)
            Destroy(this.transform.GetChild(this.transform.childCount - 1).gameObject);
        }
    }
}
