using System;
using ModApi.Attachable;
using MSCLoader;
using UnityEngine;

namespace TommoJProductions.SecureSpareTire
{
    public class SecureSpareTireMod : Mod
    {
        // Written, 16.03.2019

        #region Mod Properties

        public override string ID => "SecureSpareTire";
        public override string Name => "Secure Spare Tire";
        public override string Version => "0.1";
        public override string Author => "tommojphillips";

        #endregion

        #region Fields

        public const string FILE_NAME = "SecureSpareTire.txt";

        public static string[] vaildWheelNames = new string[] 
        {
            "wheel_regula", // All stock and some aftermarket rims are called this.
            "wheel_offset", // Most aftermarket rims are called this.
        };

        #endregion

        #region Properties

        private SecureTirePart secureTirePart
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public override void OnLoad()
        {
            // Written, 16.03.2019

            GameObject parent = GameObject.Find("SATSUMA(557kg, 248)"); // gameobject that will be attached to spare tire, in this case the satsuma!.
            // Creating trigger for spare tire. and assigning the local location and scale of the trigger related to the parent. (in this case the satsuma).
            Trigger trigger = new Trigger("spareTireTrigger", parent, new Vector3(0, -0.053f, -1.45f), Quaternion.Euler(0, 0, 0), new Vector3(0.3f, 0.3f, 0.24f), false);
            // Creating a new instance of the sparetirepart
            this.secureTirePart = new SecureTirePart(this.loadData(), // Loading saved or default data.
                GameObject.Find("CARPARTS/PartsExtra/wheel_regula") ?? GameObject.Find("wheel_regula"), // the spare tire gameobject instance.
                parent,
                trigger,
                new Vector3(0, -0.053f, -1.45f), // Install position
                Quaternion.Euler(0, 0, 270));// Install rotation
            ModConsole.Print(string.Format("{0} v{1}: Loaded.", this.Name, this.Version));
        }

        public override void OnSave()
        {
            // Written, 17.03.2019

            //this.saveData();
        }
        private PartSaveInfo loadData()
        {
            // Written, 16.08.2019

            try
            {
                return SaveLoad.DeserializeSaveFile<PartSaveInfo>(this, FILE_NAME);
            }
            catch (NullReferenceException)
            {
                // no save file exists.. // setting/loading default save data.

                return this.secureTirePart.defaultPartSaveInfo;
            }
        }

        #endregion
    }
}
