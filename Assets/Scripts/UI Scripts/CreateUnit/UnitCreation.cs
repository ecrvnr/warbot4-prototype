using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WarBotEngine.Managers;
using WarBotEngine.WarBots;

namespace WarBotEngine.UI {

    /// <summary>
    /// This class is used to instantiate a Warbot manually.
    /// </summary>
    public class UnitCreation : MonoBehaviour {

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// The warBot prefab that will be created.
        /// </summary>
        private GameObject warBotPrefab;

        /// <summary>
        /// Toggle for the creation.
        /// </summary>
        public static bool toggle_creation;

        /// <summary>
        /// The color of the team we want the warbot to join.
        /// </summary>
        private Color color;

        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            toggle_creation = false;
            color = Color.black;
            SetButtonActivable(false);
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            if (toggle_creation && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {

                    Debug.Log(hit.collider.name);
                    if (hit.collider.name == "Map") {
                        GameObject warbot = Instantiate(warBotPrefab,
                                                        new Vector3(hit.point.x, hit.point.y, hit.point.z),
                                                        warBotPrefab.transform.rotation);

                        GameObject team_red = GameObject.Find("Team Manager 1"),
                                   team_blue = GameObject.Find("Team Manager 2");

                        if (color == Color.red) {
                            //team_red.GetComponent<TeamManager>().AddWarBot(warbot);
                            team_red.GetComponent<TeamManager>()
                                    .InitUnit(warbot, warbot.GetComponent<WarBotController>().GetUnitType());
                            warbot.transform.SetParent(team_red.transform);

                            if (warBotPrefab.name == "Warbase")
                                TeamHealth.nb_red_warbases++;

                        } else if (color == Color.blue) {
                            //team_blue.GetComponent<TeamManager>().AddWarBot(warbot);
                            team_blue.GetComponent<TeamManager>()
                                     .InitUnit(warbot, warbot.GetComponent<WarBotController>().GetUnitType());
                            warbot.transform.SetParent(team_blue.transform);

                            if (warBotPrefab.name == "Warbase")
                                TeamHealth.nb_blue_warbases++;
                        }

                    }
                }
            }
        }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/

        /// <summary>
        /// Get the color for the future warbot.
        /// </summary>
        /// <param name="colorName">The name of the color for the desired team.</param>
        public void setColor(string colorName) {
            if(colorName == "Rouge" || colorName == "Red")
                color = Color.red;
            else if (colorName == "Bleue" || colorName == "Blue")
                color = Color.blue;

            SetButtonActivable(true);
        }


        /// <summary>
        /// Toggle the creation of unit.
        /// </summary>
        /// <param name="prefab">Boolean</param>
        public void ToggleCreation(GameObject prefab) {
            toggle_creation = true;
            warBotPrefab = prefab;
        }

        /// <summary>
        /// Set the buttons of the panel to be interactable or not.
        /// </summary>
        /// <param name="toggle">Boolean</param>
        public static void SetButtonActivable(bool toggle) {
            GameObject.Find("Warbase_Button").GetComponent<Button>().interactable = toggle;
            GameObject.Find("WarExplorer_Button").GetComponent<Button>().interactable = toggle;
            GameObject.Find("WarHeavy_Button").GetComponent<Button>().interactable = toggle;
            GameObject.Find("WarEngineer_Button").GetComponent<Button>().interactable = toggle;
            GameObject.Find("WarTurret_Button").GetComponent<Button>().interactable = toggle;
        }
    }
}