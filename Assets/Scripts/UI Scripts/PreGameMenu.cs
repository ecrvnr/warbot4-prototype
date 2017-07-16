using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.Editeur;

namespace WarBotEngine.UI {

    /// <summary>
    /// Used in the Pre-game menu to initialize it.
    /// </summary>
    public class PreGameMenu : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/

        /// <summary>
        /// Dropdowns for the selection of red and blue team.
        /// </summary>
        public Dropdown red_team,
                        blue_team;

        /// <summary>
        /// All the sliders for every warbots.
        /// They'll indicate how much of each unit will be instanciated at the very beggining of the game.
        /// </summary>
        public Slider slider_WarBase,
                      slider_WarExplorer,
                      slider_WarHeavy,
                      slider_WarEnginner,
                      slider_WarTurret,
                      slider_Ressources;

        /// <summary>
        /// The GameObject that will hold all the variables for the next scene.
        /// </summary>
        public GameObject initializer;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Contains the element that will list up all the teams neams
        /// </summary>
        private XMLInterpreter xmlInterpreter;


        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
        **********************************************************************************************************/

        // Use this for initialization
        void Start() {
            DontDestroyOnLoad(initializer); 

            xmlInterpreter = new XMLInterpreter();

            string path = Constants.teamsDirectory;
            List<string> teams = xmlInterpreter.allTeamsInXmlFiles(path);

            red_team.AddOptions(teams);
            blue_team.AddOptions(teams);

            initializer.GetComponent<Managers.Initializer>().red_team = teams[0];
            initializer.GetComponent<Managers.Initializer>().blue_team = teams[0];
        }

        // Update is called once per frame
        void Update() {
            slider_WarBase.transform.Find("N_Warbots").GetComponent<Text>().text = slider_WarBase.value.ToString();
            slider_WarExplorer.transform.Find("N_Warbots").GetComponent<Text>().text = slider_WarExplorer.value.ToString();
            slider_WarHeavy.transform.Find("N_Warbots").GetComponent<Text>().text = slider_WarHeavy.value.ToString();
            slider_WarEnginner.transform.Find("N_Warbots").GetComponent<Text>().text = slider_WarEnginner.value.ToString();
            slider_WarTurret.transform.Find("N_Warbots").GetComponent<Text>().text = slider_WarTurret.value.ToString();
            slider_Ressources.transform.Find("N_Ressources").GetComponent<Text>().text = slider_Ressources.value.ToString();
        }

        /******************************************** GUI FUNCTIONS ********************************************
         * This section holds all functions for the user interface.                                             *
         ********************************************************************************************************/

        /// <summary>
        /// Red Team dropdown
        /// </summary>
        public void dropDownRedTeam(int index) {
            initializer.GetComponent<Managers.Initializer>().red_team = xmlInterpreter.allTeamsInXmlFiles(Constants.teamsDirectory)[index];
        }

        /// <summary>
        /// Blue Team dropdown
        /// </summary>
        public void dropDownBlueTeam(int index) {
            initializer.GetComponent<Managers.Initializer>().blue_team = xmlInterpreter.allTeamsInXmlFiles(Constants.teamsDirectory)[index];
        }

        /// <summary>
        /// WarBase slider.
        /// </summary>
        public void ChangeValueWarBase(float newValue) {
            initializer.GetComponent<Managers.Initializer>().setElement("WarBase", (int) newValue);
        }

        /// <summary>
        /// WarExplorer slider.
        /// </summary>
        public void ChangeValueWarExplorer(float newValue) {
            initializer.GetComponent<Managers.Initializer>().setElement("WarExplorer", (int) newValue);
        }

        /// <summary>
        /// WarHeavy slider.
        /// </summary>
        public void ChangeValueWarHeavy(float newValue) {
            initializer.GetComponent<Managers.Initializer>().setElement("WarHeavy", (int) newValue);
        }

        /// <summary>
        /// WarEngineer slider.
        /// </summary>
        public void ChangeValueWarEngineer(float newValue) {
            initializer.GetComponent<Managers.Initializer>().setElement("WarEngineer", (int) newValue);
        }

        /// <summary>
        /// WarTurret slider.
        /// </summary>
        public void ChangeValueWarTurret(float newValue) {
            initializer.GetComponent<Managers.Initializer>().setElement("WarTurret", (int) newValue);
        }

        /// <summary>
        /// Ressouces slider.
        /// </summary>
        public void ChangeValueRessources(float newValue) {
            initializer.GetComponent<Managers.Initializer>().ressources_time = (int) newValue;
        }
    }


}

