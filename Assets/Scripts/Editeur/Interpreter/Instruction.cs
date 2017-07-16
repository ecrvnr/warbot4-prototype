using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace WarBotEngine.Editeur
{
	/**
	 * @Author : Celia Rouquairol
	 * 
	 * @Project : Warbot Unity version : FSM design & implementation
	 * 
	 * @Resume :
	 * An instruction to execute
	 **/
	public abstract class Instruction
    {
		/// <summary>
		/// The name of the attribute containing the additionalText in xml.
		/// </summary>
		public static readonly string TEXT_ATTRIBUTE_NAME = "additionalText";

		/// <summary>
		/// The additional text.
		/// </summary>
		private string additionalText = "";

        /***********/
        /* METHODS */
        /***********/

		/// <summary>
		/// Gets or sets the add text.
		/// </summary>
		/// <value>The add text.</value>
		public string AddText{
			get{ return this.additionalText;} 
			set{ 
				if (needTextInput()) {
					this.additionalText = value;	
				}
			}
		}

        /// <summary>
		/// Calls the unit corresponding method
        /// </summary>
        /// <param name="u">U.</param>
        public abstract bool execute(Unit u);

        /// <summary>
		/// Returns the instruction xml structure
        /// </summary>
        /// <returns>The structure.</returns>
        public abstract XmlNode xmlStructure();

		/// <summary>
		/// Return the name of the instruction
		/// </summary>
		public virtual string Type()
		{
			return this.GetType ().Name;
		}

		/// <summary>
		/// Return the description of this instruction
		/// </summary>
		public virtual string Description()
		{
			return "Instruction";
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
        public abstract Instruction Clone();

		/// <summary>
		/// Tells if the instruction needs a text
		/// </summary>
		/// <returns><c>true</c>, if text input was needed, <c>false</c> otherwise.</returns>
		public abstract bool needTextInput ();
    }
}
