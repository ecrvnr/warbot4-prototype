using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarBotEngine.Managers;
using WarBotEngine.Items;

namespace WarBotEngine.WarBots {
    /// <summary>
    /// A simple script to detect other unit's through triggers.
    /// </summary>
    public class DetectionController : MonoBehaviour {
        /*********************************** EDITOR ATTRIBUTES ************************************
	     * This section holds public attributes, which are visible and editable within the editor.*
	     * For attributes accessible publicly but not visible in the editor, use properties.      *
	     ******************************************************************************************/

        /// <summary>
        /// The unit's real perception range.
        /// </summary>
        public float perception_radius;

        /*********************************** HIDDEN ATTRIBUTES **************************************
	     * This section holds private/protected attributes, which are NOT visible within the editor.*
	     * Use this section for attributes that aren't meant to be accessible from other classes.   *
	     ********************************************************************************************/

        /// <summary>
        /// The enemy units currently in line of sight.
        /// </summary>
        protected List<GameObject> visible_enemies;

        /// <summary>
        /// The alliees units currently in line of sight
        /// </summary>
        protected List<GameObject> visible_alliees;

        /// <summary>
        /// The detected items. Those remain even after leaving perception range because they don't move. 
        /// They get removed from the list upon disappearing (getting picked up/destroyed).
        /// </summary>
        protected List<GameObject> detected_items;

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /// <summary>
        /// Gets or sets the currently visible enemies.
        /// </summary>
        /// <value>The detected enemies.</value>
        public List<GameObject> VisibleEnemies { get { return this.visible_enemies; } set { this.visible_enemies = value; } }

        /// <summary>
        /// Gets or sets the currently visible alliees.
        /// </summary>
        /// <value>The detected enemies.</value>
        public List<GameObject> VisibleAlliees { get { return this.visible_alliees; } set { this.visible_alliees = value; } }

        /// <summary>
        /// Gets or sets the detected items.
        /// </summary>
        /// <value>The detected items.</value>
        public List<GameObject> DetectedItems { get { return this.detected_items; } set { this.detected_items = value; } }


        /***************************************** DELEGATES AND EVENTS ***************************************************
         * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
	     * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
	     ******************************************************************************************************************/

        /// <summary>
        /// Delegate that will handle detection events.
        /// </summary>
        public delegate void DetectionEventHandler(GameObject detection);

        /// <summary>
        /// Event based on the previous delegate.
        /// </summary>
        public event DetectionEventHandler EnemyLostEvent;

        /// <summary>
        /// Event based on the previous delegate.
        /// </summary>
        public event DetectionEventHandler EnemyDetectedEvent;

        /// <summary>
        /// Event based on the previous delegate.
        /// </summary>
        public event DetectionEventHandler AllieeLostEvent;

        /// <summary>
        /// Event based on the previous delegate.
        /// </summary>
        public event DetectionEventHandler AllieeDetectedEvent;

        /// <summary>
		/// Event based on the previous delegate.
		/// </summary>
		public event DetectionEventHandler ItemDetectedEvent;

        /// <summary>
        /// Event based on the previous delegate.
        /// </summary>
        public event DetectionEventHandler ItemLostEvent;

        /******************************************* UNITY FUNCTIONS ********************************************
	     * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
	     * In principle, every function in this section will run once per frame, except Start().                *
	     ********************************************************************************************************/

        /// <summary>
        /// On awaking this instance
        /// </summary>
        private void Awake() {
            //Init();
        }

        /// <summary>
        /// On starting this instance.
        /// </summary>
        void Start() {

        }

        /// <summary>
        /// Update this instance. Detect all relevant entities around us and update our internal lists accordingly.
        /// </summary>
        void Update() {
            
        }

        /******************************************* OTHER FUNCTIONS ***************************************************
	     * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
	     * In principle, every function in this section should only be called from within primitives.                   *
	     ****************************************************************************************************************/

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init() {
            this.visible_enemies = new List<GameObject>();
            this.visible_alliees = new List<GameObject>();
            this.detected_items = new List<GameObject>();
        }

        /// <summary>
        /// Detects all units and items within a specified radius around this unit.
        /// </summary>
        public void GetPercepts() {
            List<GameObject> items_in_range = new List<GameObject>();
            List<GameObject> enemies_in_range = new List<GameObject>();
            List<GameObject> alliees_in_range = new List<GameObject>();

            foreach (Collider c in Physics.OverlapSphere(this.transform.position , this.perception_radius)) {
                GameObject entity = c.gameObject;
                if (entity.CompareTag("WarBot")) {
                    if (entity.GetComponent<WarBotController>().TeamManager != this.GetComponent<WarBotController>().TeamManager)
                    {
                        if (!(enemies_in_range.Contains(entity))) {
                            enemies_in_range.Add(entity);
                        }
                    }
                    else
                    {
                        if (!(alliees_in_range.Contains(entity)))
                        {
                            alliees_in_range.Add(entity);
                        }
                    }
                } else if (entity.CompareTag("WarItem")) {
                    if (!(items_in_range.Contains(entity))) {
                        items_in_range.Add(entity);
                    }
                }
            }

            var new_enemy_detections = from detected in enemies_in_range
                                       where !(this.visible_enemies.Contains(detected))
                                       select detected;
            foreach (GameObject detected in new_enemy_detections) {
                OnEnemyDetected(detected);
            }

            var new_alliee_detections = from detected in alliees_in_range
                                       where !(this.visible_alliees.Contains(detected))
                                       select detected;
            foreach (GameObject detected in new_alliee_detections)
            {
                OnAllieeDetected(detected);
            }

            var enemies_lost = from lost in this.visible_enemies
                               where !(enemies_in_range.Contains(lost))
                               select lost;
            foreach (GameObject lost in enemies_lost.ToList()) {
                OnEnemyLost(lost);
            }

            var alliees_lost = from lost in this.visible_alliees
                               where !(alliees_in_range.Contains(lost))
                               select lost;
            foreach (GameObject lost in alliees_lost.ToList())
            {
                OnAllieeLost(lost);
            }

            var new_item_detections = from detected in items_in_range
                                      where !(this.detected_items.Contains(detected))
                                      select detected;
            foreach (GameObject detected in new_item_detections) {
                OnItemDetected(detected);
            }
        }

        /// <summary>
        /// Triggered whenever we detect an enemy.
        /// </summary>
        /// <param name="enemy"></param>
        public void OnEnemyDetected(GameObject enemy) {
            enemy.GetComponent<HealthController>().DeathEvent += this.OnEnemyLost;
            if (!(this.visible_enemies.Contains(enemy))) {
                this.visible_enemies.Add(enemy);
            }
            if (this.EnemyDetectedEvent != null) {
                this.EnemyDetectedEvent(enemy);
            }
        }

        /// <summary>
        /// Triggered when we lose an enemy.
        /// </summary>
        /// <param name="enemy"></param>
        public void OnEnemyLost(GameObject enemy) {
            if (this.visible_enemies.Contains(enemy)) {
                this.visible_enemies.Remove(enemy);
            }
            if (this.EnemyLostEvent != null) {
                this.EnemyLostEvent(enemy);
            }
        }

        /// <summary>
        /// Triggered whenever we detect an alliee.
        /// </summary>
        /// <param name="alliee"></param>
        public void OnAllieeDetected(GameObject alliee)
        {
            alliee.GetComponent<HealthController>().DeathEvent += this.OnAllieeLost;
            if (!(this.visible_alliees.Contains(alliee)))
            {
                this.visible_alliees.Add(alliee);
            }
            if (this.AllieeDetectedEvent != null)
            {
                this.AllieeDetectedEvent(alliee);
            }
        }

        /// <summary>
        /// Triggered when we lose an alliee.
        /// </summary>
        /// <param name="alliee"></param>
        public void OnAllieeLost(GameObject alliee)
        {
            if (this.visible_alliees.Contains(alliee))
            {
                this.visible_alliees.Remove(alliee);
            }
            if (this.AllieeLostEvent != null)
            {
                this.AllieeLostEvent(alliee);
            }
        }

        /// <summary>
        /// Triggered when we detect an item.
        /// </summary>
        /// <param name="item"></param>
        public void OnItemDetected(GameObject item) {
            item.GetComponent<WarResource>().PickedUpEvent += this.OnItemLost;
            if (!(this.detected_items.Contains(item))) {
                this.detected_items.Add(item);
            }
            if (this.ItemDetectedEvent != null) {
                this.ItemDetectedEvent(item);
            }
        }

        /// <summary>
        /// Triggered when we lose an item.
        /// </summary>
        /// <param name="item"></param>
        public void OnItemLost(GameObject item) {
            if (this.detected_items.Contains(item)) {
                this.detected_items.Remove(item);
            }
            if (this.ItemLostEvent != null) {
                this.ItemLostEvent(item);
            }
        }
    }
}