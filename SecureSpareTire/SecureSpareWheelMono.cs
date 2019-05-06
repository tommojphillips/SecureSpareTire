using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MSCLoader;
using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Attachable.Options;

namespace TommoJProductions.SecureSpareTire
{
    internal class SecureSpareWheelMono : MonoBehaviour
    {
        // Written, 05.05.2019

        #region Fields

        /// <summary>
        /// Represents the file name for the save file.
        /// </summary>
        private const string FILE_NAME = "SecureSpareTire.txt";
        /// <summary>
        /// Represents all vaild wheel names (<see cref="UnityEngine.Object.name"/>).
        /// </summary>
        internal static Dictionary<WheelTypeEnum, string> wheelTypeNames = new Dictionary<WheelTypeEnum, string>()
        {
            { WheelTypeEnum.wheel_regula, "wheel_regula" },
            { WheelTypeEnum.wheel_offset, "wheel_offset" }
        };
        /// <summary>
        /// Represents all wheel id names.
        /// </summary>
        internal static Dictionary<WheelIDEnum, string> wheelIDNames = new Dictionary<WheelIDEnum, string>()
        {
            { WheelIDEnum.wheel_steel, "wheel_steel" },
        };
        /// <summary>
        /// Represents all wheel ids and their associated wheel type.
        /// </summary>
        private static Dictionary<WheelIDEnum, WheelTypeEnum> wheels = new Dictionary<WheelIDEnum, WheelTypeEnum>()
        {
            { WheelIDEnum.wheel_steel, WheelTypeEnum.wheel_regula },
        };

        #endregion

        #region Properties

        /// <summary>
        /// Represents the secure spare tire part.
        /// </summary>
        private SecureWheelPart secureTirePart
        {
            get;
            set;
        }

        #endregion

        private void Start()
        {
            // Written, 05.05.2019

            try
            {
                SecureWheelSaveData saveData = loadData();
                GameObject wheel = null;

                if (saveData is null)
                {
                    // initial mod load.

                    ModConsole.Print(String.Format("[Tire Mod] - Initial mod load."));

                    // finding stock wheel that is not attached to car.. (preferably the orignal sparetire).

                    GameObject[] stockWheels = FindObjectsOfType<GameObject>().Where(go => go.name == wheelTypeNames[WheelTypeEnum.wheel_regula]).ToArray(); // Getting all wheels of type, regular.
                    int stockWheelsFound = stockWheels.Length;
                    stockWheels = stockWheels
                        .Where(go => PlayMakerFSM.FindFsmOnGameObject(go, "Use").FsmVariables.FindFsmString("ID").Value.Contains(wheelIDNames[WheelIDEnum.wheel_steel]) // if the id is equal to wheel_steel..
                        && PlayMakerFSM.FindFsmOnGameObject(go, "Use").FsmVariables.FindFsmString("Corner").Value == string.Empty) // and if the corner is equal to nothing..
                        .ToArray(); // Getting only stock wheels (wheel_steel) that are not installed on the car.
                    ModConsole.Print(String.Format("[Tire Mod] - Found {0} stock wheel/s {1} of which are vaild.", stockWheelsFound, stockWheels.Length));
                    bool foundPreferedTire = false;
                    for (int i = 0; i < stockWheels.Length; i++)
                    {
                        GameObject stockWheel = stockWheels[i];
                        if (PlayMakerFSM.FindFsmOnGameObject(stockWheel, "Use").FsmVariables.FindFsmString("ID").Value == wheelIDNames[WheelIDEnum.wheel_steel] + "5")
                        {
                            ModConsole.Print(String.Format("[Tire Mod] - prefered startup tire (steel_wheel5) is available for use."));
                            foundPreferedTire = true;
                            wheel = stockWheel;
                            break;
                        }
                    }
                    if (!foundPreferedTire)
                    {
                        ModConsole.Print(String.Format("[Tire Mod] - prefered startup tire (steel_wheel5) is not availale for use. using lowest instanced available stock wheel."));
                        wheel = stockWheels.OrderBy(sw => PlayMakerFSM.FindFsmOnGameObject(sw, "Use").FsmVariables.FindFsmString("ID").Value).ToArray()[0]; // gwtting most lowest instance of stock wheel. eg. (stock_wheel2 over stock_wheel4).
                    }
                }
                else
                {
                    // normal mod load.

                    ModConsole.Print(String.Format("[Tire Mod] - Normal mod load."));
                }
                ModConsole.Print(String.Format("[Tire Mod] - Using Wheel, {0}", PlayMakerFSM.FindFsmOnGameObject(wheel, "Use").FsmVariables.FindFsmString("ID").Value));

                // Setting up attachable part.

                GameObject parent = GameObject.Find("SATSUMA(557kg, 248)"); // gameobject that will be attached to spare tire, in this case the satsuma!.
                                                                            // Creating trigger for spare tire. and assigning the local location and scale of the trigger related to the parent. (in this case the satsuma).
                Trigger trigger = new Trigger("spareTireTrigger", parent, new Vector3(0, -0.053f, -1.45f), Quaternion.Euler(0, 0, 0), new Vector3(0.3f, 0.3f, 0.24f), false);
                // Creating a new instance of the sparetirepart
                this.secureTirePart = new SecureWheelPart(null, // Loading saved or default data.
                    new PartOptions()
                    {
                        partInitializationProcedure = PartInitializationProcedureEnum.Assign,
                        activePartAddRigidbodyComponent = false,
                        activePartSetPositionRotation = false
                    },
                    wheel,
                    parent,
                    trigger,
                    new Vector3(0, -0.053f, -1.45f), // Install position
                    SecureWheelPart.tireUpRotation);// Install rotation

                ModConsole.Print(String.Format("[Tire Mod] - SecureSpareTireMono: Started"));
            }
            catch (Exception ex)
            {
                ModConsole.Error(String.Format("[sstm.start] - <b>StackTrace:</b>\r\n{0}\r\n", ex.StackTrace));
            }
        }

        /// <summary>
        /// Loads Tire save data.
        /// </summary>
        internal static SecureWheelSaveData loadData()
        {
            // Written, 05.05.2019

            try
            {
                return SaveLoad.DeserializeSaveFile<SecureWheelSaveData>(SecureSpareWheelMod.instance, FILE_NAME);
            }
            catch (NullReferenceException)
            {
                // no save file exists.. // setting/loading default save data.

                return null; // As of "Mod API v0.1.1.4-alpha"; will auto load default save data for the part if null is passed..
            }
        }

        internal void onSave()
        {
            // Written, 05.05.2019

            try
            {
                SecureWheelSaveData saveData = new SecureWheelSaveData();
                saveData.partSaveInfo = this.secureTirePart.getSaveInfo();
                SaveLoad.SerializeSaveFile(SecureSpareWheelMod.instance, saveData, FILE_NAME);
            }
            catch (Exception ex)
            {
                ModConsole.Error("<b>[SecureSpareTireMod]</b> - an error occured while attempting to save part info.. see: " + ex.ToString());
            }
        }
    }
}
