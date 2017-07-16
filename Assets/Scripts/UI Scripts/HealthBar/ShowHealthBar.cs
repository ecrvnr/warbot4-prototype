using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.WarBots;

namespace WarBotEngine.UI {

    public class ShowHealthBar : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
       * This section holds public attributes, which are visible and editable within the editor.*
       * For attributes accessible publicly but not visible in the editor, use properties.      *
       ******************************************************************************************/
        /// <summary>
        /// The Health GameObject. It's the Image that represent the health of a Warbot entity.
        /// </summary>
        public GameObject healthBar;

        /*********************************** HIDDEN ATTRIBUTES **************************************
       * This section holds private/protected attributes, which are NOT visible within the editor.*
       * Use this section for attributes that aren't meant to be accessible from other classes.   *
       ********************************************************************************************/

        /// <summary>
        /// The HealtController of the current Warbot entity.
        /// </summary>
        private HealthController hl;

        /// <summary>
        /// The size of the Health.
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
            hl =  (HealthController) this.transform.parent.gameObject.GetComponent(typeof(HealthController));
            width = this.transform.Find("Health").GetComponent<RectTransform>().rect.width;
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            healthBar.transform.localScale = new Vector3(Mathf.Clamp(hl.CurrentHealth / hl.max_health , 0 , width) , healthBar.transform.localScale.y , healthBar.transform.localScale.z);

            if(!this.transform.parent.name.Contains("Warbase"))
                this.transform.position = new Vector3(this.transform.parent.position.x + 0, this.transform.parent.position.y + 5, this.transform.parent.position.z - 8);
            else
                this.transform.position = new Vector3(this.transform.parent.position.x + 0, this.transform.parent.position.y + 25, this.transform.parent.position.z - 20);

        }

    }
}