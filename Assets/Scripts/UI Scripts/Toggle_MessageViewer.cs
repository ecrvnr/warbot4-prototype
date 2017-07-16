using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used to toggle the view of messages being sent.
    /// </summary>
    public class Toggle_MessageViewer : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/

        /// <summary>
        /// Boolean for activating the view of messages sent
        /// </summary>
        public static bool ToggleMessageViewer;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        void Start() { ToggleMessageViewer = false; }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Toggle the message viewer.
        /// </summary>
        /// <param name="toggle">Boolean</param>
        public void Toggle(bool toggle) { ToggleMessageViewer = toggle; }

    }
}

