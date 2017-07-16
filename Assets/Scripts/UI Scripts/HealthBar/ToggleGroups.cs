using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.UI {

    /// <summary>
    /// This class is used when the Toggle "Show Health Bar" is checked/unchecked.
    /// It will instatiate a health bar for every units prefab when the toggle is checked and destroy it when unchecked.
    /// </summary>
    public class ToggleGroups : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/

        /// <summary>
        /// The ShowGroup prefab.
        /// </summary>
        public GameObject ShowGroupPrefab;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
        *********************************************************************************************/

        /// <summary>
        /// List of GameObjects already containing a ShowGroup prefab.
        /// </summary>
        private List<GameObject> alreadyHaveGroups;

        /// <summary>
        /// Boolean that toggle healthbars.
        /// </summary>
        private bool toggle_groups;

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Toggle the ShowGroup.
        /// </summary>
        /// <param name="activated">Boolean that determine if the health bar is activated or not.</param>
        public void Toggle(bool activated) {
            toggle_groups = activated;
        }

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        public void Start() {
            alreadyHaveGroups = new List<GameObject>();
            toggle_groups = false;
        }

        public void Update() {
            if (toggle_groups) {
                GameObject[] warbots = GameObject.FindGameObjectsWithTag("WarBot");
                if (warbots.Length != 0) {
                    foreach (GameObject warbot in warbots) {
                        if (!alreadyHaveGroups.Contains(warbot)) {
                            alreadyHaveGroups.Add(warbot);
                            Vector3 position = warbot.transform.position;
                            GameObject group =
                                    Instantiate(ShowGroupPrefab,
                                                new Vector3(position.x, position.y + 20, position.z - 25),
                                                Quaternion.identity);
                            group.transform.Find("Background").transform.Rotate(0f,180f,180f);
                            group.GetComponent<LineUpCanvasWithCamera>().mainCamera = Camera.main;
                            group.transform.SetParent(warbot.transform);
                        }
                    }
                }
            }
            else {
                alreadyHaveGroups.Clear();
                GameObject[] groups = GameObject.FindGameObjectsWithTag("ShowGroups");
                foreach (GameObject group in groups) {
                    Destroy(group);
                }
            }
        }

    }
}