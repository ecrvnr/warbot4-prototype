using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarBotEngine.WarBots;

namespace WarBotEngine.UI {

    /// <summary>
    /// Show informations for a given warbot, selected by the user.
    /// </summary>
    public class ShowInformationWarbot : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/

        /// <summary>
        /// The panel showing the informations of the warbot.
        /// </summary>
        public GameObject panel_Informations;

        /// <summary>
        /// Toggle the informations.
        /// </summary>
        private bool toggle_Informations;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/
        
        /// <summary>
        /// The text showed in the information panel.
        /// </summary>
        private Text text_Informations;

        /// <summary>
        /// The current selected warbot.
        /// </summary>
        private WarBotController CurrentWarBot;


        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            toggle_Informations = false;
            panel_Informations.SetActive(false);

            text_Informations = panel_Informations.transform.Find("Informations").gameObject.GetComponent<Text>();
            text_Informations.text = "";
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            if (toggle_Informations) {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    RaycastHit hit;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                        CurrentWarBot = hit.transform.gameObject.GetComponent<WarBotController>();
                    }
                }

                if(CurrentWarBot != null)
                    text_Informations.text = CurrentWarBot.Type.WarType() + "\n" + 
                        CurrentWarBot.HealthController.CurrentHealth + "/" + CurrentWarBot.HealthController.max_health + "\n" + 
                        CurrentWarBot.GetComponent<InventoryController>().CurrentInventory + "/" + CurrentWarBot.GetComponent<InventoryController>().max_inventory + "\n";
                else 
                    text_Informations.text = "";
            }

        }

        /// <summary>
        /// Toggle the information mode.
        /// </summary>
        public void TogglingShowInfos() {
            toggle_Informations = !toggle_Informations;

            panel_Informations.SetActive(toggle_Informations);

        }

    }

}