using System;
using System.Collections.Generic;

namespace WarBotEngine.Editeur
{
	/**
	 * @Author : Celia Rouquairol
	 * 
	 * @Project : Warbot Unity version : FSM design & implementation
	 * 
	 * @Resume :
	 * Checks if all conditions are true
	 * If it is so, execute all actions
	 * Return true if all conditions were true, false otherwise.
	 * It doesn't matter if some actions return false, as long as the conditions are true the control is considered executed.
	 **/
	public abstract class Control : Instruction
	{
		/// <summary>
		/// The name of the node for the parameters in the xml.
		/// </summary>
		public static readonly string PARAM_NODE_NAME = "parameters";

		/// <summary>
		/// The name of the node for the action in the xml.
		/// </summary>
		public static readonly string ACTION_NODE_NAME = "actions";

		/// <summary>
		/// conditions to check
		/// </summary>
		protected List<Condition> conditions;

        /// <summary>
        /// actions to execute
        /// </summary>
        protected List<Instruction> actions; 

		// Getters
		public List<Condition> getConditions() { return conditions; }
		public List<Instruction> getActions() { return actions; }

        // Setters
        public void setConditions(List<Condition> c) { conditions = c; }
		public void setActions(List<Instruction> a) 
		{ 
			actions = new List<Instruction> ();
			foreach (Instruction i in a) 
			{
				if(i is Action || i is If)
					actions.Add(i); 
			}
		}
        public void addCondition(Condition c) { conditions.Add(c); }
		public void addAction(Instruction a) { if(a is Action || a is If) actions.Add(a); }

		public override bool needTextInput ()
		{
			return false;
		}
    }

}
