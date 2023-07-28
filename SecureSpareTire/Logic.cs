using MSCLoader;

using System.Linq;
using System;

using UnityEngine;
using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Database;
using TommoJProductions.ModApi;

namespace TommoJProductions.SecureSpareTire
{
    internal class Logic
    {

        public static string[] vaildWheelNames = new string[]
        {
            "wheel_regula", // All stock and some aftermarket rims are called this.
            "wheel_offset", // Most aftermarket rims are called this.
        };

        private SaveData saveData;

        internal Part[] wheelParts;

        internal void onLoad(SaveData saveData)
        {
            // Written, 20.09.2021

            GameObject satsuma = Database.databaseVehicles.satsuma;
            GameObject[] wheels = UnityEngine.Object.FindObjectsOfType<GameObject>().Where(go => vaildWheelNames.Any(vwn => vwn == go.name)).ToArray();
            wheelParts = new Part[wheels.Length];

            this.saveData = saveData;

            Trigger trigger = new Trigger("spareTireTrigger", satsuma, new Vector3(0, -0.053f, -1.45f), Vector3.forward * 90);
            trigger.onPartPreAssembledToTrigger -= trigger_onPartPreAssembledToTrigger;
            trigger.onPartPreAssembledToTrigger += trigger_onPartPreAssembledToTrigger;
            AssemblyTypeJointSettings jointSettings = new AssemblyTypeJointSettings(satsuma.GetComponent<Rigidbody>());
            PartSettings settings = new PartSettings()
            {
                assembleType = AssembleType.joint,
                setPositionRotationOnInitialisePart = false,
                assemblyTypeJointSettings = jointSettings,
                setPhysicsMaterialOnInitialisePart = true,
                installedPartToLayer = LayerMasksEnum.DontCollide
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
        }
        
        private void trigger_onPartPreAssembledToTrigger(Trigger arg1)
        {
            // Written, 26.06.2022

            float rot = Vector3.Dot(arg1.triggerCallback.part.transform.up, Vector3.up);
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
          
    }
}
