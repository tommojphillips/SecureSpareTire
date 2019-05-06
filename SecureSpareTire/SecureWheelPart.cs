using System.Linq;
using UnityEngine;
using MSCLoader;
using TommoJProductions.ModApi;
using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Attachable.Options;

namespace TommoJProductions.SecureSpareTire
{
    /// <summary>
    /// Represents the secure tire part for the Satsuma.
    /// </summary>
    internal class SecureWheelPart : Part
    {
        // Written, 16.03.2019

        #region Properties

        /// <summary>
        /// Represents the default save info.
        /// </summary>
        public override PartSaveInfo defaultPartSaveInfo => new PartSaveInfo() { installed = false };
        /// <summary>
        /// Represents the rigid (installed) part.
        /// </summary>
        public override GameObject rigidPart
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the active (free) part.
        /// </summary>
        public override GameObject activePart
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the id of the current wheel.
        /// </summary>
        internal string id
        {
            get;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Represents if the part (active) is in the trigger. | overriding as base is not assignable.
        /// </summary>
        //private new bool isPartInTrigger = false;
        /// <summary>
        /// Represents the tire/wheel that was previously in the trigger.
        /// </summary>
        //private GameObject previousTire = null;
        /// <summary>
        /// Represents the tire down rotation.
        /// </summary>
        //internal static readonly Quaternion tireDownRotation = Quaternion.Euler(0, 0, 270);
        /// <summary>
        /// Represents the tire up rotation.
        /// </summary>
        internal static readonly Quaternion tireUpRotation = Quaternion.Euler(0, 0, 90);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of this.
        /// </summary>
        /// <param name="inPartSaveInfo">The save info.</param>
        /// <param name="inPart">The part to base off.</param>
        /// <param name="inParent">The part's Parent gameobject.</param>
        /// <param name="inPartTrigger">The part's trigger.</param>
        /// <param name="inPartPosition">The part's installed position. (local; relevant to the parent)</param>
        /// <param name="inPartRotation">The part's installed rotation. (local; relevant to the parent)</param>
        public SecureWheelPart(PartSaveInfo inPartSaveInfo, PartOptions inPartOptions, GameObject inPart, GameObject inParent, Trigger inPartTrigger, Vector3 inPartPosition, Quaternion inPartRotation) : base(inPartSaveInfo, inPartOptions, inPart, inParent, inPartTrigger, inPartPosition, inPartRotation)
        {
            // Written, 23.03.2019

            this.id = PlayMakerFSM.FindFsmOnGameObject(inPart, "Use").FsmVariables.FindFsmString("ID").Value;

            // Setting up active part.
            this.activePart.AddComponent<DeleteTireMono>();
            this.activePart.AddComponent<ResetWheelMono>();
            //this.activePart.transform.position = inPart.transform.position;

            // Setting up rigid part.

            // Destorying all but the last child gameobject. (assuming the last child is the tire gameobject)
            for (int i = 0; i < this.rigidPart.transform.childCount - 1; i++)
                Object.Destroy(this.rigidPart.transform.GetChild(i).gameObject);
            // Destorying all but mesh renders and mesh filters on rigid wheel.
            foreach (Object _obj in this.rigidPart.GetComponents<Object>().Where(obj => !(obj is MeshRenderer) && !(obj is MeshFilter) && !(obj is BoxCollider) && !(obj is MeshCollider) && !(obj is Rigid)))
                Object.Destroy(_obj);
            this.rigidPart.rename("spare wheel");

            ModConsole.Print("SecureWheelPart Initialized for: " + this.id);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The trigger exit logic.
        /// </summary>
        /// <param name="inCollider">The collider that the trigger detected.</param>
        /*protected override void onTriggerExit(Collider inCollider)
        {
            // Written, 17.03.2019

            if (this.isPartCollider(inCollider))
            {
                ModClient.guiAssemble = false;
                this.isPartInTrigger = false;
            }
        }
        /// <summary>
        /// The trigger stay logic.
        /// </summary>
        /// <param name="inCollider">The collider that the trigger detected.</param>
        protected override void onTriggerStay(Collider inCollider)
        {
            // Written, 17.03.2019

            if (this.isPlayerHoldingPart)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (this.isPartInTrigger)
                    {
                        this.assemble();
                        return;
                    }
                }

                if (this.isPartCollider(inCollider))
                {
                    ModClient.guiAssemble = true;
                    this.isPartInTrigger = true;
                }
            }
            else
            {
                // wheel might not match and has been changed/modified.

                if (this.previousTire != inCollider.gameObject)
                {
                    GameObject[] wheels = Object.FindObjectsOfType<GameObject>().Where(go => SecureSpareTireMono.wheelTypeNames.Values.Any(wheelType => wheelType == go.name)).ToArray();
                    wheels = wheels.Where(wheel => SecureSpareTireMono.wheelIDNames.Values.Any(wheelID => PlayMakerFSM.FindFsmOnGameObject(wheel, "Use").FsmVariables.FindFsmString("ID").Value.Contains(wheelID)) // if the id is equal to wheel_steel..
                    && PlayMakerFSM.FindFsmOnGameObject(wheel, "Use").FsmVariables.FindFsmString("Corner").Value == string.Empty).ToArray();

                    GameObject wheelInPlayerHand = wheels.FirstOrDefault(wheel => wheel.isPlayerHolding());

                    if (inCollider.gameObject == wheelInPlayerHand)
                    {
                        ModConsole.Print("found wheel: " + wheelInPlayerHand);
                    }
                    /*foreach (string vaildWheelName in SecureSpareTireMod.instancewheelTypeNames.Values)
                    {
                        if (inCollider.gameObject.name == vaildWheelName) // Checking if the detected gameobject (in trigger) is a wheel..
                        {
                            Object.Destroy(this.activePart.GetComponent<SecureWheelMono>());
                            // Active Part.. assignment
                            this.activePart = inCollider.gameObject;
                            this.activePart.AddComponent<SecureWheelMono>();
                            // Rigid Part.. assignment
                            Vector3 rigidLocalPosition = this.rigidPart.transform.localPosition; // creating instance of transform to get previous member values of the rigid part..
                            Object.Destroy(this.rigidPart); // Destorying previous rigid (installed) part.
                            this.rigidPart = Object.Instantiate(this.activePart); // Creating a new gameobject for the rigid (installed) part
                            Object.Destroy(this.rigidPart.GetComponent<Rigidbody>()); // Destorying any rigidbody to stop the gameobject
                            this.rigidPart.AddComponent<Rigid>().part = this; // Adding rigid logic.. the mono contains disassemble logic.
                            this.makePartPickable(false, inPartInstance: PartInstanceTypeEnum.Rigid);// Sets the tag to 'DontPickThis' as this makes the Gameobject not pick-a-up-able.. :)
                            this.rigidPart.transform.SetParent(this.parent.transform, false); // Setting parent of spare tire
                            this.rigidPart.transform.localPosition = rigidLocalPosition; // setting position of spare tire..
                            this.rigidPart.transform.localRotation = tireUpRotation; // setting rotation of spare tire..
                            this.rigidPart.SetActive(false); // As the part is not installed yet (spare tire), and the rigid part has been updated, setting active to false for the rigid (installed) gameobject.
                            break;
                        }
                    }//
                    this.previousTire = inCollider.gameObject;
                }
            }     
        }*/

        #endregion
    }
}
