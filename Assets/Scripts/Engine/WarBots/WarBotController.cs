using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.CodeDom;
using WarBotEngine.Managers;

namespace WarBotEngine.WarBots {

    /// <summary>
    /// This describes the base behaviour for every WarBotController.
    /// This class is the root of the hierarchy for every entity that will be programmed by the users through FSM.
    /// </summary>
    public class WarBotController : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /*********************************** HIDDEN ATTRIBUTES **************************************
		 * This section holds private/protected attributes, which are NOT visible within the editor.*
		 * Use this section for attributes that aren't meant to be accessible from other classes.   *
		 ********************************************************************************************/

        /// <summary>
        /// Our team's ID.
        /// </summary>
        public TeamManager team_manager;

        /// <summary>
        /// Tells us whether this unit is currently selected.
        /// </summary>
        protected bool selected;

        /// <summary>
        /// The circle around the unit.
        /// </summary>
        protected GameObject unit_indicator;

        /// <summary>
        /// Themouse hover indicator.
        /// </summary>
        protected GameObject unit_plate;

        /// <summary>
        /// The type of the current WarBot.
        /// </summary>
        protected BotType type;

        /*********************************************** PROPERTIES **************************************************
		   * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		   *************************************************************************************************************/

        /// <summary>
        /// Gets the health controller.
        /// </summary>
        /// <value>The health controller.</value>
        public HealthController HealthController {
            get {
                if (this.GetComponent<HealthController>() != null) {
                    return this.GetComponent<HealthController>();
                } else {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Gets the detection controller.
        /// </summary>
        /// <value>The detection controller.</value>
        public DetectionController DetectionController {
            get {
                if (this.GetComponent<DetectionController>() != null) {
                    return this.GetComponent<DetectionController>();
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Our team's ID.
        /// </summary>
        /// <value>The team I.</value>
        public TeamManager TeamManager { get { return this.team_manager; } set { this.team_manager = value; } }

        /// <summary>
        /// Tells us whether this unit is currently selected.
        /// </summary>
        public bool Selected { get { return this.selected; } set { this.selected = value; } }

        /// <summary>
        /// Getter / Setter for the BotType.
        /// </summary>
        public BotType Type { get; set; }

        /***************************************** DELEGATES AND EVENTS ***************************************************
         * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
         * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
         ******************************************************************************************************************/


        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        /// <summary>
        /// Start this instance. Will run once when the entity is instantiated.
        /// This method is virtual so that we can override it in entities inheriting from this.
        /// </summary>
        protected virtual void Start() {
            this.Init();
        }

        /// <summary>
        /// Update this instance. Will run once every frame.
        /// This method is virtual so that we can override it in entities inheriting from this.
        /// </summary>
        protected virtual void Update() {
            this.DetectionController.GetPercepts();
        }


        protected void OnMouseUpAsButton() {
            GameObject.Find("Game Manager").GetComponent<GameManager>().SelectUnit(this.gameObject);
        }

        /// <summary>
        /// When the mouse is over this unit.
        /// </summary>
        protected void OnMouseOver() {
            Color col = this.TeamManager.GetComponent<TeamManager>().color;
            col.a = 1.0f;
            this.unit_indicator.GetComponent<Renderer>().material.SetColor("_Color" , col);
        }

        /// <summary>
        /// When the mouse moves away from this unit.
        /// </summary>
        protected void OnMouseExit() {
            Color col = Color.white;
            col.a = 0.0f;
            this.unit_indicator.GetComponent<Renderer>().material.SetColor("_Color" , col);
        }

        /******************************************** PRIMITIVES **********************************************
		 * This section holds all our primitives, which are the functions that the FSM will be allowed to use.* 
		 * In principle, any unit's main loop should only call primitives.                                    *
		 ******************************************************************************************************/


        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Initializes this instance's attributes.
        /// This method is used to avoid charging the Start() method too much.
        /// </summary>
        protected virtual void Init() {
            this.unit_indicator = Instantiate(Resources.Load("Prefabs/Units/Unit Indicator") , this.transform.position + Vector3.up * 0.01f , this.transform.rotation , this.transform.Find("Model")) as GameObject;
            this.unit_indicator.transform.name = "Unit Indicator";
            this.unit_plate = this.transform.Find("Model/Unit Indicator/Unit Plate").gameObject as GameObject;
            Color col = this.TeamManager.GetComponent<TeamManager>().color;
            col.a = 0.0f;
            this.unit_indicator.GetComponent<Renderer>().material.SetColor("_Color" , col);
            col.a = 0.3f;
            this.unit_plate.GetComponent<Renderer>().material.SetColor("_Color" , col);
        }

        /// <summary>
        /// Makes this unit the selected one.
        /// </summary>
        public void Select() {
            this.selected = true;
            Color col = this.TeamManager.GetComponent<TeamManager>().color;
            col.a = 1.0f;
            this.unit_plate.GetComponent<Renderer>().material.SetColor("_Color" , col);
        }

        /// <summary>
        /// Makes this unit unselected.
        /// </summary>
        public void Unselect() {
            this.selected = false;
            Color col = this.TeamManager.GetComponent<TeamManager>().color;
            col.a = 0.3f;
            this.unit_plate.GetComponent<Renderer>().material.SetColor("_Color" , col);
        }

        /// <summary>
        /// Get the unit type
        /// </summary>
        /// <returns></returns>
        public virtual BotType GetUnitType() { return BotType.WarBase; }

    }
}
