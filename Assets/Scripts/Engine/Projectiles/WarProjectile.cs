using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.WarBots;
using WarBotEngine.Managers;

namespace WarBotEngine.Projectiles {
    /// <summary>
    /// This class represents general WarProjectiles and won't be attached to any entity.
    /// </summary>
    public class WarProjectile : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /// <summary>
        /// The speed of the projectile.
        /// </summary>
        public float move_speed;

        /// <summary>
        /// The max range that the projectile can travel before getting destroyed.
        /// </summary>
        public float lifetime;

        /// <summary>
        /// The damage caused to targets.
        /// </summary>
        public float damage_caused;

        /// <summary>
        /// The explosion particles.
        /// </summary>
        public ParticleSystem explosion_particles;

        /// <summary>
        /// Start this instance.
        /// </summary>
        public Light explosion_spotlight;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Keep tabs on the parent to avoid colliding with it.
        /// </summary>
        private GameObject parent_entity;

        private TeamManager team;

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /***************************************** DELEGATES AND EVENTS ***************************************************
		 * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
		 * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
		 ******************************************************************************************************************/

        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        protected void Awake() {
            this.Init();
        }

        // Use this for initialization
        protected virtual void Start() {
            StartCoroutine(LifetimeCounter());
        }

        // Update is called once per frame
        protected virtual void FixedUpdate() {
            this.transform.rotation = Quaternion.LookRotation(this.GetComponent<Rigidbody>().velocity.normalized);
        }

        /// <summary>
        /// Raises the trigger enter event.
        /// </summary>
        /// <param name="collision">Collision.</param>
        protected virtual void OnTriggerEnter(Collider collision) {

            if (collision.gameObject.CompareTag("WarBot") && collision.gameObject.GetComponent<WarBotController>().TeamManager.GetComponent<TeamManager>() != this.team) {
                collision.gameObject.GetComponent<HealthController>().ReduceHealth(this.damage_caused);
                this.Explode();
            }
            if (((this.parent_entity == null) || (this.parent_entity != null && collision.gameObject != this.parent_entity)) && !(collision.gameObject.CompareTag("WarProjectile")) && !(collision.gameObject.CompareTag("UnitSelector"))) {
                this.Explode();
            }
        }

        /******************************************* OTHER FUNCTIONS ***************************************************
		* This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		* In principle, every function in this section should only be called from within primitives.                   *
		* **************************************************************************************************************/

        /// <summary>
        /// Initializes the variables.
        /// </summary>
        protected virtual void Init() {
            this.team = this.transform.parent.GetComponent<WarBotController>().TeamManager.GetComponent<TeamManager>();
            this.parent_entity = this.transform.parent.gameObject;
            this.transform.parent = null;
            this.GetComponent<Rigidbody>().velocity = this.transform.forward * move_speed;
        }

        /// <summary>
        /// This coroutine ensures that the projectile won't overstay its welcome.
        /// </summary>
        /// <returns>The counter.</returns>
        protected IEnumerator LifetimeCounter() {

            while (this.lifetime > 0.0f) {
                this.lifetime -= Time.deltaTime;
                yield return null;
            }
            this.Explode();
            yield break;
        }

        /// <summary>
        /// Destroys the projectile with a neat effect.
        /// </summary>
        protected virtual void Explode() {
            SoundManager.Actual.PlayExplosion(this.gameObject);
            Instantiate(this.explosion_particles , this.transform.position - this.transform.forward * 2 , this.transform.rotation , null);
            Instantiate(this.explosion_spotlight , this.transform.position , this.transform.rotation , null);
            Destroy(this.gameObject);
        }
    }
}