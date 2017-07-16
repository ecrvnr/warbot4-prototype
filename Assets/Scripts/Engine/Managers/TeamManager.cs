using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.WarBots;

namespace WarBotEngine.Managers {

    public class TeamManager : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /// <summary>
        /// Nom de l'équipe
        /// </summary>
        public string team_name;

        /// <summary>
        /// Couleur de l'équipe
        /// </summary>
        public Color color;

        /// <summary>
        /// The minimum amount of time we wait between to unit spawns.
        /// </summary>
        public float spawn_delay;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Liste des unités de l'équipe
        /// </summary>
        public List<GameObject> warbots;

        /// <summary>
        /// The available area to spawn initial units. 
        /// </summary>
        public Area base_spawn_area;

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

        /// <summary>
        /// On awaking this isntance.
        /// </summary>
        private void Awake() {
            this.Init();
        }

        /// <summary>
        /// On Starting this instance
        /// </summary>
        private void Start() {
            Initializer initializer = Initializer.Actual;
            if (initializer != null)
            {
                for (int i = 0; i < initializer.getElement("WarBase"); i++)
                    SpawnUnit(BotType.WarBase);
                for (int i = 0; i < initializer.getElement("WarExplorer"); i++)
                    SpawnUnit(BotType.WarExplorer);
                for (int i = 0; i < initializer.getElement("WarHeavy"); i++)
                    SpawnUnit(BotType.WarHeavy);
                for (int i = 0; i < initializer.getElement("WarEngineer"); i++)
                    SpawnUnit(BotType.WarEngineer);
                for (int i = 0; i < initializer.getElement("WarTurret"); i++)
                    SpawnUnit(BotType.WarTurret);
            }
        }

        /// <summary>
        /// On updating this instance
        /// </summary>
        private void Update() {

        }

        private void Init() {
            this.warbots = new List<GameObject>();
        }

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Vérifie si l'unité est dans cette équipe
        /// </summary>
        /// <param name="agent">unité à tester</param>
        /// <returns>true si dans l'équipe</returns>
        public bool OnTheTeam(GameObject agent) {
            return this.warbots.Contains(agent);
        }

        /// <summary>
        /// Place une unité dans la zone d'apparition en faisant attention au collisions
        /// </summary>
        /// <param name="type">type de l'unité</param>
        public void SpawnUnit(BotType type) {
            Vector3 position = this.base_spawn_area.RandomPoint();
            var lookPos = Vector3.zero - position;
            var rotation = Quaternion.LookRotation(lookPos);
            rotation *= Quaternion.Euler(0.0f , Random.Range(-45.0f , 45.0f) , 0.0f);
            SpawnUnit(type , position , rotation);
        }

        /// <summary>
        /// Place une unité aux coordonnées indiquées
        /// </summary>
        /// <param name="type">type de l'unité</param>
        /// <param name="position">position de l'unité</param>
        /// <param name="orientation">orientation de l'unité</param>
        public void SpawnUnit(BotType type , Vector3 position , Quaternion orientation) {
            GameObject unit = Instantiate(type.PrefabByType() , position , orientation , this.transform);
            InitUnit(unit , type);
        }

        /// <summary>
        /// Initialise une nouvelle unité
        /// </summary>
        /// <param name="agent">nouvelle unité</param>
        public void InitUnit(GameObject agent , BotType type) {
            this.warbots.Add(agent);
            agent.GetComponent<HealthController>().DeathEvent += this.OnWarBotDead;
            agent.GetComponent<WarBotController>().TeamManager = this;
            agent.GetComponent<WarBotController>().Type = type;
        }

        /// <summary>
        /// Adds the given unit to this team
        /// </summary>
        /// <param name=""></param>
        public void AddWarBot(GameObject agent) {
            if(agent.GetComponent<WarBotController>().team_manager != null) {
                agent.GetComponent<HealthController>().DeathEvent -= agent.GetComponent<WarBotController>().team_manager.OnWarBotDead;
                agent.GetComponent<WarBotController>().team_manager.warbots.Remove(agent);
            }

            this.warbots.Add(agent);
            agent.GetComponent<HealthController>().DeathEvent += this.OnWarBotDead;
            agent.GetComponent<WarBotController>().TeamManager = this;
        }

        /// <summary>
        /// This function will get called whenever a warbot dies.
        /// </summary>
        /// <param name="warbot"></param>
        private void OnWarBotDead(GameObject warbot) {
            if (this.warbots.Contains(warbot)) {
                this.warbots.Remove(warbot);
            }

        }

    }
}