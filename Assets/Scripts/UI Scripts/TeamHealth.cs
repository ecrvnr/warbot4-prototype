using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.WarBots;
using WarBotEngine.UI;

namespace WarBotEngine.UI {
    /// <summary>
    /// Show the health of each teams.
    /// The team health is represented by the health of every warbase in a team.
    /// </summary>
    public class TeamHealth : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/
        /// <summary>
        /// The health bars of each team.
        /// </summary>
        public GameObject blue_healthbar,
                          red_healthbar;

        public static int nb_blue_warbases,
                          nb_red_warbases;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Max health of a warbase
        /// </summary>
        private float max_health;

        /// <summary>
        /// The width of the image inside the healthbar.
        /// </summary>
        private float width;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        // Use this for initialization
        void Start() {
            if (!Managers.PropertiesManager.Actual.PropertiesAssigned)
                Managers.PropertiesManager.Actual.AssignProperties();
            max_health = WarBase.MaxHealth; // * Managers.Initializer.Actual.getElement("WarBase");
            width = blue_healthbar.transform.Find("Health").GetComponent<RectTransform>().rect.width;

            GameObject.Find("Red_Team_Name").GetComponent<Text>().text = Managers.Initializer.Actual.red_team;
            GameObject.Find("Blue_Team_Name").GetComponent<Text>().text = Managers.Initializer.Actual.blue_team;

            nb_blue_warbases = Managers.Initializer.Actual.getElement("WarBase");
            nb_red_warbases = Managers.Initializer.Actual.getElement("WarBase");
        }

        // Update is called once per frame
        void LateUpdate() {
            float total_red_health = 0, total_blue_health = 0;

            Object[] count_WarBase = FindObjectsOfType(typeof(WarBase));

            foreach (WarBase warBase in count_WarBase) {
                if (warBase.GetComponent<WarBotController>().TeamManager.color == Color.red) {
                    total_red_health += warBase.HealthController.CurrentHealth; 
                } else {
                    total_blue_health += warBase.HealthController.CurrentHealth;
                }
            }

            red_healthbar.transform.Find("Health").GetComponent<RectTransform>().transform.localScale =
                    new Vector3(Mathf.Clamp(total_red_health / (max_health * nb_red_warbases), 0, width), red_healthbar.transform.localScale.y, red_healthbar.transform.localScale.z);

            blue_healthbar.transform.Find("Health").GetComponent<RectTransform>().transform.localScale =
                    new Vector3(Mathf.Clamp(total_blue_health / (max_health * nb_blue_warbases), 0, width), blue_healthbar.transform.localScale.y, blue_healthbar.transform.localScale.z);

        }
    }

}
