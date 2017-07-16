using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used to activate the manual creation of WarBots.
    /// </summary>
    public class ToggleCreation : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/

        /// <summary>
        /// The toggle deletion button.
        /// </summary>
        public GameObject Creation_Panel;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
        *********************************************************************************************/

        /// <summary>
        /// boolean that toggle the deletion of a warbot0
        /// </summary>
        private bool toggle_panel;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
       **********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            toggle_panel = false;
            Creation_Panel.SetActive(false);
        }

        /******************************************** GUI FUNCTIONS ********************************************
       * This section holds all functions for the user interface.                                             *
       ********************************************************************************************************/

        /// <summary>
        /// Toggle the creation panel.
        /// </summary>
        public void ToggleCreationPanel() {
            toggle_panel = !toggle_panel;
            
            Creation_Panel.SetActive(toggle_panel);
        }

    }
}