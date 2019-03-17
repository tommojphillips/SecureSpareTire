using ModApi;
using ModApi.Attachable;
using UnityEngine;

namespace TommoJProductions.SecureSpareTire
{
    internal class SecureTirePart : Part
    {
        // Written, 16.03.2019

        #region Properties

        public override PartSaveInfo defaultPartSaveInfo => new PartSaveInfo() { installed = true };
        public override GameObject rigidPart
        {
            get;
            set;
        }
        public override GameObject activePart
        {
            get;
            set;
        }

        #endregion

        #region Fields

        private bool _isPartInTrigger = false;
        private GameObject previousTire = null;
        /// <summary>
        /// Represents the tire down rotation.
        /// </summary>
        internal static readonly Quaternion tireDownRotation = Quaternion.Euler(0, 0, 270);
        /// <summary>
        /// Represents the tire up rotation.
        /// </summary>
        internal static readonly Quaternion tireUpRotation = Quaternion.Euler(0, 0, 90);

        #endregion

        #region Constructors

        public SecureTirePart(PartSaveInfo inPartSaveInfo, GameObject inPart, GameObject inParent, Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation) : base(inPartSaveInfo, inPart, inParent, inPartTrigger, inPartPosition, inPartRotation)
        {
            // Written, 17.03.2019
        }

        #endregion

        #region Methods

        protected override void onTriggerExit(Collider inCollider)
        {
            // Written, 17.03.2019

            if (this.isPartCollider(inCollider))
            {
                ModClient.guiAssemble = false;
                this._isPartInTrigger = false;
            }
        }
        
        protected override void onTriggerStay(Collider inCollider)
        {
            // Written, 17.03.2019

            if (this.isPlayerHoldingPart)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (this._isPartInTrigger)
                    {
                        /*if (inCollider.gameObject.transform.rotation.z >= 0 && inCollider.gameObject.transform.rotation.z < 180) // setting rotation of spare tire..
                            this.rigidPart.transform.localRotation = tireUpRotation;
                        else
                            this.rigidPart.transform.localRotation = tireUpRotation;*/
                        this.assemble();
                        return;
                    }
                }

                if (this.isPartCollider(inCollider))
                {
                    ModClient.guiAssemble = true;
                    this._isPartInTrigger = true;
                }
            }
            else
            {
                if (this.previousTire != inCollider.gameObject)
                {
                    foreach (string vaildWheelName in SecureSpareTireMod.vaildWheelNames)
                    {
                        if (inCollider.gameObject.name == vaildWheelName)
                        {
                            // Active Part..
                            this.activePart = inCollider.gameObject;
                            // Rigid Part..
                            Vector3 rigidLocalPosition = this.rigidPart.transform.localPosition; // creating instance of transform to get previous member values of the rigid part..
                            Object.Destroy(this.rigidPart); // Destorying previous rigid (installed) part.
                            this.rigidPart = Object.Instantiate(inCollider.gameObject); // Creating a new gameobject for the rigid (installed) part
                            Object.Destroy(this.rigidPart.GetComponent<Rigidbody>()); // Destorying any rigidbody to stop the gameobject
                            this.rigidPart.AddComponent<Rigid>().part = this; // Adding rigid logic.. the mono contains disassemble logic.
                            this.makePartPickable(false, inPartInstance: PartInstanceTypeEnum.Rigid);// Sets the tag to 'DontPickThis' as this makes the Gameobject not pick-a-up-able.. :)
                            this.rigidPart.transform.SetParent(this.parent.transform, false); // Setting parent of spare tire
                            this.rigidPart.transform.localPosition = rigidLocalPosition; // setting position of spare tire..
                            this.rigidPart.transform.localRotation = tireUpRotation; // setting rotation of spare tire..
                            this.rigidPart.SetActive(false); // As the part is not installed yet (spare tire), and the rigid part has been updated, setting active to false for the rigid (installed) gameobject.
                        }
                    }
                    this.previousTire = inCollider.gameObject;
                }
            }

            //base.onTriggerStay(inCollider);            
        }

        #endregion
    }
}
