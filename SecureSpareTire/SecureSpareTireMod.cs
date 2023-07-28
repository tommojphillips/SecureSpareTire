using MSCLoader;

using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace TommoJProductions.SecureSpareTire
{
    public partial class SecureSpareTireMod : Mod
    {
        #region Mod Properties

        public override string ID => "SecureSpareTire";
        public override string Name => "Secure Spare Tire";
        public override string Version => VersionInfo.version;
        public override string Author => "tommojphillips";
        public override bool SecondPass => true;

        public override string Description => "Secure a wheel in the satsumas trunk." + DESCRIPTION;

        public static readonly string DESCRIPTION = "\n Latest Release: " + VersionInfo.lastestRelease +
            "\nComplied With: ModApi v" + ModApi.VersionInfo.version + " BUILD " + ModApi.VersionInfo.build + ".";

        #endregion

        public const string FILE_NAME = "SecureSpareTire.txt";

        private Logic logic;

        #region Mod Methods

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, onLoad);
            SetupFunction(Setup.OnSave, onSave);
            SetupFunction(Setup.OnNewGame, newGame);
        }

        internal void onSave()
        {
            // Written, 17.03.2019

            try
            {
                SaveLoad.SerializeSaveFile(this, SecureSpareTire.SaveData.getWheelSaveData(logic.wheelParts), FILE_NAME);
            }
            catch (Exception ex)
            {
                ModConsole.Error("<b>[SecureSpareTireMod]</b> - an error occured while attempting to save part info.. see: " + ex.ToString());
            }
        }
        private void onLoad()
        {

            logic = new Logic();
            logic.onLoad(loadData());

            ModConsole.Print(string.Format("{0} v{1}: Loaded.", Name, Version));
        }
        private void newGame() 
        {
            onSave();
        }

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
