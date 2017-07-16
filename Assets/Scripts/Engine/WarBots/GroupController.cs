using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.WarBots
{

    /// <summary>
    /// Groups and roles controller
    /// </summary>
    public class GroupController : MonoBehaviour
    {


        ////// CLASS DEFINITION //////


        /// <summary>
        /// Group of units
        /// </summary>
        public class Group
        {


            ////// ATTRIBUTES //////


            /// <summary>
            /// Name of the group
            /// </summary>
            protected string name;

            /// <summary>
            /// List of units that are in the group
            /// </summary>
            protected List<GameObject> members;

            /// <summary>
            /// List of roles for each units
            /// </summary>
            protected List<string> roles;


            ////// ACCESSORS //////


            /// <summary>
            /// Public getter for the name
            /// </summary>
            public string Name { get { return name; } }

            /// <summary>
            /// Public getter for the members list
            /// </summary>
            public GameObject[] Members { get { return members.ToArray(); } }


            ////// CONSTRUCTOR //////


            /// <summary>
            /// Basic constructor of a group
            /// </summary>
            /// <param name="name">name of the group</param>
            public Group(string name)
            {
                this.name = name;
                this.members = new List<GameObject>();
                this.roles = new List<string>();
            }


            ////// METHODS //////


            /// <summary>
            /// Add a unit in the group
            /// </summary>
            /// <param name="agent">the unit which must to add</param>
            public void Add(GameObject agent)
            {
                this.members.Add(agent);
                this.roles.Add("");
            }

            /// <summary>
            /// Add a unit in the group and assign his role
            /// </summary>
            /// <param name="agent">the unit which must to add</param>
            /// <param name="role">the role of the unit</param>
            public void Add(GameObject agent, string role)
            {
                this.members.Add(agent);
                this.roles.Add(role);
            }

            /// <summary>
            /// Remove a unit of the group
            /// </summary>
            /// <param name="agent">the unit which must to remove of the group</param>
            public void Remove(GameObject agent)
            {
                int index = this.members.IndexOf(agent);
                if (index >= 0)
                {
                    this.members.RemoveAt(index);
                    this.roles.RemoveAt(index);
                }
            }

            /// <summary>
            /// Determines if the unit is in the group
            /// </summary>
            /// <param name="agent">the unit to test</param>
            /// <returns>Return true if the unit is in the group and false otherwise</returns>
            public bool Inside(GameObject agent)
            {
                return this.members.Contains(agent);
            }

            /// <summary>
            /// Remove all members in the group
            /// </summary>
            public void Clear()
            {
                this.members.Clear();
                this.roles.Clear();
            }

            /// <summary>
            /// Return the role of the unit in the group
            /// </summary>
            /// <param name="agent">the unit</param>
            /// <returns>Return the role if the unit is in the group and "" otherwise</returns>
            public string GetRole(GameObject agent)
            {
                int index = this.members.IndexOf(agent);
                if (index < 0)
                    return "";
                else
                    return this.roles[index];
            }

            /// <summary>
            /// Assign a role for the unit in the group
            /// </summary>
            /// <param name="agent">the unit</param>
            /// <param name="role">the role to assign</param>
            public void SetRole(GameObject agent, string role)
            {
                int index = this.members.IndexOf(agent);
                if (index >= 0)
                    this.roles[index] = role;
            }

            /// <summary>
            /// Clear the role of the unit in the group
            /// </summary>
            /// <param name="agent">the unit</param>
            public void ClearRole(GameObject agent)
            {
                this.SetRole(agent, "");
            }

            /// <summary>
            /// Return the members that have a role assigned
            /// </summary>
            /// <returns>Return the list</returns>
            public GameObject[] GetMembersWithRole()
            {
                List<GameObject> res = new List<GameObject>();
                for (int i = 0; i < this.members.Count; i++)
                {
                    if (this.roles[i] != "")
                        res.Add(this.members[i]);
                }
                return res.ToArray();
            }

            /// <summary>
            /// Return the members that have the role assigned
            /// </summary>
            /// <param name="role">role to test</param>
            /// <returns>Return the list</returns>
            public GameObject[] GetMembersWithRole(string role)
            {
                List<GameObject> res = new List<GameObject>();
                for (int i = 0; i < this.members.Count; i++)
                {
                    if (this.roles[i] == role)
                        res.Add(this.members[i]);
                }
                return res.ToArray();
            }

            /// <summary>
            /// Return the members that have no role assigned
            /// </summary>
            /// <returns>Return the list</returns>
            public GameObject[] GetMembersWithoutRole()
            {
                return this.GetMembersWithRole("");
            }

        }


        ////// STATIC ATTRIBUTES //////


        /// <summary>
        /// List of groups in the first team
        /// </summary>
        protected static List<Group> groupsTeam1 = new List<Group>();

        /// <summary>
        /// List of groups in the second team
        /// </summary>
        protected static List<Group> groupsTeam2 = new List<Group>();


        ////// STATIC ACCESSORS //////


        /// <summary>
        /// Public getter for the list of groups in the first team
        /// </summary>
        public static Group[] Team1Groups { get { return groupsTeam1.ToArray(); } }

        /// <summary>
        /// Public getter for the list of groups in the second team
        /// </summary>
        public static Group[] Team2Groups { get { return groupsTeam2.ToArray(); } }


        ////// STATIC METHODS //////


        /// <summary>
        /// Add a group in the specified team
        /// </summary>
        /// <param name="team">ID of the team (1 for team 1 and 2 for team 2)</param>
        /// <param name="group">the group which must to add</param>
        /// <returns>Return true if success and false otherwise</returns>
        public static bool AddGroup(int team, Group group)
        {
            if (team == 1)
                groupsTeam1.Add(group);
            else if (team == 2)
                groupsTeam2.Add(group);
            else
                return false;
            return true;
        }

        /// <summary>
        /// Remove a group of the specified team
        /// </summary>
        /// <param name="team">ID of the team (1 for team 1 and 2 for team 2)</param>
        /// <param name="group">the group which must to remove</param>
        /// <returns>Return true if success and false otherwise</returns>
        public static bool RemoveGroup(int team, Group group)
        {
            if (team == 1)
                groupsTeam1.Remove(group);
            else if (team == 2)
                groupsTeam2.Remove(group);
            else
                return false;
            return true;
        }

        /// <summary>
        /// Remove all groups
        /// </summary>
        public static void ClearGroups()
        {
            groupsTeam1.Clear();
            groupsTeam2.Clear();
        }

        /// <summary>
        /// Find a group in the specified team by its name
        /// </summary>
        /// <param name="team">ID of the team (1 for team 1 and 2 for team 2)</param>
        /// <param name="name">name of the group</param>
        /// <returns>Return the group if exist in the team and null otherwise</returns>
        public static Group FindGroup(int team, string name)
        {
            List<Group> list;
            if (team == 1)
                list = groupsTeam1;
            else if (team == 2)
                list = groupsTeam2;
            else
                return null;

            foreach (Group g in list)
            {
                if (g.Name == name)
                    return g;
            }
            return null;
        }

        /// <summary>
        /// Return the list of units which have the specified role in all groups in the specified team
        /// </summary>
        /// <param name="team">ID of the team (1 for team 1 and 2 for team 2)</param>
        /// <param name="role">the role of the units</param>
        /// <returns>Return the list units and null if the ID of the team is not correct</returns>
        public static GameObject[] FindWithRole(int team, string role)
        {
            List<Group> list;
            if (team == 1)
                list = groupsTeam1;
            else if (team == 2)
                list = groupsTeam2;
            else
                return null;

            List<GameObject> res = new List<GameObject>();
            foreach (Group g in list)
            {
                foreach (GameObject agent in g.GetMembersWithRole(role))
                {
                    if (!res.Contains(agent))
                        res.Add(agent);
                }
            }
            return res.ToArray();
        }


        ////// ATTRIBUTES //////


        /// <summary>
        /// Groups of the unit
        /// </summary>
        protected List<Group> groups;


        ////// ACCESSORS //////


        /// <summary>
        /// Public getter for the groups
        /// </summary>
        public Group[] Groups { get { return groups.ToArray(); } }


        ////// UNITY METHODS //////


        // Use this for initialization
        void Start()
        {
            this.Init();
            // When the unit die, leave all groups
            this.gameObject.GetComponent<HealthController>().DeathEvent += this.OnDie;
        }

        // Update is called once per frame
        void Update()
        {

        }


        ////// SPECIFIC METHODS //////


        /// <summary>
        /// Initialise datas
        /// </summary>
        protected void Init()
        {
            groups = new List<Group>();
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
        /// Called when the unit die
        /// </summary>
        /// <param name="agent"></param>
        protected void OnDie(GameObject agent)
        {
            this.LeaveAllGroups();
        }

        /// <summary>
        /// Indicate if the unit is in a specified group
        /// </summary>
        /// <param name="name">name of the group</param>
        public bool InsideGroup(string name)
        {
            foreach (Group g in this.groups)
            {
                if (g.Name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Indicate if the unit is in a specified group and role
        /// </summary>
        /// <param name="name">name of the group</param>
        /// <param name="name">name of the role</param>
        public bool InsideGroup(string name, string role)
        {
            foreach (Group g in this.groups)
            {
                if (g.Name == name && g.GetRole(this.gameObject) == role)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Add the unit in a group
        /// </summary>
        /// <param name="name">name of the group</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool EnterGroup(string name)
        {
            return this.EnterGroup(name, "");
        }

        /// <summary>
        /// Add the unit in a group and assign a role
        /// </summary>
        /// <param name="name">name of the group</param>
        /// <param name="role">role in the group</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool EnterGroup(string name, string role)
        {
            int teamID = this.GetTeamID();
            if (teamID == 0)
                return false;

            Group group = GroupController.FindGroup(teamID, name);
            if (group == null)
            {
                GroupController.AddGroup(teamID, new Group(name));
                group = GroupController.FindGroup(teamID, name);
            }
            if (this.groups.Contains(group))
                return false;

            groups.Add(group);
            group.Add(this.gameObject, role);
            return true;
        }

        /// <summary>
        /// Remove the unit of a group
        /// </summary>
        /// <param name="name">name of the group</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool LeaveGroup(string name)
        {
            foreach (Group g in groups)
            {
                if (g.Name == name)
                {
                    g.Remove(this.gameObject);
                    if (g.Members.Length == 0)
                        GroupController.RemoveGroup(this.GetTeamID(), g);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Change the role of the unit in the specified group
        /// </summary>
        /// <param name="group">the name of the group</param>
        /// <param name="role">the role to assign</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool ChangeRole(string group, string role)
        {
            foreach (Group g in groups)
            {
                if (g.Name == name)
                {
                    g.SetRole(this.gameObject, role);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove the role of the unit in the specified group
        /// </summary>
        /// <param name="group">the name of the group</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool ClearRole(string group)
        {
            foreach (Group g in groups)
            {
                if (g.Name == name)
                {
                    g.ClearRole(this.gameObject);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Leave all groups
        /// </summary>
        public void LeaveAllGroups()
        {
            foreach (Group g in groups)
            {
                g.Remove(this.gameObject);
            }
            groups.Clear();
        }

    }

}
