using System;
using System.Collections.Generic;
using System.Xml;

namespace WarBotEngine.Editeur
{
	/**
	 * @Author : Edouard BREUILLE
	 * 
	 * @Project : Warbot Unity version : FSM design & implementation
	 * 
	 * @Structure : 
	 * if (conditions)
	 * {
	 *  actions
	 * }
	 * else
	 * {
	 *  actions
	 * }
	 * 
	 **/
	class If: Control
	{
		public static readonly string ELSE_NODE_NAME = "elseActions";

		private List<Instruction> elseActions;
		
		public If()
		{
			setConditions(new List<Condition>());
			setActions(new List<Instruction>());
			setElseActions (new List<Instruction> ());
		}

		public If(List<Condition> c, List<Instruction> a, List<Instruction> ea)
		{
			setConditions(c);
			setActions(a);
			setElseActions (ea);
		}

		public List<Instruction> getElseActions()
		{
			return this.elseActions;
		}

		public void setElseActions(List<Instruction> ea)
		{
			elseActions = new List<Instruction> ();
			foreach (Instruction i in ea) 
			{
				if(i is Action || i is If)
					elseActions.Add(i); 
			}
		}

		/**
		 * Returns True if conditions are true, false otherwise. 
		 */
		public override bool execute(Unit u)
		{
			bool conditionIsTrue = true;

			foreach (Condition c in getConditions()) // check if all conditions are true, breaks if at least one is false
			{
				if (!c.execute(u))
				{
					conditionIsTrue = false;
					break;
				}
			}

			if (conditionIsTrue) { // if all conditions are true, execute all actions
				foreach (Action a in getActions()) {
					if (a.execute (u)) {
						return true;
					}
				}
			}
			else 
			{
				foreach (Action a in getElseActions()) {
					if (a.execute (u)) {
						return true;
					}
				}
			}

			return false; // tells if it's a terminal action
		}

		public override string Description ()
		{
			return "Structure de contrôle pour organiser les actions en blocs de type [if() - then{} - else{}]";
		}

		/**
         * Returns the IfThenElse xml structure
         * <IfThenElse>
         *  <parameters>
         *      <condition></condition>
         *  </parameters>
         *  <actions>
         *      <action1></action1>
         *      <action2></action2>
         *  </actions>
         *  <elseActions>
         *   	<action3></action3>
         *  </elseActions>
         * </IfThenElse>
         */
		public override XmlNode xmlStructure()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode ifNode = doc.CreateElement(this.Type());

			XmlNode paramNode = doc.CreateElement(Control.PARAM_NODE_NAME);
			if(getConditions().Count > 0) {
				foreach(Condition c in getConditions())
				{
					paramNode.AppendChild(doc.ImportNode(c.xmlStructure(), true));
				}
			}
			ifNode.AppendChild(paramNode);

			XmlNode actNode = doc.CreateElement(Control.ACTION_NODE_NAME);
			if (getActions().Count > 0) {
				foreach (Action a in getActions())
				{
					actNode.AppendChild(doc.ImportNode(a.xmlStructure(), true));
				}
			}
			ifNode.AppendChild(actNode);

			XmlNode elseActNode = doc.CreateElement(ELSE_NODE_NAME);
			if (getElseActions().Count > 0) {
				foreach (Action a in getElseActions())
				{
					elseActNode.AppendChild(doc.ImportNode(a.xmlStructure(), true));
				}
			}
			ifNode.AppendChild(elseActNode);

			return ifNode;
		}

        public override Instruction Clone()
        {
            If res = new If();
            foreach (Condition cond in this.conditions)
                res.conditions.Add((Condition)cond.Clone());
            foreach (Instruction ins in this.actions)
                res.actions.Add(ins.Clone());
            foreach (Instruction ins in this.elseActions)
                res.elseActions.Add(ins.Clone());
            return res;
        }
    }
}
