using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
    /// Conteneur de droite d'édition des comportements en glissez-dépossez
    /// </summary>
    public class BehaviorEditor : Container
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/

            
        /// <summary>
        /// Minimum left space left for the first primitive
        /// </summary>
        private static readonly int DIM_MINIMUM_SPACE = 200;

        /// <summary>
        /// Background color
        /// </summary>
        private static readonly Color BACKGROUND_COLOR = new Color((float)0xe8 / 255, (float)0xec / 255, (float)0xf1 / 255); //#e8ecf1

        /// <summary>
        /// Actual editor
        /// </summary>
        private static BehaviorEditor actual = null;
        

        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
        /// Getter of the actual editor
        /// </summary>
        public static BehaviorEditor Actual { get { return actual; } }

        /// <summary>
        /// Editor's first element
        /// </summary>
        public Primitive First { get { return (Primitive)this.childs[0]; } }

        /// <summary>
        /// Editor's last element
        /// </summary>
        public Primitive Last { get { return (Primitive)this.childs[this.childs.Count-1]; } }

		/// <summary>
		/// Instruction list for ctrl-Z
		/// </summary>
        public Stack<List<Instruction>> cancel_stack = new Stack<List<Instruction>>();

		/// <summary>
		/// Brand new Editor with the specified teamName and unitName.
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="unitName">Unit name.</param>
		public void Set(string teamName, string unitName)
		{
			this.RemoveAllChilds ();
            cancel_stack = new Stack<List<Instruction>>();

            XMLInterpreter interpreter = new XMLInterpreter();
			List<Instruction> instructions = interpreter.xmlToUnitBehavior(teamName, Constants.teamsDirectory, unitName);
            this.Set(instructions);
		}

		/// <summary>
		/// Set the specified instructions.
		/// </summary>
		/// <param name="instructions">Instructions.</param>
        public void Set(List<Instruction> instructions)
        {
            //Debug.Log ("nb instructions :" + instructions.Count);
            this.AllowScrollbar = true;
            this.AllowMotionScroll = true;
            this.Background = BACKGROUND_COLOR;

            this.AddChild(new Primitive(new Vector2(DIM_MINIMUM_SPACE, 0), Primitive.NAME_PRIMITIVE_BEGIN));
            this.First.ExtendHeight += OnExtendHeight;
            //Debug.Log ("Parcours et récupération des " + instructions.Count + " instructions");
            for (int i = instructions.Count - 1; i > -1; i--)
            {
                //Debug.Log (i + " ème instruction");
                //Debug.Log ("Nom de l'instruction "+instructions [i].Type ());
                Primitive p = new Primitive(new Vector2(0, 0), instructions[i].Type(), instructions[i]);
                if (instructions[i].AddText != "")
                    p.AddText = instructions[i].AddText;
                this.Last.PushPrimitive(p, this.Last.GlobalPosition);
            }
        }

        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/


		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.BehaviorEditor"/> class.
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="unitName">Unit name.</param>
        public BehaviorEditor(string teamName, string unitName) : base(new Rect(Screen.width * TeamSelection.DIM_WIDTH, 0, Screen.width * (1 - TeamSelection.DIM_WIDTH), Screen.height))
        {
            BehaviorEditor.actual = this;

            if (teamName != "")
            {
                XMLInterpreter interpreter = new XMLInterpreter();
                List<Instruction> instructions = interpreter.xmlToUnitBehavior(teamName, Constants.teamsDirectory, unitName);
                this.Set(instructions);
                cancel_stack = new Stack<List<Instruction>>();
                cancel_stack.Push(instructions);
            }
        }

        /// <summary>
        /// Called when elements of the editor extends
        /// </summary>
        /// <param name="w">widget</param>
        /// <param name="args">arguments</param>
        public void OnExtendHeight(Widget w, object args)
        {
            this.inner_width = this.First.TotalWidth + 2 * DIM_MINIMUM_SPACE;
            this.motionscroll.ScrollWidth = this.inner_width;
            this.inner_height = this.First.TotalHeight + DIM_MINIMUM_SPACE;
            this.scrollbar.ScrollHeight = this.inner_height;
        }

        /// <summary>
        /// Override of the inherited method to avoid being it executed in the parent code
        /// </summary>
        protected override void RefreshDiplaying() 
		{
		}

		/// <summary>
		/// Called when components are updated
		/// </summary>
		public override void OnUpdate ()
		{
			base.OnUpdate ();
		}

		/// <summary>
		/// Update all the primitives with their instructions
		/// for the graphic and logical part match
		/// </summary>
		public void Maj()
		{
			Primitive p = this.First;
			while (p != null) 
			{
				p.Maj ();
				p = p.Next;
			}
		}

		/// <summary>
		/// Save instruction list for the unit of this team
		/// </summary>
        /// <param name="push_cancel">Indicate if we have to save the instructions in the stack</param>
		public void Save(bool push_cancel = true)
		{
			Maj ();
			XMLInterpreter interpreter = new XMLInterpreter ();
			List<Instruction> instructions = this.behavior ();
            interpreter.behaviorToXml (MainLayout.Actual.Team_selection.Team_selector.Selection, 
				Constants.teamsDirectory, MainLayout.Actual.Team_selection.Unit_selector.Selection, instructions);
            if (push_cancel)
            {
                List<Instruction> copy = new List<Instruction>();
                foreach (Instruction ins in instructions)
                    copy.Add(ins.Clone());
                cancel_stack.Push(copy);
            }
        }

		/// <summary>
		/// Return the instruction list currently showed in the editor
		/// </summary>
		public List<Instruction> behavior()
		{
			List<Instruction> instructions = new List<Instruction> ();
			Primitive p = this.First;
			p = p.Next;
			while (p != null) {
				instructions.Add (p.Instruction);
				p = p.Next;
			}
			return instructions;
		}

        /*********************************
         ****** METHODES DE GESTION ******
         *********************************/


        public override void AddChild(Widget widget)
        {
            base.AddChild(widget);
            foreach (Widget w in childs)
            {
                w.Active = true;
            }
        }

        public override void OnKeyEvent(KeyCode keycode, bool state)
        {
            if (!this.Active)
                return;
            base.OnKeyEvent(keycode, state);
            if (!state && keycode == KeyCode.Z && Input.GetKey(KeyCode.LeftControl))
            {
                if (this.cancel_stack.Count > 1)
                {
                    this.RemoveAllChilds();
                    this.cancel_stack.Pop();
                    this.Set(this.cancel_stack.Peek());
                    this.Save(false);
                }
            }
        }

    }

}
