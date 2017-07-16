using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used when the user want informations about the UI.
    /// </summary>
    public class UI_Information : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/

        /// <summary>
        /// The GameObject that hold the informations.
        /// </summary>
        public GameObject InformationText;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/
        void Start() {
            InformationText.SetActive(false);
        }

        /******************************************** GUI FUNCTIONS ********************************************
         * This section holds all functions for the user interface.                                             *
         ********************************************************************************************************/
         /// <summary>
         /// Show the text given in parameter in the InformationText GameObject.
         /// </summary>
         /// <param name="text">The desired text.</param>
        public void ShowUIInformations(string text) {
            InformationText.SetActive(true);
            InformationText.transform.Find("Text").GetComponent<Text>().text = text;
        }

        /// <summary>
        /// Hide the information box.
        /// Used when the user move the mouse out a of GUI element.
        /// </summary>
        public void HideUIInformations() { InformationText.SetActive(false); }
    }


}
