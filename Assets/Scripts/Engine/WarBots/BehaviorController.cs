using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.WarBots
{

    /// <summary>
    /// Behaviors controller
    /// </summary>
    public class BehaviorController : MonoBehaviour
    {


        /// <summary>
        /// The behavior of a unit
        /// </summary>
        public class Behavior
        {


            ////////////////////////
            ////// ATTRIBUTES //////
            ////////////////////////


            /// <summary>
            /// Name of the unit
            /// </summary>
            protected string unit_name;

            /// <summary>
            /// List of instructions
            /// </summary>
            protected List<Editeur.Instruction> instr_list;


            ///////////////////////
            ////// ACCESSORS //////
            ///////////////////////


            /// <summary>
            /// Name of the unit
            /// </summary>
            public string Name { get { return unit_name; } }

            /// <summary>
            /// List of instructions
            /// </summary>
            public Editeur.Instruction[] Instructions { get { return instr_list.ToArray(); } }


            /////////////////////////
            ////// CONSTRUCTOR //////
            /////////////////////////


            /// <summary>
            /// Basic constructor of a behavior
            /// </summary>
            /// <param name="unit"></param>
            /// <param name="instr"></param>
            public Behavior(string unit, List<Editeur.Instruction> instr)
            {
                this.unit_name = unit;
                this.instr_list = instr;
            }


            /////////////////////
            ////// METHODS //////
            /////////////////////


            /// <summary>
            /// Return a unit initialised with the behavior and agent
            /// </summary>
            /// <param name="agent">agent to include in the unit</param>
            /// <returns>Return a unit if success and null otherwise</returns>
            public Editeur.Unit GetUnit(GameObject agent)
            {
                if (BotTypes.WarType(agent.GetComponent<WarBotController>().Type) != this.unit_name)
                    return null;

                return new Editeur.Unit(agent, this.instr_list);
            }

        }


        ///////////////////////////////
        ////// STATIC ATTRIBUTES //////
        ///////////////////////////////


        /// <summary>
        /// Behaviors list of team 1
        /// </summary>
        protected static List<Behavior> behavior_team1 = new List<Behavior>();

        /// <summary>
        /// Behaviors list of team 2
        /// </summary>
        protected static List<Behavior> behavior_team2 = new List<Behavior>();


        ////////////////////////////
        ////// STATIC METHODS //////
        ////////////////////////////


        /// <summary>
        /// Load behaviors of a team
        /// </summary>
        /// <param name="team_id">ID of the team (1 for team 1 and 2 for team 2)</param>
        /// <param name="name">the name of the team</param>
        /// <returns>Return true if success and false otherwise</returns>
        public static bool LoadBehaviors(int team_id, string name)
        {
            if (team_id != 1 && team_id != 2)
                return false;

            Editeur.XMLInterpreter interpreter = new Editeur.XMLInterpreter();
            string filename = interpreter.whichFileName(name, Editeur.Constants.teamsDirectory);
            if (filename == "")
                return false;

            Dictionary<string, List<Editeur.Instruction>> dictionary = interpreter.xmlToBehavior(filename, Editeur.Constants.teamsDirectory);
            List<Behavior> team = (team_id == 1) ? behavior_team1 : behavior_team2;
            foreach (string key in dictionary.Keys)
                team.Add(new Behavior(key, dictionary[key]));

            Debug.Log("Comportement de l'équipe " + name + " chargée.");
            return true;
        }

        /// <summary>
        /// Return the behavior of the specified unit
        /// </summary>
        /// <param name="team_id">ID of the team (1 for team 1 and 2 for team 2)</param>
        /// <param name="unit">the unit</param>
        /// <returns>return the behavior if success and null otherwise</returns>
        public static Behavior GetBehavior(int team_id, string unit)
        {
            if (team_id != 1 && team_id != 2)
                return null;

            List<Behavior> team = (team_id == 1) ? behavior_team1 : behavior_team2;
            foreach (Behavior b in team)
            {
                if (b.Name == unit)
                    return b;
            }
            return new Behavior(unit, new List<Editeur.Instruction>());
        }

        /// <summary>
        /// Clear teams behaviors
        /// </summary>
        public static void ClearBehaviors()
        {
            behavior_team1.Clear();
            behavior_team2.Clear();
        }


        ////////////////////////
        ////// ATTRIBUTES //////
        ////////////////////////


        /// <summary>
        /// The unit that contain the behavior of the unit
        /// </summary>
        protected Editeur.Unit unit;


        ///////////////////////////
        ////// UNITY METHODS //////
        ///////////////////////////


        // Use this for initialization
        void Start()
        {
            this.Init();
        }

        // Update is called once per frame
        void Update()
        {
			if (Time.timeScale != 0)
				this.Run();
        }


        //////////////////////////////
        ////// SPECIFIC METHODS //////
        //////////////////////////////


        /// <summary>
        /// Initialise datas
        /// </summary>
        protected void Init()
        {
            Behavior behavior = GetBehavior(this.GetTeamID(), this.GetUnitName());
            if (behavior != null)
                this.unit = behavior.GetUnit(this.gameObject);
            else
                this.unit = null;
        }

        /// <summary>
        /// Return the team ID of the unit
        /// </summary>
        /// <returns>The ID if success and 0 otherwise</returns>
        protected int GetTeamID()
        {
            Managers.GameManager game_manager = Managers.GameManager.instance;
            if (game_manager.team1.GetComponent<Managers.TeamManager>().OnTheTeam(this.gameObject))
                return 1;
            else if (game_manager.team2.GetComponent<Managers.TeamManager>().OnTheTeam(this.gameObject))
                return 2;
            else
                return 0;
        }

        /// <summary>
        /// Return the name of the unit
        /// </summary>
        /// <returns>Return the name of the unit</returns>
        protected string GetUnitName()
        {
            return BotTypes.WarType(this.GetComponent<WarBotController>().Type);
        }

        /// <summary>
        /// Run the behavior
        /// </summary>
        public void Run()
        {
            if (this.unit != null)
                this.unit.Run();
        }

    }

}
