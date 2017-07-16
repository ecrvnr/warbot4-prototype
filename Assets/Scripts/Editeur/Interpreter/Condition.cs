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
	 * condition ()
	 * {
	 *  unit.ThisAction()
	 * }
	 * @Resume :
	 * Calls the corresponding action of the unit
	 * What is "Condition" in the design pattern is classical method for the unit
	 **/
	public class Condition : Instruction
	{
		/// <summary>
		/// The condition.
		/// </summary>
        private string condition;

		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.Condition"/> class.
		/// </summary>
		/// <param name="c">C.</param>
        public Condition(string c)
        {
            condition = c;
        }

		/// <summary>
		/// Calls the unit corresponding method
		/// </summary>
		/// <param name="u">U.</param>
        public override bool execute(Unit u)
        {
            if (needTextInput())
                u.Message = this.AddText;
            return u.GetCondition(condition).Invoke();
        }
        
        public override string Type()
        {
            return condition;
        }

		public override string Description ()
		{
			return Unit.GetDescription (condition);
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
			Condition c = new Condition (condition);
			c.AddText = this.AddText;
            return c;
        }

		public override bool needTextInput ()
		{
			return Unit.NeedMessage(this.condition);
		}

    }
}
