using System;
using MSCLoader;
using UnityEngine;

namespace TommoJProductions.SecureSpareTire
{
    public class SecureSpareWheelMod : Mod
    {
        // Written, 16.03.2019

        #region Mod Properties

        public override string ID => "SecureSpareTire";
        public override string Name => "Secure Spare Tire";
        public override string Version => "0.2.1";
        public override string Author => "tommojphillips";

        #endregion

        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// Represents the instance of <see cref="SecureSpareWheelMod"/>.
        /// </summary>
        internal static SecureSpareWheelMod instance
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Occurs on game load.
        /// </summary>
        public override void OnLoad()
        {
            // Written, 05.05.2019)

            instance = this;

            GameObject secureSpareTireGameObject = new GameObject(string.Format("{0} v{1}", this.Name, this.Version));
            secureSpareTireGameObject.AddComponent<SecureSpareWheelMono>();
            ModConsole.Print(string.Format("{0}: Loaded.", secureSpareTireGameObject.name));
        }
        /// <summary>
        /// Occurs when player saves the game!
        /// </summary>
        public override void OnSave()
        {
            // Written, 05.05.2019

            //this.secureSpareTireMono.onSave();
        }        

        #endregion
    }
}
