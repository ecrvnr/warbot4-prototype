using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WarBotEngine.Projectiles;
using System.Security.Cryptography;

namespace WarBotEngine.WarBots {

    public class AttackController : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /// <summary>
        /// The unit's reload time.
        /// </summary>
        public float reload_time;

        /// <summary>
        /// The type of projectile fired by the unit.
        /// </summary>
        public GameObject projectile;

        /// <summary>
        /// The muzzle flash.
        /// </summary>
        public Light muzzle_flash;

        /*********************************** HIDDEN ATTRIBUTES **************************************
		 * This section holds private/private attributes, which are NOT visible within the editor.*
		 * Use this section for attributes that aren't meant to be accessible from other classes.   *
		 ********************************************************************************************/

        /// <summary>
        /// Tells us if we're currently reloading or not (ready to fire).
        /// </summary>
        private float reloading;

        /// <summary>
        /// This unit's canon.
        /// </summary>
        private GameObject canon;

        /// <summary>
        /// This unit's canon.
        /// </summary>
        private GameObject warprojectile_emitter;

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /// <summary>
        /// Gets the detected items.
        /// </summary>
        /// <value>The detected items.</value>
        public List<GameObject> VisibleEnemies { get { return this.GetComponent<DetectionController>().VisibleEnemies; } }

        /***************************************** DELEGATES AND EVENTS ***************************************************
		 * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
		 * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
		 ******************************************************************************************************************/

        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        private void Start() {
            this.Init();
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        private void Update() {
            
        }

        /******************************************** PRIMITIVES **********************************************
		 * This section holds all our primitives, which are the functions that the FSM will be allowed to use.* 
		 * In principle, any unit's main loop should only call primitives.                                    *
		 ******************************************************************************************************/

        public bool Reloaded()
        {
            return (this.reloading + this.reload_time < Time.time);
        }

        public bool Shoot()
        {
            if (!this.Reloaded())
                return false;
            this.Fire();
            return true;
        }

        public void Aim(Vector3 position)
        {
            Vector3 lookPos = CalculateFireAngle(position);
            canon.transform.rotation = Quaternion.LookRotation(lookPos);
        }

        public void Aim(GameObject enemy)
        {
            this.Aim(enemy.transform.position);
        }

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Init this instance.
        /// </summary>
        private void Init() {
            this.canon = this.transform.Find("Model/Canon").gameObject as GameObject;
            this.warprojectile_emitter = this.transform.Find("Model/Canon/WarProjectile Emitter").gameObject as GameObject;
            this.reloading = 0f;
        }

        /// <summary>
        /// Calculates the vector at which we must fire to hit our target.
        /// </summary>
        /// <returns>The parabola trajectory.</returns>
        /// <param name="target_position">Target position.</param>
        public Vector3 CalculateFireAngle(Vector3 target_position) {
            Vector3 lookPos = target_position - this.warprojectile_emitter.transform.position; //the initial vector from us to our target
            float height_delta = lookPos.y; //the height difference between us and our target
            lookPos.y = 0.0f; //flatten the target vector to get the range
            float range = lookPos.magnitude; //the distance between us and our target
            float projectile_velocity = this.projectile.GetComponent<WarProjectile>().move_speed;
            float gravity_value = Physics.gravity.magnitude;
            float up_angle = Mathf.Atan((Mathf.Pow(projectile_velocity , 2.0f)
                - Mathf.Sqrt(Mathf.Pow(projectile_velocity , 4.0f) - gravity_value
                * (gravity_value * Mathf.Pow(range , 2.0f) + 2.0f * height_delta * Mathf.Pow(projectile_velocity , 2.0f)))) / (gravity_value * range)); //where the magic happens
            lookPos.y = up_angle * lookPos.magnitude; //apply x-axis rotation to the target vector
            return lookPos;
        }

        /// <summary>
        /// Shoot a rocket.
        /// </summary>
        public void Fire() {
            SoundManager.Actual.PlayFire(this.gameObject);
            Instantiate(this.projectile , this.warprojectile_emitter.transform.position , this.warprojectile_emitter.transform.rotation , this.transform);
            Instantiate(this.muzzle_flash , this.warprojectile_emitter.transform.position , this.warprojectile_emitter.transform.rotation , this.transform);
            this.reloading = Time.time;
        }
    }
}