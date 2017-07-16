using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.Managers;
using WarBotEngine.WarBots;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used when the user want informations about the group of each warbots.
    /// </summary>
    public class ShowGroups : MonoBehaviour {

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// The TeamManager of the current unit.
        /// </summary>
        private TeamManager tm;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        // Use this for initialization
        void Start() {
            //First parent : WarBot. Then parent of this parent : TeamManager.
            tm = (TeamManager) this.transform.parent.parent.gameObject.GetComponent(typeof(TeamManager));
        }

        // Update is called once per frame
        void Update() {

            string groups = "";

            if (tm.color == Color.red) {
                foreach (GroupController.Group group in GroupController.Team1Groups) {
                    groups += group.GetRole(this.transform.parent.gameObject) + "\n";
                }
            } else {
                foreach (GroupController.Group group in GroupController.Team2Groups) {
                    groups += group.GetRole(this.transform.parent.gameObject) + "\n";
                }
            }

            if (groups == "") groups = "Aucun groupe assigné";

            this.transform.Find("Background/Text").GetComponent<Text>().text = groups;

            if (!this.transform.parent.name.Contains("Warbase"))
                this.transform.position = new Vector3(this.transform.parent.position.x + 0, this.transform.parent.position.y + 12, this.transform.parent.position.z - 12);
            else
                this.transform.position = new Vector3(this.transform.parent.position.x + 0, this.transform.parent.position.y + 30, this.transform.parent.position.z - 30);

        }
    }
}
