using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used when the user want to manualy move a warbot.
    /// </summary>
    public class MoveUnit : MonoBehaviour {

        /*********************************** HIDDEN ATTRIBUTES **************************************
        * This section holds private/protected attributes, which are NOT visible within the editor.*
        * Use this section for attributes that aren't meant to be accessible from other classes.   *
        ********************************************************************************************/

        /// <summary>
        /// Toggle the move of a warbot.
        /// </summary>
        private bool toggle;

        /// <summary>
        /// Check if the unit is any unit is selected.
        /// </summary>
        private bool selected;

        /// <summary>
        /// The Unit that will be moved.
        /// </summary>
        private GameObject WarUnit;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        // Use this for initialization
        void Start() {
            toggle = false;
            selected = false;
        }

        // Update is called once per frame
        void Update() {
            if (toggle) {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit)) {
                        if (!selected) {
                            if (hit.transform.tag == "WarBot") {
                                WarUnit = hit.transform.gameObject;
                                selected = true;
                            }
                        } else {
                            if (WarUnit != null && hit.transform.name == "Map") {
                                WarUnit.transform.position = hit.point;
                                selected = false;
                            }
                        }

                    }
                }
            }
        }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Toggle the move option.
        /// </summary>
        public void ToggleMove() { toggle = !toggle; }

    }

}
