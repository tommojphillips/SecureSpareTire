using MSCLoader;

using System;
using System.Collections.Generic;
using System.Linq;

using TommoJProductions.ModApi;
using TommoJProductions.ModApi.Attachable;

namespace TommoJProductions.SecureSpareTire
{
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
}
