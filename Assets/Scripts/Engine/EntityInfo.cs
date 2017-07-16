using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine
{

    /// <summary>
    /// Contain some informations about entity
    /// </summary>
    public class EntityInfo
    {

        ////// ATTRIBUTE //////

        /// <summary>
        /// The entity
        /// </summary>
        private GameObject entity;

        ////// ACCESSORS //////

        /// <summary>
        /// Public getter of the entity
        /// </summary>
        public GameObject Entity { get { return entity; } }

        /// <summary>
        /// Indicate if the entity is a unit
        /// </summary>
        public bool WarUnit { get { return this.entity.CompareTag("WarBot"); } }

        /// <summary>
        /// Indicate if the entity is a ressource
        /// </summary>
        public bool WarRessource { get { return this.entity.CompareTag("WarItem"); } }

        /// <summary>
        /// Indicate if the entity is a projectile
        /// </summary>
        public bool WarProjectile { get { return this.entity.CompareTag("WarProjectile"); } }

        /// <summary>
        /// Public getter of the entity position
        /// </summary>
        public Vector3 Position { get { return this.entity.transform.position; } }

        /// <summary>
        /// Public getter of the unit's team ID
        /// </summary>
        public int TeamID
        {
            get
            {
                Managers.GameManager game_manager = Managers.GameManager.instance;
                if (game_manager.team1.GetComponent<Managers.TeamManager>().OnTheTeam(this.entity))
                    return 1;
                else if (game_manager.team2.GetComponent<Managers.TeamManager>().OnTheTeam(this.entity))
                    return 2;
                else
                    return 0;
            }
        }

        ////// CONSTRUCTOR //////

        /// <summary>
        /// Basic constructor of entity informations
        /// </summary>
        /// <param name="entity"></param>
        public EntityInfo(GameObject entity)
        {
            this.entity = entity;
        }

        ////// METHODS //////

        /// <summary>
        /// Indicate if the agent is an enemy
        /// </summary>
        /// <param name="agent">The agent</param>
        public bool IsEnemy(GameObject agent)
        {
            return this.WarUnit && entity.GetComponent<WarBots.WarBotController>().TeamManager != this.entity.GetComponent<WarBots.WarBotController>().TeamManager;
        }

        /// <summary>
        /// Indicate if the agent is an alliee
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public bool IsAlliee(GameObject agent)
        {
            return this.WarUnit && entity.GetComponent<WarBots.WarBotController>().TeamManager == this.entity.GetComponent<WarBots.WarBotController>().TeamManager;
        }

        /// <summary>
        /// Get alliees excepted self
        /// </summary>
        public GameObject[] GetAlliees()
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>(this.entity.GetComponent<WarBots.WarBotController>().TeamManager.warbots);
            result.Remove(this.entity);
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee bases excepted self
        /// </summary>
        public GameObject[] GetAllieeBases()
        {
            if (!this.WarUnit)
                return null;
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarBase)
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee explorers excepted self
        /// </summary>
        public GameObject[] GetAllieeExplorers()
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarExplorer)
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee turrets excepted self
        /// </summary>
        public GameObject[] GetAllieeTurrets()
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarTurret)
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee heavies excepted self
        /// </summary>
        public GameObject[] GetAllieeHeavies()
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarHeavy)
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee engineers excepted self
        /// </summary>
        public GameObject[] GetAllieeEngineers()
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarEngineer)
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee in a group excepted self
        /// </summary>
        public GameObject[] GetAllieesInGroup(string group)
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.GroupController>().InsideGroup(group))
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee in group with a role excepted self
        /// </summary>
        public GameObject[] GetAllieesInGroupRole(string group, string role)
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (GameObject a in this.GetAlliees())
            {
                if (a.GetComponent<WarBots.GroupController>().InsideGroup(group, role))
                    result.Add(a);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get alliee senders of a specified message
        /// </summary>
        public GameObject[] GetAllieesSendersOf(string msg)
        {
            if (!this.WarUnit)
                return null;

            List<GameObject> result = new List<GameObject>();
            foreach (WarBots.MessageController.Message message in this.entity.GetComponent<WarBots.MessageController>().Messages)
            {
                if (message.Content[0] == msg)
                    result.Add(message.Sender);
            }
            return result.ToArray();
        }

    }

}
