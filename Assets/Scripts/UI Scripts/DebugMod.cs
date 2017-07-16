using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.UI {

    /// <summary>
    /// This class is used to toggle the debug panel.
    /// It is also used to set buttons interactable or not.
    /// </summary>
    public class DebugMod : MonoBehaviour {
        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/
        //Hold the reference to the Debug Panel, will be used to enable/disable it.
        public GameObject Debug_Panel;

        /// <summary>
        /// Hold all the buttons of the debug panel. Used to activate/deactivate them.
        /// </summary>
        public GameObject[] buttons;

        /// <summary>
        /// Bool that allow a button to be interactable or not.
        /// </summary>
        private bool isInteractable;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/
        private bool debugModtoggled;

        // Use this for initialization
        void Start() {
            debugModtoggled = false;
            Debug_Panel.SetActive(false);
            isInteractable = true;
        }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Toggle the debug panel.
        /// </summary>
        public void TogglingDebugMod() {
            debugModtoggled = !debugModtoggled;
            Debug_Panel.SetActive(debugModtoggled);
        }

        /// <summary>
        /// This function is used when the user click on a button.
        /// We want the button he clicked to still be interactable and the others to be not.
        /// This way, we can't have multiple functions like delete and create unit at the same time.
        /// </summary>
        /// <param name="NotToggledButton">The button that will remain interactable</param>
        public void ToggleButtons(GameObject NotToggledButton) {
            isInteractable = !isInteractable;
            foreach (GameObject b in buttons) {
                if(b != NotToggledButton)
                    b.GetComponent<Button>().interactable = isInteractable;
            }
        }
    }
}