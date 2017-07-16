using System;
using System.Xml;

namespace WarBotEngine.Editeur
{
	/**
	 * @Author : Celia Rouquairol
	 * 
	 * @Project : Warbot Unity version : FSM design & implementation
	 *
	 * @Structure : 
	 * action.execute (unit)
	 * {
	 *  unit.ThisAction()
	 * }
	 * @Resume :
	 * Calls the corresponding action of the unit
	 **/
	public class Action : Instruction
	{
        private string action;

        public Action(string a)
        {
            action = a;
        }

        public override bool execute(Unit u)
        {
            if (needTextInput())
                u.Message = this.AddText;
            return u.GetAction(action).Invoke();
        }

        public override string Type()
        {
            return action;
        }

		public override string Description ()
		{
			return Unit.GetDescription(action);
		}

        public override XmlNode xmlStructure()
        {
            XmlDocument doc = new XmlDocument();

			XmlNode node = doc.CreateElement(this.Type());

			if (this.needTextInput () && this.AddText.Length > 0) 
			{
				XmlAttribute addText = doc.CreateAttribute(TEXT_ATTRIBUTE_NAME);
				addText.Value = this.AddText;
				node.Attributes.Append(addText);
			}

            return node;
        }

        public override Instruction Clone()
        {
			Action a = new Action(action);
			a.AddText = this.AddText;
			return a;
        }

		public override bool needTextInput ()
		{
			return Unit.NeedMessage(this.action);
		}
    }
}
