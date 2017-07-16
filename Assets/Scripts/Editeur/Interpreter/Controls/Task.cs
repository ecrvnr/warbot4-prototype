using System.Collections.Generic;
using System.Xml;

namespace WarBotEngine.Editeur
{
	/**
	 * @Author : Celia Rouquairol
	 * 
	 * @Project : Warbot Unity version : FSM design & implementation
	 * 
	 * @Structure : 
	 * When (conditions)
	 * {
	 *  actions
	 * }
	 * 
	 * @Resume :
	 * The execute method returns True if the conditions were all true and the actions had been executed, false otherwise.
	 * It doesn't matter if some actions return false as long as the conditions are true, the when control is considered executed.
	 **/
	class Task : Control
	{
		public Task()
        {
            setConditions(new List<Condition>());
			setActions(new List<Instruction>());
        }

		public Task(List<Condition> c, List<Instruction> a)
		{
            setConditions(c);
			setActions(a);
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

			if (conditionIsTrue) // if all conditions are true, execute all actions
			{
				foreach (Action a in getActions())
				{
					if (a.execute (u)) 
					{
						return true;
					}
				}
			}

			return false; // tells if the when control has been executed or not
		}

		public override string Description ()
		{
			return "Structure de contrôle pour organiser les actions en tâches";
		}

        /**
         * Returns the when xml structure
         * <when>
         *  <parameter>
         *      <condition></condition>
         *  </parameter>
         *  <action>
         *      <action1></action1>
         *      <action2></action2>
         *  </action>
         * </when>
         */
        public override XmlNode xmlStructure()
        {
            XmlDocument doc = new XmlDocument();
			XmlNode whenNode = doc.CreateElement(this.Type());

			XmlNode paramNode = doc.CreateElement(Control.PARAM_NODE_NAME);
            if(getConditions().Count > 0) {
                foreach(Condition c in getConditions())
                {
                    paramNode.AppendChild(doc.ImportNode(c.xmlStructure(), true));
                }
			}
			whenNode.AppendChild(paramNode);

			XmlNode actNode = doc.CreateElement(Control.ACTION_NODE_NAME);
            if (getActions().Count > 0) {
				foreach (Instruction a in getActions())
                {
                    actNode.AppendChild(doc.ImportNode(a.xmlStructure(), true));
                }
			}
			whenNode.AppendChild(actNode);

            return whenNode;
        }

        public override Instruction Clone()
        {
            Task res = new Task();
            foreach (Condition cond in this.conditions)
                res.conditions.Add((Condition)cond.Clone());
            foreach (Instruction ins in this.actions)
                res.actions.Add(ins.Clone());
            return res;
        }
    }
}
