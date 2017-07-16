using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.WarBots
{

    /// <summary>
    /// Inventory controller
    /// </summary>
    public class InventoryController : MonoBehaviour
    {


        ///////////////////////////////
        ////// STATIC ATTRIBUTES //////
        ///////////////////////////////
        

        /// <summary>
        /// Cost for heal a unit
        /// </summary>
        public static int HealCost;

        /// <summary>
        /// Health count regeneration
        /// </summary>
        public static int HealValue;

        /// <summary>
        /// Number of ressources gained by take
        /// </summary>
        public static int TakeCount;

        /// <summary>
        /// Number of ressources transfered by a give
        /// </summary>
        public static int GiveCount;

        /// <summary>
        /// Minimal distance for give ressources
        /// </summary>
        public static float GiveDistance;


        ////////////////////////
        ////// ATTRIBUTES //////
        ////////////////////////


        /// <summary>
        /// Maximum capacity of the inventory
        /// </summary>
        public int max_inventory;

        /// <summary>
        /// Current state of the inventory
        /// </summary>
        private int current_inventory;


        ///////////////////////
        ////// ACCESSORS //////
        ///////////////////////


        /// <summary>
        /// Current state of the inventory
        /// </summary>
        public int CurrentInventory { get { return this.current_inventory; } }

        /// <summary>
        /// Indicate if the inventory is empty
        /// </summary>
        public bool Empty { get { return this.current_inventory == 0; } }

        /// <summary>
        /// Indicate if the inventory is full
        /// </summary>
        public bool Full { get { return this.current_inventory == this.max_inventory; } }


        ///////////////////////////
        ////// UNITY METHODS //////
        ///////////////////////////


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        //////////////////////////////
        ////// SPECIFIC METHODS //////
        //////////////////////////////


        /// <summary>
        /// Init inventory
        /// </summary>
        public void Init()
        {
            this.current_inventory = this.max_inventory / 2;
        }

        /// <summary>
        /// Add ressources in the inventory
        /// </summary>
        /// <param name="res">ressource number</param>
        public void Add(int res)
        {
            this.current_inventory += res;
            if (this.current_inventory > this.max_inventory)
                this.current_inventory = this.max_inventory;
        }

        /// <summary>
        /// Try to remove ressouces in the inventory
        /// </summary>
        /// <param name="res">ressource number</param>
        /// <returns>Return true if success (enough ressources) and false otherwise</returns>
        public bool Remove(int res)
        {
            if (this.current_inventory < res)
                return false;
            this.current_inventory -= res;
            return true;
        }

        /// <summary>
        /// Clear inventory
        /// </summary>
        public void Clear()
        {
            this.current_inventory = 0;
        }

        /// <summary>
        /// Indicate if the unit can give ressources to an agent
        /// </summary>
        /// <param name="agent">the agent to give</param>
        public bool CanGive(GameObject agent)
        {
            return (Vector3.Distance(this.transform.position, agent.transform.position) <= GiveDistance);
        }

        /// <summary>
        /// Give ressources to an agent
        /// </summary>
        /// <param name="agent">the agent to give</param>
        /// <param name="count">number of ressources to transfert</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool Give(GameObject agent, int count)
        {
            if (Vector3.Distance(this.transform.position, agent.transform.position) > GiveDistance)
                return false;

            InventoryController agent_inv = agent.GetComponent<InventoryController>();
            if (agent_inv.Full)
                return false;
            if (this.current_inventory < count)
            {
                agent_inv.Add(this.current_inventory);
                this.current_inventory = 0;
            }
            else {
                this.Remove(count);
                agent_inv.Add(count);
            }
            return true;
        }


    }

}
