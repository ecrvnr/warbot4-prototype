using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.WarBots;

namespace WarBotEngine.UI {

    public class ShowRessources : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
       * This section holds public attributes, which are visible and editable within the editor.*
       * For attributes accessible publicly but not visible in the editor, use properties.      *
       ******************************************************************************************/
        /// <summary>
        /// The Ressource GameObject. It's the Image that represent the ressources of a Warbot entity.
        /// </summary>
        public GameObject RessourceBar;

        /*********************************** HIDDEN ATTRIBUTES **************************************
       * This section holds private/protected attributes, which are NOT visible within the editor.*
       * Use this section for attributes that aren't meant to be accessible from other classes.   *
       ********************************************************************************************/

        /// <summary>
        /// The InventoryController of the current Warbot entity.
        /// </summary>
        private InventoryController ic;

        /// <summary>
        /// The size of the RessourceBar.
        /// </summary>
        private float width;

        /******************************************* UNITY FUNCTIONS ********************************************
        * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
        * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            ic =  (InventoryController) this.transform.parent.gameObject.GetComponent(typeof(InventoryController));
            width = this.transform.Find("Ressources").GetComponent<RectTransform>().rect.width;
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            RessourceBar.transform.localScale = new Vector3(Mathf.Clamp(ic.CurrentInventory / (float) ic.max_inventory , 0 , width) , RessourceBar.transform.localScale.y , RessourceBar.transform.localScale.z);

            if (!this.transform.parent.name.Contains("Warbase"))
                this.transform.position = new Vector3(this.transform.parent.position.x + 0, this.transform.parent.position.y + 5, this.transform.parent.position.z - 12);
            else
                this.transform.position = new Vector3(this.transform.parent.position.x + 0, this.transform.parent.position.y + 25, this.transform.parent.position.z - 25);
        }

    }
}