using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace  WarBotEngine.UI {
    /// <summary>
    /// This class is used when the user want to manually delete a Warbot.
    /// </summary>
    public class DeleteUnit : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/

        /// <summary>
        /// The toggle deletion button.
        /// </summary>
        public GameObject Button;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
        *********************************************************************************************/

        /// <summary>
        /// boolean that toggle the deletion of a warbot0
        /// </summary>
        private bool toggle_deletion;


        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
       **********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            toggle_deletion = false;
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            if (toggle_deletion) {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    RaycastHit hit;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                        GameObject objectHit = hit.transform.gameObject;

                        if (objectHit.tag == "WarBot") {
                            Destroy(objectHit);
                        }
                    }
                }
            }
        }

        /******************************************** GUI FUNCTIONS ********************************************
       * This section holds all functions for the user interface.                                             *
       ********************************************************************************************************/
       /// <summary>
       /// Toggle the deletion of warbots.
       /// </summary>
        public void ToggleDeletion() {
            toggle_deletion = !toggle_deletion;
        }
    }
}