using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
    /// The unit which containt the behavior and run instructions
    /// </summary>
	public class Unit
    {


        /////////////////////////
        ////// ENUMERATORS //////
        /////////////////////////
        

        /// <summary>
        /// Types of a primitive
        /// </summary>
        public enum PRIMITVE_TYPE
        {
            CONDITION,
            ACTION
        }


        //////////////////////////////////////////
        ////// PRIMITIVES CUSTOM ATTRIBUTES //////
        //////////////////////////////////////////


        /// <summary>
        /// Primitive attribute (type of the primitive)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class PrimitiveTypeAttribute : System.Attribute
        {

            /// <summary>
            /// Type of the primitive
            /// </summary>
            protected PRIMITVE_TYPE value;

            /// <summary>
            /// Type of the primitive
            /// </summary>
            public PRIMITVE_TYPE Value { get { return value; } }

            /// <summary>
            /// Constructor of the custom attribute
            /// </summary>
            /// <param name="type">type of the primitve</param>
            public PrimitiveTypeAttribute(PRIMITVE_TYPE type)
            {
                this.value = type;
            }

        }

        /// <summary>
        /// Primitive attribute (allowed unit)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class UnitAllowedAttribute : System.Attribute
        {

            /// <summary>
            /// Allowed unit
            /// </summary>
            protected WarBots.BotType value;

            /// <summary>
            /// Allowed unit
            /// </summary>
            public WarBots.BotType Value { get { return value; } }

            /// <summary>
            /// Constructor of the custom attribute
            /// </summary>
            /// <param name="unit">unit to allow</param>
            public UnitAllowedAttribute(WarBots.BotType unit)
            {
                this.value = unit;
            }

        }

        /// <summary>
        /// Primitive attribute (description of the primitive)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class PrimitiveDescriptionAttribute : System.Attribute
        {

            /// <summary>
            /// Description of the primitive
            /// </summary>
            protected string value;

            /// <summary>
            /// Description of the primitive
            /// </summary>
            public string Value { get { return value; } }

            /// <summary>
            /// Constructor of the custom attribute
            /// </summary>
            /// <param name="type">description of the primitve</param>
            public PrimitiveDescriptionAttribute(string descr)
            {
                this.value = descr;
            }

        }

        /// <summary>
        /// Primitive attribute (if the primitive need a message)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class PrimitiveMessageNeededAttribute : System.Attribute
        {

            /// <summary>
            /// Description of the primitive
            /// </summary>
            protected bool value;

            /// <summary>
            /// Description of the primitive
            /// </summary>
            public bool Value { get { return value; } }

            /// <summary>
            /// Constructor of the custom attribute
            /// </summary>
            /// <param name="type">if the primitive need a message</param>
            public PrimitiveMessageNeededAttribute(bool needed = true)
            {
                this.value = needed;
            }

        }


        ///////////////////////
        ////// DELEGATES //////
        ///////////////////////


        /// <summary>
        /// Delegate of an action
        /// </summary>
        public delegate bool ActionDelegate();

        /// <summary>
        /// Delegate of a control
        /// </summary>
        /// <returns></returns>
        public delegate bool ControlDelegate();


        ////////////////////////
        ////// ATTRIBUTES //////
        ////////////////////////


        /// <summary>
        /// The GameObject of the unit
        /// </summary>
        private GameObject agent;

        /// <summary>
        /// The behavior of the unit
        /// </summary>
        private List<Instruction> behavior;

        /// <summary>
        /// The current target
        /// </summary>
        private object target;

        /// <summary>
        /// The type of unit to be spawned
        /// </summary>
        private WarBots.BotType selected_unit;

        /// <summary>
        /// A message to send
        /// </summary>
        private string message;


        //////////////////////////////
        ////// STATIC ACCESSORS //////
        //////////////////////////////


        /// <summary>
        /// Get all condition names
        /// </summary>
        public static string[] Conditions
        {
            get
            {
                List<string> result = new List<string>();
                foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
                {
                    PrimitiveTypeAttribute[] attributes = (PrimitiveTypeAttribute[])method.GetCustomAttributes(typeof(PrimitiveTypeAttribute), true);
                    if (attributes.Length > 0 && attributes[0].Value == PRIMITVE_TYPE.CONDITION)
                        result.Add(method.Name);
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Get all action names
        /// </summary>
        public static string[] Actions
        {
            get
            {
                List<string> result = new List<string>();
                foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
                {
                    PrimitiveTypeAttribute[] attributes = (PrimitiveTypeAttribute[])method.GetCustomAttributes(typeof(PrimitiveTypeAttribute), true);
                    if (attributes.Length > 0 && attributes[0].Value == PRIMITVE_TYPE.ACTION)
                        result.Add(method.Name);
                }
                return result.ToArray();
            }
        }


        ///////////////////////
        ////// ACCESSORS //////
        ///////////////////////


        /// <summary>
        /// The GameObject of the unit
        /// </summary>
        public GameObject Agent { get { return this.agent; } }

        /// <summary>
        /// The behavior of the unit
        /// </summary>
        public Instruction[] Behavior { get { return this.behavior.ToArray(); } }

        /// <summary>
        /// The BehaviorController
        /// </summary>
        public WarBots.BehaviorController Controller { get { return this.agent.GetComponent<WarBots.BehaviorController>(); } }

        /// <summary>
        /// Determine if the unit can move
        /// </summary>
        public bool Movable
        {
            get
            {
                WarBots.BotType type = this.GetUnitType();
                return (type == WarBots.BotType.WarEngineer ||
                        type == WarBots.BotType.WarExplorer ||
                        type == WarBots.BotType.WarHeavy);
            }
        }

        /// <summary>
        /// Determine if the unit can shoot
        /// </summary>
        public bool Shooter
        {
            get
            {
                WarBots.BotType type = this.GetUnitType();
                return (type == WarBots.BotType.WarHeavy ||
                        type == WarBots.BotType.WarTurret);
            }
        }

        /// <summary>
        /// Determine if the unit can create
        /// </summary>
        public bool Creator
        {
            get
            {
                WarBots.BotType type = this.GetUnitType();
                return (type == WarBots.BotType.WarBase ||
                        type == WarBots.BotType.WarEngineer);
            }
        }

        /// <summary>
        /// A message to send
        /// </summary>
        public string Message { get { return this.message; } set { this.message = value; } }


        /////////////////////////
        ////// CONSTRUCTOR //////
        /////////////////////////


        /// <summary>
        /// Basic constructor of a unit
        /// </summary>
        /// <param name="agent">the GameObject of the unit</param>
        /// <param name="beh">the behavior of the unit</param>
        public Unit(GameObject agent, List<Instruction> beh)
        {
            this.agent = agent;
            this.behavior = beh;
            this.target = null;
        }


        ////////////////////////////
        ////// STATIC METHODS //////
        ////////////////////////////


        /// <summary>
        /// Get all conditions
        /// </summary>
        /// <returns>Return an array of conditions</returns>
        public static string[] GetConditions()
        {
            return Unit.Conditions;
        }

        /// <summary>
        /// Get all conditions of the specified unit
        /// </summary>
        /// <param name="unit">the unit</param>
        /// <returns>Return conditions if unit exist and null otherwise</returns>
        public static string[] GetConditions(string unit)
        {
            foreach (WarBots.BotType u in Enum.GetValues(typeof(WarBots.BotType)))
            {
                if (u.ToString() == unit)
                    return GetConditions(u);
            }
            return null;
        }

        /// <summary>
        /// Get all conditions of the specified unit
        /// </summary>
        /// <param name="unit">the unit</param>
        /// <returns>Return conditions</returns>
        public static string[] GetConditions(WarBots.BotType unit)
        {
            List<string> result = new List<string>();
            foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
            {
                PrimitiveTypeAttribute[] attributes = (PrimitiveTypeAttribute[])method.GetCustomAttributes(typeof(PrimitiveTypeAttribute), true);
                if (attributes.Length > 0 && attributes[0].Value == PRIMITVE_TYPE.CONDITION)
                {
                    foreach (UnitAllowedAttribute u in (UnitAllowedAttribute[])method.GetCustomAttributes(typeof(UnitAllowedAttribute), true))
                    {
                        if (u.Value == unit)
                        {
                            result.Add(method.Name);
                            break;
                        }
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all actions
        /// </summary>
        /// <returns>Return an array of actions</returns>
        public static string[] GetActions()
        {
            return Unit.Actions;
        }

        /// <summary>
        /// Get all actions of the specified unit
        /// </summary>
        /// <param name="unit">the unit</param>
        /// <returns>Return actions if unit exist and null otherwise</returns>
        public static string[] GetActions(string unit)
        {
            foreach (WarBots.BotType u in Enum.GetValues(typeof(WarBots.BotType)))
            {
                if (u.ToString() == unit)
                    return GetActions(u);
            }
            return null;
        }

        /// <summary>
        /// Get all actions of the specified unit
        /// </summary>
        /// <param name="unit">the unit</param>
        /// <returns>Return actions</returns>
        public static string[] GetActions(WarBots.BotType unit)
        {
            List<string> result = new List<string>();
            foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
            {
                PrimitiveTypeAttribute[] attributes = (PrimitiveTypeAttribute[])method.GetCustomAttributes(typeof(PrimitiveTypeAttribute), true);
                if (attributes.Length > 0 && attributes[0].Value == PRIMITVE_TYPE.ACTION)
                {
                    foreach (UnitAllowedAttribute u in (UnitAllowedAttribute[])method.GetCustomAttributes(typeof(UnitAllowedAttribute), true))
                    {
                        if (u.Value == unit)
                        {
                            result.Add(method.Name);
                            break;
                        }
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get the description of a primitive
        /// </summary>
        /// <param name="name">name of the primitive</param>
        /// <returns>Return the description if exist and "" otherwise</returns>
        public static string GetDescription(string name)
        {
            foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
            {
                if (method.Name != name)
                    continue;

                PrimitiveDescriptionAttribute[] attributes = (PrimitiveDescriptionAttribute[])method.GetCustomAttributes(typeof(PrimitiveDescriptionAttribute), true);
                if (attributes.Length > 0)
                    return attributes[0].Value;
            }
            return "";
        }

        /// <summary>
        /// Indicate if a primitive need a text/message
        /// </summary>
        /// <param name="name">name of the primitive</param>
        /// <returns>Return true if needed and false otherwise</returns>
        public static bool NeedMessage(string name)
        {
            foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
            {
                if (method.Name != name)
                    continue;

                PrimitiveMessageNeededAttribute[] attributes = (PrimitiveMessageNeededAttribute[])method.GetCustomAttributes(typeof(PrimitiveMessageNeededAttribute), true);
                if (attributes.Length > 0)
                    return attributes[0].Value;
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Prepare a formated message/text and send it
        /// </summary>
        /// <param name="content">the content message to send</param>
        public bool PrepareAndSendMessage(string content)
        {
            Regex regex = new Regex(@"^\s*((@to\-[a-z\-]+)(\[.+\])? )?(\@target |\@self )?(.*)$");
            Match match = regex.Match(content);
            if (match.Success)
            {
                GroupCollection groups = match.Groups;

                //The receivers of the message
                GameObject[] receivers = null;
                if (groups[2].Success)
                    receivers = this.GetFormatedUnits(groups[2].Value, groups[3].Value);
                else if (this.target != null && this.target.GetType() == typeof(GameObject) && (new EntityInfo(this.agent)).IsAlliee((GameObject)this.target))
                        receivers = new GameObject[] { (GameObject)this.target };
                if (receivers == null)
                    return false;

                //If a target need to be attached
                GameObject message_target = null;
                if (groups[2].Success)
                    message_target = this.GetFormatedTarget(groups[4].Value);

                //The message content
                WarBots.MessageController.Message message_content = new WarBots.MessageController.Message(this.agent, new string[]
                {
                    groups[5].Value,
                    this.ConvertToFormatedPosition(message_target)
                });

                //Send the message
                WarBots.MessageController self = this.agent.GetComponent<WarBots.MessageController>();
                foreach (GameObject a in receivers)
                    self.SendMessage(message_content, a);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Get units receivers by text of type "@to-receiver[args]"
        /// </summary>
        /// <param name="text">"@to-"receiver</param>
        /// <param name="args">[args]</param>
        /// <returns>Return receivers if success and null otherwise</returns>
        public GameObject[] GetFormatedUnits(string text, string args)
        {
            EntityInfo info = new EntityInfo(this.agent);
            switch (text)
            {
                case "@to-all":
                    return info.GetAlliees();
                case "@to-bases":
                    return info.GetAllieeBases();
                case "@to-explorers":
                    return info.GetAllieeExplorers();
                case "@to-turrets":
                    return info.GetAllieeTurrets();
                case "@to-heavies":
                    return info.GetAllieeHeavies();
                case "@to-engineers":
                    return info.GetAllieeEngineers();
                case "@to-group":
                    return info.GetAllieesInGroup(args.Substring(1, args.Length - 2));
                case "@to-group-role":
                    string[] splited = args.Substring(1, args.Length - 2).Split(',');
                    if (splited.Length == 2)
                        return info.GetAllieesInGroupRole(splited[0], splited[1]);
                    else
                        return null;
                case "@to-senders-of":
                    return info.GetAllieesSendersOf(args.Substring(1, args.Length - 2));
            }
            return null;
        }

        /// <summary>
        /// Get target to join on the message by text @target or @self
        /// </summary>
        /// <param name="text">"@target" or "@self"</param>
        /// <returns>Return target if success and null otherwise</returns>
        public GameObject GetFormatedTarget(string text)
        {
            switch (text)
            {
                case "@target":
                    if (this.target != null && this.target.GetType() == typeof(GameObject))
                        return (GameObject)this.target;
                    else
                        return null;
                case "@self":
                    return this.agent;
            }
            return null;
        }

        /// <summary>
        /// Get a formated text of a GameObject position
        /// </summary>
        /// <param name="a">the GameObject</param>
        /// <returns>Return a text of type "x,y,z" if success and "" otherwise</returns>
        public string ConvertToFormatedPosition(GameObject a)
        {
            if (a == null)
                return "";
            else
                return a.transform.position.x + "," + a.transform.position.y + "," + a.transform.position.z;
        }


        /////////////////////
        ////// METHODS //////
        /////////////////////


        /// <summary>
        /// Execute the unit behavior
        /// </summary>
        public void Run()
        {
            this.target = null;
            this.message = null;
            foreach (Instruction i in behavior)
            {
                if (i.execute(this))
                    break;
            }
            this.agent.GetComponent<WarBots.MessageController>().Clear();
        }

        /// <summary>
        /// Get the condition specified by the name
        /// </summary>
        /// <param name="name">name of the control</param>
        /// <returns>Return a delegate if success and null otherwise</returns>
        public ControlDelegate GetCondition(string name)
        {
            foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
            {
                PrimitiveTypeAttribute[] attributes = (PrimitiveTypeAttribute[])method.GetCustomAttributes(typeof(PrimitiveTypeAttribute), true);
                if (attributes.Length > 0 && attributes[0].Value == PRIMITVE_TYPE.CONDITION && method.Name == name)
                    return (ControlDelegate)Delegate.CreateDelegate(typeof(ControlDelegate), this, method);
            }
            return null;
        }

        /// <summary>
        /// Get the action specified by the name
        /// </summary>
        /// <param name="name">name of the action</param>
        /// <returns>Return a delegate if success and null otherwise</returns>
        public ActionDelegate GetAction(string name)
        {
            foreach (System.Reflection.MethodInfo method in typeof(Unit).GetMethods())
            {
                PrimitiveTypeAttribute[] attributes = (PrimitiveTypeAttribute[])method.GetCustomAttributes(typeof(PrimitiveTypeAttribute), true);
                if (attributes.Length > 0 && attributes[0].Value == PRIMITVE_TYPE.ACTION && method.Name == name)
                    return (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), this, method);
            }
            return null;
        }

        /// <summary>
        /// Return the type of the unit
        /// </summary>
        /// <returns>Return the type of the unit</returns>
        protected WarBots.BotType GetUnitType()
        {
            return this.agent.GetComponent<WarBots.WarBotController>().Type;
        }


        ////////////////////////
        ////// PRIMITIVES //////
        ////////////////////////


        // !!!!!! ALL CONTROL/ACTION METHODS NEED TO RETURN BOOL WITHOUT PARAMETERS AND BE PUBLIC !!!!!!


        ////// RANDOM //////


        /// <summary>
        /// Do a roll of dice
        /// </summary>
        /// <returns>Return result of the roll of dice</returns>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Fait un jet de dés pour valider ou non la condition (indiquez une valeur entre 1 et 100 [50 par défaut], validée si le jet est inférieur à la valeur)")]
        [PrimitiveMessageNeeded]
        public bool RollOfDice()
        {
            int value = 50;
            if (this.message != "")
            {
                if (!int.TryParse(this.message, out value) || (value < 1 || value > 100))
                    value = 0;
            }
            return UnityEngine.Random.Range(1, 100) <= value;
        }


        ////// WALKING //////


        /// <summary>
        /// The unit walk forward
        /// </summary>
        /// <returns>Return true if action success and false otherwise</returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Fait avancer l'unité (termine l'action si réussi)")]
        public bool Walk()
        {
            if (this.Movable)
            {
                this.agent.GetComponent<WarBots.WalkController>().Walk();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The unit turn with a specific target (current target)
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Oriente l'unité vers une cible prédéfinie (à utiliser après une instruction Target...)")]
        public bool Turn()
        {
            if (this.target == null)
                return false;

            if (this.Movable)
            {
                if (this.target.GetType() == typeof(float))
                {
                    this.agent.GetComponent<WarBots.WalkController>().Turn((float)this.target);
                }
                else if (this.target.GetType() == typeof(Vector3))
                {
                    this.agent.GetComponent<WarBots.WalkController>().Turn((Vector3)this.target);
                }
                else if (this.target.GetType() == typeof(GameObject))
                {
                    this.agent.GetComponent<WarBots.WalkController>().Turn((GameObject)this.target);
                }
            }
            return false;
        }

        /// <summary>
        /// The unit turn with 180 degrees
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Fait faire demi-tour à l'unité")]
        public bool HalfTurn()
        {
            if (this.Movable)
            {
                this.agent.GetComponent<WarBots.WalkController>().Turn(180f);
            }
            return false;
        }

        /// <summary>
        /// The unit turn with a random value
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Oriente l'unité aléatoirement")]
        public bool RandomTurn()
        {
            if (this.Movable)
            {
                this.agent.GetComponent<WarBots.WalkController>().Turn(UnityEngine.Random.Range(0f, 360f));
            }
            return false;
        }


        ////// ATTACKING //////


        /// <summary>
        /// The unit aim his weapon on a target
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [PrimitiveDescription("Oriente l'arme de l'unité vers une cible prédéfinie")]
        public bool Aim()
        {
            if (this.target == null)
                return false;

            if (this.target.GetType() == typeof(Vector3))
                this.agent.GetComponent<WarBots.AttackController>().Aim((Vector3)this.target);
            else if (this.target.GetType() == typeof(GameObject))
                this.agent.GetComponent<WarBots.AttackController>().Aim((GameObject)this.target);
            return false;
        }

        /// <summary>
        /// The unit shoot a projectile if is reloading
        /// </summary>
        /// <returns>Return true if action success and false otherwise</returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [PrimitiveDescription("Fait tirer l'unité (termine l'action si réussi)")]
        public bool Shoot()
        {
            return this.agent.GetComponent<WarBots.AttackController>().Shoot();
        }

        /// <summary>
        /// Check if unit is reloading
        /// </summary>
        /// <returns>Return true if is reloading and false otherwise</returns>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [PrimitiveDescription("Validée si l'arme de l'unité est chargée")]
        public bool Reloaded()
        {
            return this.Shooter && this.agent.GetComponent<WarBots.AttackController>().Reloaded();
        }


        ////// GATHERING //////


        /// <summary>
        /// Give a ressource to the target
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Vérifié si l'unité peut donner des ressources à une unité alliée (assez proche)")]
        public bool CanGive()
        {
            if (this.target == null)
                return false;

            EntityInfo info = new EntityInfo(this.agent);
            if (this.target.GetType() == typeof(GameObject) && info.IsAlliee((GameObject)this.target))
                return this.agent.GetComponent<WarBots.InventoryController>().CanGive((GameObject)this.target);
            else
                return false;
        }

        /// <summary>
        /// Give a ressource to the target
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Donne une ressource à l'unité cible alliée (termine l'action si réussi, à utiliser après une instruction Target...)")]
        public bool Give()
        {
            if (this.target == null)
                return false;

            EntityInfo info = new EntityInfo(this.agent);
            if (this.target.GetType() == typeof(GameObject) && info.IsAlliee((GameObject)this.target))
                return this.agent.GetComponent<WarBots.InventoryController>().Give((GameObject)this.target, WarBots.InventoryController.GiveCount);
            else
                return false;
        }

        /// <summary>
        /// Indicate if inventory is full
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Soigne l'unité (termine l'action si réussi)")]
        public bool Heal()
        {
            WarBots.HealthController health = this.agent.GetComponent<WarBots.HealthController>();
            WarBots.InventoryController inventory = this.agent.GetComponent<WarBots.InventoryController>();
            if (health.Full || inventory.CurrentInventory < WarBots.InventoryController.HealCost)
                return false;
            health.IncreaseHealth(WarBots.InventoryController.HealValue);
            inventory.Remove(WarBots.InventoryController.HealCost);
            return true;
        }

        /// <summary>
        /// Indicate if inventory is full
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée si l'inventaire est plein")]
        public bool Full()
        {
            return this.agent.GetComponent<WarBots.InventoryController>().Full;
        }

        /// <summary>
        /// Indicate if inventory is not full
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée si l'inventaire n'est pas plein")]
        public bool NotFull()
        {
            return !this.agent.GetComponent<WarBots.InventoryController>().Full;
        }

        /// <summary>
        /// Indicate if inventory is empty
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée si l'inventaire est vide")]
        public bool Empty()
        {
            return this.agent.GetComponent<WarBots.InventoryController>().Empty;
        }

        /// <summary>
        /// Indicate if inventory is not empty
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée si l'inventaire n'est pas vide")]
        public bool NotEmpty()
        {
            return !this.agent.GetComponent<WarBots.InventoryController>().Empty;
        }


        ////// SPAWNING //////


        /// <summary>
        /// Create a unit
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Crée l'unité indiquée (termine l'action si réussi, à utiliser après une instruction Select...)")]
        public bool CreateUnit()
        {
            if (this.Creator && this.Controller.GetComponent<WarBots.SpawnController>().CanCreate(this.selected_unit))
                return this.Controller.GetComponent<WarBots.SpawnController>().SpawnUnit(this.selected_unit);
            else
                return false;
        }

        /// <summary>
        /// Check if the unit can create
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée si l'unité indiquée peut être créée (à utiliser après une instruction Select...)")]
        public bool CanCreate()
        {
            return this.Controller.GetComponent<WarBots.SpawnController>().CanCreate(this.selected_unit);
        }


        ////// IDLE //////


        /// <summary>
        /// Idle
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Ne fait rien (termine l'action)")]
        public bool Idle()
        {
            return true;
        }


        ////// TARGETS //////


        /// <summary>
        /// Set target to the neareast enemy
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'ennemi le plus proche")]
        public bool TargetNearestEnemy()
        {
            List<GameObject> enemies = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            if (enemies.Count > 0)
            {
                this.target = enemies[0];
                float distance = Vector3.Distance(this.Controller.transform.position, enemies[0].transform.position), tmp;
                for (int i = 1; i < enemies.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, enemies[i].transform.position)) < distance)
                    {
                        this.target = enemies[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to a random enemy
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible un ennemi aléatoirement")]
        public bool TargetRandomEnemy()
        {
            List<GameObject> enemies = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            if (enemies.Count > 0)
            {
                this.target = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)];
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast alliee
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'allié le plus proche")]
        public bool TargetNearestAlliee()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            if (alliees.Count > 0)
            {
                this.target = alliees[0];
                float distance = Vector3.Distance(this.Controller.transform.position, alliees[0].transform.position), tmp;
                for (int i = 1; i < alliees.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, alliees[i].transform.position)) < distance)
                    {
                        this.target = alliees[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to a random alliee
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible un allié aléatoirement")]
        public bool TargetRandomAlliee()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            if (alliees.Count > 0)
            {
                this.target = alliees[UnityEngine.Random.Range(0, alliees.Count - 1)];
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast ressource
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible la ressource la plus proche")]
        public bool TargetNearestRessource()
        {
            List<GameObject> ressources = this.Controller.GetComponent<WarBots.DetectionController>().DetectedItems;
            if (ressources.Count > 0)
            {
                this.target = ressources[0];
                float distance = Vector3.Distance(this.Controller.transform.position, ressources[0].transform.position), tmp;
                for (int i = 1; i < ressources.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, ressources[i].transform.position)) < distance)
                    {
                        this.target = ressources[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast alliee base
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible la base alliée la plus proche")]
        public bool TargetNearABase()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarBase)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast alliee turret
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible la tourelle alliée la plus proche")]
        public bool TargetNearATurret()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarTurret)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast alliee tank
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible le tank allié le plus proche")]
        public bool TargetNearAHeavy()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarHeavy)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast alliee explorer
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'explorateur allié le plus proche")]
        public bool TargetNearAExplorer()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarExplorer)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast alliee engineer
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'ingénieur alliée la plus proche")]
        public bool TargetNearAEngineer()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarEngineer)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast enemy base
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible la base ennemie la plus proche")]
        public bool TargetNearEBase()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarBase)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast enemy turret
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible la tourelle ennemie la plus proche")]
        public bool TargetNearETurret()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarTurret)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast enemy tank
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible le tank ennemi le plus proche")]
        public bool TargetNearEHeavy()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarHeavy)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast enemy explorer
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'explorateur ennemi le plus proche")]
        public bool TargetNearEExplorer()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarExplorer)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }

        /// <summary>
        /// Set target to the neareast enemy engineer
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'ingénieur ennemi la plus proche")]
        public bool TargetNearEEngineer()
        {
            List<GameObject> alliees = this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies;
            List<GameObject> agents = new List<GameObject>();
            foreach (GameObject a in alliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarEngineer)
                    agents.Add(a);
            }
            if (agents.Count > 0)
            {
                this.target = agents[0];
                float distance = Vector3.Distance(this.Controller.transform.position, agents[0].transform.position), tmp;
                for (int i = 1; i < agents.Count; i++)
                {
                    if ((tmp = Vector3.Distance(this.Controller.transform.position, agents[i].transform.position)) < distance)
                    {
                        this.target = agents[i];
                        distance = tmp;
                    }
                }
            }
            else
            {
                this.target = null;
            }
            return false;
        }


        ////// UNIT SELECTION //////


        /// <summary>
        /// Select a turret unit for spawn
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Sélectionne l'unité WarTurret pour la création")]
        public bool SelectWarTurret()
        {
            this.selected_unit = WarBots.BotType.WarTurret;
            return false;
        }

        /// <summary>
        /// Select an explorer unit for spawn
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [PrimitiveDescription("Sélectionne l'unité WarExplorer pour la création")]
        public bool SelectWarExplorer()
        {
            this.selected_unit = WarBots.BotType.WarExplorer;
            return false;
        }

        /// <summary>
        /// Select an explorer unit for spawn
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [PrimitiveDescription("Sélectionne l'unité WarHeavy pour la création")]
        public bool SelectWarHeavy()
        {
            this.selected_unit = WarBots.BotType.WarHeavy;
            return false;
        }

        /// <summary>
        /// Select an explorer unit for spawn
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [PrimitiveDescription("Sélectionne l'unité WarEngineer pour la création")]
        public bool SelectWarEngineer()
        {
            this.selected_unit = WarBots.BotType.WarEngineer;
            return false;
        }

        /// <summary>
        /// Select an explorer unit for spawn
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [PrimitiveDescription("Sélectionne une unité aléatoire pour la création")]
        public bool SelectRandomUnit()
        {
            WarBots.BotType[] values = (WarBots.BotType[])Enum.GetValues(typeof(WarBots.BotType));
            int random_value = UnityEngine.Random.Range(0, values.Length - 2);
            for (int i = 0, index = 0; i < values.Length; i++)
            {
                if (values[i] != WarBots.BotType.WarBase && values[i] != WarBots.BotType.WarTurret && random_value == index++)
                    this.selected_unit = values[i];
            }
            return false;
        }


        ////// NEAR PERCEPTS //////


        /// <summary>
        /// Test if enemies are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des ennemis à proximité")]
        public bool NearEnemies()
        {
            return (this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies.Count > 0);
        }

        /// <summary>
        /// Test if alliees are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des alliés à proximité")]
        public bool NearAlliees()
        {
            return (this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees.Count > 0);
        }

        /// <summary>
        /// Test if ressources are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des ressources à proximité")]
        public bool NearRessources()
        {
            return (this.Controller.GetComponent<WarBots.DetectionController>().DetectedItems.Count > 0);
        }

        /// <summary>
        /// Test if alliee bases are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des bases alliées à proximité")]
        public bool NearAllieeBases()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarBase)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if alliee turrets are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des tourelles alliées à proximité")]
        public bool NearAllieeTurret()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarTurret)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if alliee heavys are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des tanks alliés à proximité")]
        public bool NearAllieeHeavy()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarHeavy)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if alliee explorers are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des explorateurs alliés à proximité")]
        public bool NearAllieeExplorer()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarExplorer)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if alliee engineers are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des ingénieurs alliés à proximité")]
        public bool NearAllieeEngineers()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleAlliees)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarEngineer)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if enemy bases are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des bases ennemies à proximité")]
        public bool NearEnemyBases()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarBase)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if enemy turrets are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des tourelles ennemies à proximité")]
        public bool NearEnemyTurret()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarTurret)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if enemy heavys are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des tanks ennemis à proximité")]
        public bool NearEnemyHeavy()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarHeavy)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if enemy explorers are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des explorateurs ennemis à proximité")]
        public bool NearEnemyExplorer()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarExplorer)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Test if enemy engineers are visible
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Validée s'il y a des ingénieurs ennemis à proximité")]
        public bool NearEnemyEngineers()
        {
            foreach (GameObject a in this.Controller.GetComponent<WarBots.DetectionController>().VisibleEnemies)
            {
                if (a.GetComponent<WarBots.WarBotController>().Type == WarBots.BotType.WarEngineer)
                    return true;
            }
            return false;
        }


        ////// GROUPS MANAGEMENT //////


        /// <summary>
        /// Add the unit in a group
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Ajoute l'unité à un groupe")]
        [PrimitiveMessageNeeded]
        public bool EnterGroup()
        {
            if (this.message != "")
            {
                string[] splited = this.message.Split(' ');
                if (message.Length == 1)
                    this.agent.GetComponent<WarBots.GroupController>().EnterGroup(splited[0]);
                else
                    this.agent.GetComponent<WarBots.GroupController>().EnterGroup(splited[0], splited[1]);
            }
            return false;
        }

        /// <summary>
        /// Remove the unit of a group
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Supprime l'unité d'un groupe")]
        [PrimitiveMessageNeeded]
        public bool LeaveGroup()
        {
            if (this.message != "")
            {
                this.agent.GetComponent<WarBots.GroupController>().LeaveGroup(this.message);
            }
            return false;
        }

        /// <summary>
        /// Indicate if a unit is in a group
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Indique si l'unité est dans un groupe (le nom du groupe est facultatif)")]
        [PrimitiveMessageNeeded]
        public bool InsideGroup()
        {
            if (this.message != "")
            {
                return (this.agent.GetComponent<WarBots.GroupController>().InsideGroup(this.message));
            }
            else
            {
                return (this.agent.GetComponent<WarBots.GroupController>().Groups.Length > 0);
            }
        }

        /// <summary>
        /// Change the unit's role of in a group
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Change le rôle de l'unité dans un groupe")]
        [PrimitiveMessageNeeded]
        public bool ChangeRole()
        {
            if (this.message != "")
            {
                string[] splited = this.message.Split(' ');
                if (splited.Length == 2)
                    this.agent.GetComponent<WarBots.GroupController>().ChangeRole(splited[0], splited[1]);
            }
            return false;
        }


        ////// MESSAGE MANAGMENT //////


        /// <summary>
        /// Indicate if a specified message was received
        /// </summary>
        [PrimitiveType(PRIMITVE_TYPE.CONDITION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Vérifié si l'on a reçu le message indiqué")]
        [PrimitiveMessageNeeded]
        public bool ReceivedMessage()
        {
            if (this.message != "")
            {
                foreach (WarBots.MessageController.Message msg in this.agent.GetComponent<WarBots.MessageController>().Messages)
                {
                    if (msg.Content[0] == this.message)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Send a message to other units
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Envoi un message à d'autres unités")]
        [PrimitiveMessageNeeded]
        public bool SendMessage()
        {
            if (this.message != "")
                this.PrepareAndSendMessage(this.message);
            return false;
        }

        /// <summary>
        /// Target the nearest unit that sended the specified message
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'unité la plus proche ayant envoyé le message indiqué")]
        [PrimitiveMessageNeeded]
        public bool TargetNearestMessage()
        {
            if (this.message != "")
            {
                List<GameObject> list = new List<GameObject>();
                foreach (WarBots.MessageController.Message msg in this.agent.GetComponent<WarBots.MessageController>().Messages)
                {
                    if (msg.Content[0] == this.message)
                        list.Add(msg.Sender);
                }
                if (list.Count > 0)
                {
                    this.target = list[0];
                    float distance = Vector3.Distance(this.Controller.transform.position, list[0].transform.position), tmp;
                    for (int i = 1; i < list.Count; i++)
                    {
                        if ((tmp = Vector3.Distance(this.Controller.transform.position, list[i].transform.position)) < distance)
                        {
                            this.target = list[i];
                            distance = tmp;
                        }
                    }
                }
                else
                {
                    this.target = null;
                }
            }
            return false;
        }

        /// <summary>
        /// Target a random unit that sended the specified message
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible une unité ayant envoyé le message indiqué")]
        [PrimitiveMessageNeeded]
        public bool TargetRandomMessage()
        {
            if (this.message != "")
            {
                List<GameObject> list = new List<GameObject>();
                foreach (WarBots.MessageController.Message msg in this.agent.GetComponent<WarBots.MessageController>().Messages)
                {
                    if (msg.Content[0] == this.message)
                    {
                        this.target = msg.Sender;
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Target the nearest entity that was sended in the specified message
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible l'entitée la plus proche se trouvant dans le message indiqué")]
        [PrimitiveMessageNeeded]
        public bool TargetNearestMsgContent()
        {
            if (this.message != "")
            {
                List<Vector3> list = new List<Vector3>();
                foreach (WarBots.MessageController.Message msg in this.agent.GetComponent<WarBots.MessageController>().Messages)
                {
                    if (msg.Content[0] == this.message && msg.Content[1] != "")
                    {
                        string[] splited = msg.Content[1].Split(',');
                        list.Add(new Vector3(
                            (float)Convert.ToDouble(splited[0]),
                            (float)Convert.ToDouble(splited[1]),
                            (float)Convert.ToDouble(splited[2])
                        ));
                    }
                }
                if (list.Count > 0)
                {
                    this.target = list[0];
                    float distance = Vector3.Distance(this.Controller.transform.position, list[0]), tmp;
                    for (int i = 1; i < list.Count; i++)
                    {
                        if ((tmp = Vector3.Distance(this.Controller.transform.position, list[i])) < distance)
                        {
                            this.target = list[i];
                            distance = tmp;
                        }
                    }
                }
                else
                {
                    this.target = null;
                }
            }
            return false;
        }

        /// <summary>
        /// Target a random entity that was sended in the specified message
        /// </summary>
        /// <returns></returns>
        [PrimitiveType(PRIMITVE_TYPE.ACTION)]
        [UnitAllowed(WarBots.BotType.WarBase)]
        [UnitAllowed(WarBots.BotType.WarTurret)]
        [UnitAllowed(WarBots.BotType.WarHeavy)]
        [UnitAllowed(WarBots.BotType.WarExplorer)]
        [UnitAllowed(WarBots.BotType.WarEngineer)]
        [PrimitiveDescription("Cible une entitée se trouvant dans le message indiqué")]
        [PrimitiveMessageNeeded]
        public bool TargetRandomMsgContent()
        {
            if (this.message != "")
            {
                List<GameObject> list = new List<GameObject>();
                foreach (WarBots.MessageController.Message msg in this.agent.GetComponent<WarBots.MessageController>().Messages)
                {
                    if (msg.Content[0] == this.message && msg.Content[1] != "")
                    {
                        string[] splited = msg.Content[1].Split(',');
                        this.target = new Vector3(
                            (float)Convert.ToDouble(splited[0]),
                            (float)Convert.ToDouble(splited[1]),
                            (float)Convert.ToDouble(splited[2])
                        );
                        return false;
                    }
                }
            }
            return false;
        }

    }
}
