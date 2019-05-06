using System;
using UnityEngine;
using MSCLoader;

namespace TommoJProductions.SecureSpareTire
{
    /// <summary>
    /// Represents wheel reset logic for <see cref="ModApi.Attachable.Part.activePart"/>. This Component attaches to <see cref="ModApi.Attachable.Active"/>.
    /// </summary>
    internal class ResetWheelMono : MonoBehaviour
    {
        // Written, 06.05.2019

        /// <summary>
        /// Represents the use playmakerfsm attached to the wheel.
        /// </summary>
        protected internal PlayMakerFSM useFsm { get; private set; }

        /// <summary>
        /// Occurs when this compoenent starts.
        /// </summary>
        private void Start()
        {
            // Written, 06.05.2019

            try
            {
                this.useFsm = PlayMakerFSM.FindFsmOnGameObject(this.gameObject, "Use");

                FsmHook.FsmInject(this.gameObject, "Reset", this.reset);
            }
            catch (Exception ex)
            {
                ModConsole.Error(String.Format("An error occured in [ResetWheelMono.Start] Error stacktrace: {0}", ex.StackTrace));
            }
        }

        private void OnEnable()
        {
            // Written, 06.05.2019

            this.useFsm.SendEvent("RESETWHEEL");
            ModConsole.Print("resetwheel called");
        }

        /// <summary>
        /// Occurs when the attached wheel is reset.
        /// </summary>
        private void reset()
        {
            // Written, 06.05.2019

            ModConsole.Print(String.Format("[ResetWheelMono] - <b>{0}</b> reset", this.useFsm.FsmVariables.FindFsmString("ID").Value));
        }
    }
}
