using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.WarBots;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used to show how much units of each team exists in the current game.
    /// </summary>
    public class StatistiquesMod : MonoBehaviour {
        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/
        ///<summary>
        /// Hold the reference to the Debug Panel, will be used to enable/disable it.
        /// </summary>
        public GameObject Stat_Panel;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/
         /// <summary>
         /// Toggle the stats panel.
         /// </summary>
        private bool statPaneltoggled;

        /// <summary>
        /// All counts about every blue team warbots are displayed here.
        /// </summary>
        private Text blue_information;


        /// <summary>
        /// All counts about every red team warbots are displayed here.
        /// </summary>
        private Text red_information;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        // Use this for initialization
        void Start() {
            statPaneltoggled = false;
            Stat_Panel.SetActive(false);

            blue_information = Stat_Panel.transform.Find("Informations_blue").gameObject.GetComponent<Text>();
            red_information = Stat_Panel.transform.Find("Informations_red").gameObject.GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            if(statPaneltoggled) {
            Object[] count_WarBase = FindObjectsOfType(typeof(WarBase));
            Object[] count_WarExplorer = FindObjectsOfType(typeof(WarExplorer));
            Object[] count_WarHeavy = FindObjectsOfType(typeof(WarHeavy));
            Object[] count_WarEnginner = FindObjectsOfType(typeof(WarEngineer));
            Object[] count_WarTurret = FindObjectsOfType(typeof(WarTurret));

            int r_warbase = 0, r_warexplorer = 0, r_warheavy = 0, r_warenginner = 0, r_warturret = 0,
                b_warbase = 0, b_warexplorer = 0, b_warheavy = 0, b_warenginner = 0, b_warturret = 0;
            
            //Count of every warbot by color.
            foreach (WarBase warBase in count_WarBase) {
                if (warBase.GetComponent<WarBotController>().TeamManager.color == Color.red) r_warbase++;
                else b_warbase++;
            }

            foreach (WarExplorer war in count_WarExplorer) {
                if (war.GetComponent<WarBotController>().TeamManager.color == Color.red) r_warexplorer++;
                else b_warexplorer++;
            }

            foreach (WarHeavy war in count_WarHeavy) {
                if (war.GetComponent<WarBotController>().TeamManager.color == Color.red) r_warheavy++;
                else b_warheavy++;
            }

            foreach (WarEngineer war in count_WarEnginner) {
                if (war.GetComponent<WarBotController>().TeamManager.color == Color.red) r_warenginner++;
                else b_warenginner++;
            }

            foreach (WarTurret war in count_WarTurret) {
                if (war.GetComponent<WarBotController>().TeamManager.color == Color.red) r_warturret++;
                else b_warturret++;
            }
            
            //Display
            blue_information.text = r_warbase + "\n\n" +
                                    r_warexplorer + "\n\n" +
                                    r_warenginner + "\n\n" +
                                    r_warheavy + "\n\n" +
                                    r_warturret;

            red_information.text = b_warbase + "\n\n" +
                                   b_warexplorer + "\n\n" +
                                   b_warheavy + "\n\n" +
                                   b_warenginner + "\n\n" +
                                   b_warturret;
                
            }
        }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/

        /// <summary>
        /// Toggle the statistique panel.
        /// </summary>
        public void TogglingStatMod() {
            statPaneltoggled = !statPaneltoggled;

            Stat_Panel.SetActive(statPaneltoggled);
        }

    }

}