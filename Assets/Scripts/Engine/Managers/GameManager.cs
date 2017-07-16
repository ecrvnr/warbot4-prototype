using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WarBotEngine.WarBots;
using WarBotEngine.Items;

namespace WarBotEngine.Managers {

    public class GameManager : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        public float spawn_delay;

        public int max_resources;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// The current instance.
        /// </summary>
        public static GameManager instance = null;

        /// <summary>
        /// Team Manager for the first team.
        /// </summary>
        public GameObject team1;

        /// <summary>
        /// Team Manager for the second team.
        /// </summary>
        public GameObject team2;

        /// <summary>
        /// The WarResources currently on the map.
        /// </summary>
        private List<GameObject> resources;

        /// <summary>
        /// The WarBots currently on the map.
        /// </summary>
        private List<GameObject> warbots;

        /// <summary>
        /// Keep tabs on spawning events.
        /// </summary>
        private Coroutine spawn_c;

        /// <summary>
        /// The unit that is currently selected.
        /// </summary>
        private GameObject selected_unit;

        /// <summary>
        /// The game area (the whole map).
        /// </summary>
        public Area game_area;

        /// <summary>
        /// The game over pannel.
        /// </summary>
        public GameObject game_over_pannel;

        /// <summary>
        /// The blur behind the game over pannel.
        /// </summary>
        public GameObject blur;

        /// <summary>
        /// The area where we will spawn resources.
        /// </summary>
        private Area resource_spawn_area;

        /*********************************************** PROPERTIES **************************************************
         * This section holds properties that we want to make publicly accessible without showing them in the editor.*
         *************************************************************************************************************/

        /// <summary>
        /// The number of warbots currently in the game
        /// </summary>
        public static int WarbotsCount {
            get {
                return instance.team1.GetComponent<TeamManager>().warbots.Count + instance.team2.GetComponent<TeamManager>().warbots.Count;
            }
        }

        /// <summary>
        /// The list of all units in game
        /// </summary>
        public static List<GameObject> WarBots {
            get {
                List<GameObject> l = instance.team1.GetComponent<TeamManager>().warbots;
                l.AddRange(instance.team2.GetComponent<TeamManager>().warbots);
                return l;
            }
        }

        /***************************************** DELEGATES AND EVENTS ***************************************************
        * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
        * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
        ******************************************************************************************************************/

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        /// <summary>
        /// On awaking this instance.
        /// </summary>
        private void Awake() {
            instance = this;
            this.game_area = new Area();
        }

        /// <summary>
        /// On Starting this instance
        /// </summary>
        private void Start() {
            this.Init();
        }

        /// <summary>
        /// On updating this instance
        /// </summary>
        private void Update() {

            CheckForClicks();
            if (this.resources.Count < max_resources && this.spawn_c == null) {
                this.spawn_c = StartCoroutine(this.SpawnResource());
            }
        }

        private void LateUpdate() {
            Color c = CheckGameOver();
            if (c != Color.clear) {
                Time.timeScale = 0;
                string equipe_gagnante;
                if (c == Color.blue)
                    equipe_gagnante = "Bleue";
                else
                    equipe_gagnante = "Rouge";

                game_over_pannel.SetActive(true); blur.SetActive(true);
                Transform go_panel = game_over_pannel.transform.Find("Text_GameOver");
                if (go_panel != null)
                    go_panel.gameObject.GetComponent<Text>().text = "Equipe " + equipe_gagnante + " gagne la partie.";
            }
        }

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Initialize the main manager
        /// </summary>
        private void Init() {
            this.resources = new List<GameObject>();
            this.spawn_c = null;
            this.selected_unit = null;
            this.resource_spawn_area = new Area(this.game_area.center , this.game_area.width / 12.0f , this.game_area.height - 30.0f);

            if (Initializer.Actual != null) {
                if (!PropertiesManager.Actual.PropertiesAssigned)
                    PropertiesManager.Actual.AssignProperties();
                GroupController.ClearGroups();
                BehaviorController.ClearBehaviors();
                BehaviorController.LoadBehaviors(1 , Initializer.Actual.red_team);
                BehaviorController.LoadBehaviors(2 , Initializer.Actual.blue_team);

                this.spawn_delay = (float)60 / (float)Initializer.Actual.ressources_time;
                this.max_resources = Items.WarResource.MaxRessources;
            }
            this.InitTeams();
        }

        /// <summary>
        /// Initializes the team managers with the given number of teams (2 by default)
        /// </summary>
        /// <param name="nb_teams"></param>
        private void InitTeams() {
            float area_size_x = this.game_area.width / 2.0f;
            Vector3 position1 = new Vector3(this.game_area.center.x - area_size_x / 2.0f , 0.0f , this.game_area.center.z);
            Vector3 position2 = new Vector3(this.game_area.center.x + area_size_x / 2.0f , 0.0f , this.game_area.center.z);

            this.team1 = Resources.Load("Prefabs/Managers/Team Manager") as GameObject;
            this.team1 = Instantiate(this.team1 , position1 , Quaternion.identity , this.transform);
            this.team1.transform.name = "Team Manager 1";
            this.team1.GetComponent<TeamManager>().color = Color.red;
            this.team1.GetComponent<TeamManager>().base_spawn_area = new Area(position1 -= new Vector3(area_size_x / 4.0f + 25.0f , 0.0f , 0.0f) , area_size_x / 4.0f - 25.0f , this.game_area.height - 50.0f);

            this.team2 = Resources.Load("Prefabs/Managers/Team Manager") as GameObject;
            this.team2 = Instantiate(this.team2 , position2 , Quaternion.identity , this.transform);
            this.team2.transform.name = "Team Manager 2";
            this.team2.GetComponent<TeamManager>().color = Color.blue;
            this.team2.GetComponent<TeamManager>().base_spawn_area = new Area(position2 += new Vector3(area_size_x / 4.0f + 25.0f , 0.0f , 0.0f) , area_size_x / 4.0f - 25.0f , this.game_area.height - 50.0f);
        }

        /// <summary>
        /// Spawn a new Resource.
        /// </summary>
        private IEnumerator SpawnResource() {
            Vector3 position = this.resource_spawn_area.RandomPoint();
            int n = Random.Range(1 , 3);
            GameObject resource = Instantiate(Resources.Load("Prefabs/Items/WarResource" + n.ToString()) as GameObject , position , Quaternion.identity , this.transform) as GameObject;
            System.Console.WriteLine(resource.ToString());
            resource.GetComponent<WarResource>().PickedUpEvent += instance.OnResourcePickedUp;
            this.resources.Add(resource);
            yield return new WaitForSeconds(this.spawn_delay);
            this.spawn_c = null;
            yield break;
        }

        /// <summary>
        /// This function will get triggered when a resource is picked up by a WarExplorer.
        /// </summary>
        /// <param name="war_resource"></param>
        private void OnResourcePickedUp(GameObject war_resource) {

            if (this.resources.Contains(war_resource)) {
                this.resources.Remove(war_resource);
            }
        }

        /// <summary>
        /// Sets the selected unit to null.
        /// </summary>
        public void UnselectUnit() {
            if (instance.selected_unit != null) {
                this.selected_unit.GetComponent<WarBotController>().Unselect();
                this.selected_unit = null;
            }
        }

        /// <summary>
        /// Selects the specified unit
        /// </summary>
        public void SelectUnit(GameObject selector) {
            this.UnselectUnit();
            this.selected_unit = selector;
            this.selected_unit.GetComponent<WarBotController>().Select();
        }

        /// <summary>
        /// Checks whether the user clicked.
        /// </summary>
        public void CheckForClicks() {
            if (Input.GetMouseButtonUp(0)) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray , out hit , 2000.0f)) {
                    if (!(hit.collider.gameObject.CompareTag("UnitSelector")) && !(hit.collider.gameObject.CompareTag("WarBot"))) {
                        instance.UnselectUnit();
                    }
                } else {
                    instance.UnselectUnit();
                }
            }
        }

        /// <summary>
        /// Places initial units on the map. Those units are decided by the sliders in the SelectionMenu.
        /// </summary>
        public void InitializeUnits() {
            //TODO
        }


        /// <summary>
        /// Checks whether the game is finished.
        /// </summary>
        /// <returns></returns>
        public Color CheckGameOver() {
            Object[] count_WarBase = FindObjectsOfType(typeof(WarBase));

            int r_warbase = 0, b_warbase = 0;

            foreach (WarBase warBase in count_WarBase) {
                if (warBase.GetComponent<WarBotController>().TeamManager.color == Color.red) r_warbase++;
                else b_warbase++;
            }

            //Return the winning team.
            if (r_warbase == 0) {
                return Color.blue;
            } else if (b_warbase == 0) {
                return Color.red;
            } else {
                return Color.clear;
            }

        }

    }
}