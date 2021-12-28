using System;
using System.Linq;
using MSCLoader;
using TommoJProductions.ModApi;
using TommoJProductions.ModApi.Attachable;
using UnityEngine;
using static TommoJProductions.ModApi.Attachable.Part;
using Object = UnityEngine.Object;

namespace TommoJProductions.SecureSpareTire
{
    public class SecureSpareTireMod : Mod
    {
        // Written, 16.03.2019 | Modified, 25.09.2021

        #region mod classes

        public class SaveData 
        {
            public string installedWheelID;
            public static SaveData getWheelSaveData(Part[] parts)
            {
                SaveData sd = new SaveData();
                int indexOfInstalled = Array.IndexOf(parts, parts.Where(_p => _p.installed).ToArray()?[0]);
                if (indexOfInstalled > -1)
                    sd.installedWheelID = parts[indexOfInstalled].gameObject.GetPlayMaker("Use").FsmVariables.GetFsmString("ID").Value;
                return sd;
            }
        }

        #endregion


        #region Mod Properties

        public override string ID => "SecureSpareTire";
        public override string Name => "Secure Spare Tire";
        public override string Version => "0.1.2";
        public override string Author => "tommojphillips";
        public override bool SecondPass => true;
        
        #endregion

        #region Fields

        public const string FILE_NAME = "SecureSpareTire.txt";

        public static string[] vaildWheelNames = new string[] 
        {
            "wheel_regula", // All stock and some aftermarket rims are called this.
            "wheel_offset", // Most aftermarket rims are called this.
        };

        private SaveData saveData;
        private Part[] wheelParts;

        #endregion

        #region Mod Methods

        public override void SecondPassOnLoad()
        {
            // Written, 20.09.2021

            GameObject satsuma = GameObject.Find("SATSUMA(557kg, 248)");
            GameObject[] wheels = Object.FindObjectsOfType<GameObject>().Where(go => vaildWheelNames.Any(vwn => vwn == go.name)).ToArray();
            wheelParts = new Part[wheels.Length];

            saveData = loadData();

            Trigger trigger = new Trigger("spareTireTrigger", satsuma,  new Vector3(0, -0.053f, -1.45f), Vector3.forward * 90);
            AssemblyTypeJointSettings jointSettings = new AssemblyTypeJointSettings(satsuma.GetComponent<Rigidbody>());
            PartSettings settings = new PartSettings() { assembleType = AssembleType.joint, installedPartToLayer = LayerMasksEnum.DontCollide, setPositionRotationOnInitialisePart = false, assemblyTypeJointSettings = jointSettings };

            for (int i = 0; i < wheels.Length; i++)
            {
                GameObject wheel = wheels[i];

                PlayMakerFSM useFsm = wheel.GetPlayMaker("Use");
                PlayMakerFSM removalFsm = wheel.GetPlayMaker("Removal");
                string wheelID = useFsm.FsmVariables.GetFsmString("ID").Value;
                bool isThisWheelInstall = false;
                if (saveData?.installedWheelID == wheelID)
                {
                    isThisWheelInstall = useFsm.FsmVariables.GetFsmString("Corner").Value == "";
                }

                wheelParts[i] = wheel.AddComponent<Part>();
                wheelParts[i].defaultSaveInfo = new PartSaveInfo() { installed = wheelID == "wheel_steel5" && !removalFsm.enabled };
                wheelParts[i].initPart(isThisWheelInstall ? new PartSaveInfo() { installed = true } : null, settings, trigger);
            }
            ModConsole.Print(string.Format("{0} v{1}: Loaded.", Name, Version));
        }
        public override void OnSave()
        {
            // Written, 17.03.2019

            try
            {
                SaveLoad.SerializeSaveFile(this, SaveData.getWheelSaveData(wheelParts), FILE_NAME);
            }
            catch (Exception ex)
            {
                ModConsole.Error("<b>[SecureSpareTireMod]</b> - an error occured while attempting to save part info.. see: " + ex.ToString());
            }
        }

        #endregion

        #region Methods        

        private SaveData loadData()
        {
            // Written, 16.08.2019

            try
            {
                return SaveLoad.DeserializeSaveFile<SaveData>(this, FILE_NAME);
            }
            catch (NullReferenceException)
            {
                // no save file exists.. // setting/loading default save data.

                return null;
            }
        }

        #endregion

    }
}
