using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.UI {

    /// <summary>
    /// This class is used when the Toggle "Show Health Bar" is checked/unchecked.
    /// It will instatiate a health bar for every units prefab when the toggle is checked and destroy it when unchecked.
    /// </summary>
    public class ToggleRessources : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/

        /// <summary>
        /// The RessourceBar prefab.
        /// </summary>
        public GameObject RessourcesBarPrefab;


        public float poxX, posY, posZ;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
        *********************************************************************************************/

        /// <summary>
        /// List of GameObjects already containing a health bar.
        /// </summary>
        private List<GameObject> alreadyHaveRessources;

        /// <summary>
        /// Boolean that toggle ressourcess.
        /// </summary>
        private bool toggle_ressources;

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Toggle the health bar.
        /// </summary>
        /// <param name="activated">Boolean that determine if the health bar is activated or not.</param>
        public void Toggle(bool activated) {
            toggle_ressources = activated;
        }

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/
        public void Start() {
            alreadyHaveRessources = new List<GameObject>();
            toggle_ressources = false;
        }

        public void Update() {
            if (toggle_ressources) {
                GameObject[] warbots = GameObject.FindGameObjectsWithTag("WarBot");
                if (warbots.Length != 0) {
                    foreach (GameObject warbot in warbots) {
                        if (!alreadyHaveRessources.Contains(warbot)) {
                            alreadyHaveRessources.Add(warbot);
                            Vector3 position = warbot.transform.position;
                            GameObject ressources =
                                    Instantiate(RessourcesBarPrefab,
                                                new Vector3(position.x + poxX, position.y + posY, position.z + posZ),
                                                Quaternion.identity);
                            ressources.GetComponent<LineUpCanvasWithCamera>().mainCamera = Camera.main;
                            ressources.transform.SetParent(warbot.transform);
                        }
                    }
                }
            }
            else {
                alreadyHaveRessources.Clear();
                GameObject[] ressourcess = GameObject.FindGameObjectsWithTag("RessourcesBar");
                foreach (GameObject ressources in ressourcess) {
                    Destroy(ressources);
                }
            }
        }

    }
}