using MSCLoader;
using System;
using System.Collections.Generic;
using System.Linq;
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
            public bool installedRightSideUp;

            public static SaveData getWheelSaveData(Part[] parts)
            {
                SaveData sd = new SaveData();
                IEnumerable<Part> installedParts = parts.Where(_p => _p.installed);
                if (installedParts.Any())
                {
                    int indexOfInstalled = Array.IndexOf(parts, installedParts.First());
                    sd.installedWheelID = parts[indexOfInstalled].gameObject.GetPlayMaker("Use").FsmVariables.GetFsmString("ID").Value;
                    sd.installedRightSideUp = parts[indexOfInstalled].triggers[0].triggerGameObject.transform.localEulerAngles.z.round() <= 90;
                }
                return sd;
            }
        }

        #endregion

        #region Mod Properties

        public override string ID => "SecureSpareTire";
        public override string Name => "Secure Spare Tire";
        public override string Version => VersionInfo.version;
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

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, onLoad);
            SetupFunction(Setup.OnSave, onSave);
            SetupFunction(Setup.OnNewGame, newGame);
        }

        private void onLoad()
        {
            // Written, 20.09.2021

            GameObject satsuma = GameObject.Find("SATSUMA(557kg, 248)");
            GameObject[] wheels = Object.FindObjectsOfType<GameObject>().Where(go => vaildWheelNames.Any(vwn => vwn == go.name)).ToArray();
            wheelParts = new Part[wheels.Length];

            saveData = loadData();

            Trigger trigger = new Trigger("spareTireTrigger", satsuma, new Vector3(0, -0.053f, -1.45f), Vector3.forward * 90);
            trigger.onPartPreAssembledToTrigger -= trigger_onPartPreAssembledToTrigger;
            trigger.onPartPreAssembledToTrigger += trigger_onPartPreAssembledToTrigger;
            AssemblyTypeJointSettings jointSettings = new AssemblyTypeJointSettings(satsuma.GetComponent<Rigidbody>());
            PartSettings settings = new PartSettings()
            {
                assembleType = AssembleType.joint,
                setPositionRotationOnInitialisePart = false,
                assemblyTypeJointSettings = jointSettings,
                setPhysicsMaterialOnInitialisePart = true
            };
            GameObject wheel;
            PlayMakerFSM useFsm;
            PlayMakerFSM removalFsm;
            string wheelID;
            bool canWheelBeInstalled;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheel = wheels[i];
                useFsm = wheel.GetPlayMaker("Use");
                removalFsm = wheel.GetPlayMaker("Removal");
                wheelID = useFsm.FsmVariables.GetFsmString("ID").Value;
                canWheelBeInstalled = false;

                if (saveData?.installedWheelID == wheelID)
                {
                    canWheelBeInstalled = useFsm.FsmVariables.GetFsmString("Corner").Value == "";
                }

                wheelParts[i] = wheel.AddComponent<Part>();
                wheelParts[i].defaultSaveInfo = new PartSaveInfo() { installed = saveData == null && wheelID == "wheel_steel5" && !removalFsm.enabled };
                wheelParts[i].initPart(canWheelBeInstalled ? new PartSaveInfo() { installed = true } : null, settings, trigger);
            }
            ModConsole.Print(string.Format("{0} v{1}: Loaded.", Name, Version));
        }

        private void trigger_onPartPreAssembledToTrigger(Trigger arg1)
        {
            // Written, 26.06.2022

            float rot = Vector3.Dot(arg1.triggerCallback.part.transform.up, Vector3.up);
            ModConsole.Print(rot);
            if (rot > 0 && rot < 270)
            {
                rot = 90;
            }
            else
            {
                rot = 270;
            }
            arg1.triggerGameObject.transform.localEulerAngles = Vector3.forward * rot;
        }

        private void onSave()
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
        private void newGame() 
        {
            onSave();
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
