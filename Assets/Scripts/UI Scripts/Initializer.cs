using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.Managers {
    /// <summary>
    /// This class is used when we initialize a new game.
    /// </summary>
    public class Initializer : MonoBehaviour {
        
        /// <summary>
        /// Actual initializer
        /// </summary>
        private static Initializer actual = null;

        /// <summary>
        /// Hold the number of warbots that will be instantiated at the start of each games.
        /// </summary>
        private Dictionary<string, int> dictionnary;

        /// <summary>
        /// Hold the time between the spawn of each ressources.
        /// </summary>
        public int ressources_time;

        /// <summary>
        /// Hold the name of the teams.
        /// </summary>
        public string red_team,
                      blue_team;

        /// <summary>
        /// Actual initializer
        /// </summary>
        public static Initializer Actual { get { return actual; } }


        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
        **********************************************************************************************************/

        void Start() {
            actual = this;

            dictionnary = new Dictionary<string, int>();
            dictionnary.Add("WarBase",1);
            dictionnary.Add("WarExplorer",0);
            dictionnary.Add("WarHeavy",0);
            dictionnary.Add("WarEngineer",0);
            dictionnary.Add("WarTurret",0);

            ressources_time = (int)GameObject.Find("Slider_Ressources").GetComponent<Slider>().value;

        }

        /// <summary>
        /// Set a value for the key.
        /// </summary>
        /// <param name="key">The key of the dictionnary.</param>
        /// <param name="value">The new value.</param>
        /// <remarks> Possible keys : "WarBase", "WarExplorer", "WarHeavy", "WarEngineer", "WarTurret" </remarks>
        public void setElement(string key, int value) {
            if (dictionnary.ContainsKey(key)) {
                dictionnary[key] = value;
            } else {
                Debug.LogError("Cette valeur de clé n'existe pas.");
            }
        }

        /// <summary>
        /// Get the value from a specific key of the dictionnary.
        /// </summary>
        /// <param name="key">The key wanted.</param>
        /// <returns>The value associated with the key.</returns>
        /// <remarks> Possible keys : "WarBase", "WarExplorer", "WarHeavy", "WarEngineer", "WarTurret" </remarks>
        public int getElement(string key) {
            if (dictionnary.ContainsKey(key)) {
                return dictionnary[key];
            } else {
                Debug.LogError("Cette valeur de clé n'existe pas.");
                return -1;
            }
        }



    }

}