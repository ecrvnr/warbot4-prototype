using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WarBotEngine.Editeur
{

    /// <summary>
    /// Gestionnaire des widgets à l'écran
    /// </summary>
    public class MainLayout : MonoBehaviour
    {


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
        /// Gestionnaire actuel
        /// </summary>
        private static MainLayout actual;

		/// <summary>
		/// La liste de tous les widgets
		/// </summary>
		private List<Widget> widgets;

		/// <summary>
		/// La zone de drage and drop contenant la fsm.
		/// </summary>
		private BehaviorEditor editor;

		/// <summary>
		/// La liste des categories de primitives.
		/// </summary>
		private PrimitivesCollection primitivesCollection;

		/// <summary>
		/// Le selecteur d'équipes et d'unités.
		/// </summary>
		private TeamSelection teamSelection;

        /// <summary>
        /// Position du curseur de la souris
        /// </summary>
        private Vector2 mouse_position;

        /// <summary>
        /// Matériel pour dessiner en OpenGL
        /// </summary>
        private static Material lineMaterial;

        /// <summary>
        /// Widget de glissez-déposez
        /// </summary>
        private DragAndDrop drag_and_drop;

        /// <summary>
        /// Conteneur supérieur pour les widgets flottants (ex: conteneur interne du sélecteur)
        /// </summary>
        private Container upper_container;

        /// <summary>
        /// Indique si l'on doit afficher avec OpenGL
        /// </summary>
        private bool display_opengl;


        /****************************
         ****** OBJETS D'UNITY ******
         ****************************/


        /// <summary>
        /// Canvas dans Unity
        /// </summary>
        public GameObject MainCanvas;

        /// <summary>
        /// UI_Manager dans Unity
        /// </summary>
        public GameObject UI_Canvas;

		public GameObject UI_Text;


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
        /// Layout actuel
        /// </summary>
        public static MainLayout Actual { get { return actual; } }

        /// <summary>
        /// Liste des widgets
        /// </summary>
        public Widget[] Widgets { get { return widgets.ToArray(); } }

        /// <summary>
        /// Objet de Drag&Drop
        /// </summary>
        public DragAndDrop Dropper { get { return this.drag_and_drop; } }

        /// <summary>
        /// Conteneur de widgets flottants
        /// </summary>
        public Widget UpperContainer { get { return this.upper_container; } }

        /// <summary>
        /// Conteneur de l'éditeur des boites
        /// </summary>
		public BehaviorEditor Editor {
            get { return this.editor; }
            set
            {
                this.RemoveWidget(this.editor);
                this.editor = value;
                this.AddWidget(this.editor);
                this.RemoveWidget(this.drag_and_drop);
                this.drag_and_drop.Editor = this.editor;
                this.AddWidget(this.drag_and_drop);
            }
        }

        /// <summary>
        /// Conteneur de la collection des primitives
        /// </summary>
		public PrimitivesCollection Primitives_collection { 
			get { return this.primitivesCollection; } 
			set{
				this.RemoveWidget (this.primitivesCollection);
				this.primitivesCollection = value;
				this.AddWidget (this.primitivesCollection);
				this.RemoveWidget (this.drag_and_drop);
				this.drag_and_drop.Primitives_collection = this.primitivesCollection;
				this.AddWidget (this.drag_and_drop);
			} 
		}
        
        /// <summary>
        /// Conteneur de sélection des équipes et unités
        /// </summary>
		public TeamSelection Team_selection{ get { return this.teamSelection; } }

        /// <summary>
        /// Raycaster de l'UI
        /// </summary>
        public UnityEngine.UI.GraphicRaycaster UI_Raycaster { get { return this.UI_Canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>(); } }

		/// <summary>
		/// Gets the U i text raycaster.
		/// </summary>
		/// <value>The U i text raycaster.</value>
		public UnityEngine.UI.GraphicRaycaster UI_TextRaycaster { get { return this.UI_Text.GetComponent<UnityEngine.UI.GraphicRaycaster> (); } }

        /// <summary>
        /// Indique si l'UI est affiché ou assigne son état
        /// </summary>
        public bool DisplayUI
        {
            get
            {
                return !this.display_opengl;
            }
            set
            {
                this.display_opengl = !value;
				this.UI_Raycaster.enabled = value;
				this.UI_TextRaycaster.enabled = value;
            }
        }

        /****************************
         ****** APPELS D'UNITY ******
         ****************************/


        /// <summary>
        /// Appelée à l'initialisation du GameObject
        /// </summary>
        void Start()
        {
            MainLayout.actual = this;
            mouse_position = new Vector2();
			widgets = new List<Widget> ();
            this.upper_container = new Container(new Rect(0, 0, Screen.width, Screen.height));

			this.teamSelection = new TeamSelection();
            this.editor = new BehaviorEditor(this.teamSelection.Team_selector.Selection, this.teamSelection.Unit_selector.Selection);
            this.primitivesCollection = new PrimitivesCollection(this.teamSelection.Unit_selector.Selection);
			this.AddWidget (editor);
			this.AddWidget (primitivesCollection);
			this.AddWidget (teamSelection);

			this.drag_and_drop = new DragAndDrop(primitivesCollection, editor);
            this.AddWidget(this.drag_and_drop);
            this.AddWidget(this.upper_container);

            if (this.teamSelection.Team_selector.Elements.Length > 0)
            {
                this.DisplayUI = false;
            }
            else
            {
                this.DisplayUI = true;
                GameObject.Find("Button_Return").GetComponent<UnityEngine.UI.Button>().interactable = false;
                this.UI_Text.SetActive(false);
            }
        }

        /// <summary>
        /// Appelée à l'update du GameObject (chaque frame)
        /// </summary>
        void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Appelée à chaque évènement
        /// </summary>
        void OnGUI()
        {
            Event e = Event.current;
            if (mouse_position != e.mousePosition)
            {
                mouse_position = e.mousePosition;
                OnMotionEvent((int)e.mousePosition.x, (int)e.mousePosition.y);
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (this.display_opengl)
                        OnMouseEvent(e.button, true, (int)e.mousePosition.x, (int)e.mousePosition.y);
                    break;
                case EventType.MouseUp:
                    if (this.display_opengl)
                        OnMouseEvent(e.button, false, (int)e.mousePosition.x, (int)e.mousePosition.y);
                    break;
                case EventType.MouseMove:
                    if (this.display_opengl)
                        OnMotionEvent((int)e.mousePosition.x, (int)e.mousePosition.y);
                    break;
                case EventType.ScrollWheel:
                    if (this.display_opengl)
                        OnScrollEvent((int)e.delta.y);
                    break;
                case EventType.KeyDown:
                    OnKeyEvent(e.keyCode, true);
                    break;
                case EventType.KeyUp:
                    OnKeyEvent(e.keyCode, false);
                    break;
                case EventType.Repaint:
                    if (this.display_opengl)
                        OnDraw();
                    break;
            }
        }


        /*********************************
         ****** METHODES DE GESTION ******
         *********************************/
        

        /// <summary>
        /// Ajoute les matrices d'OpenGL
        /// </summary>
        public void PushGL()
        {
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.MultMatrix(Matrix4x4.TRS(new Vector3(0, 1, 0), Quaternion.identity, new Vector3((float)1 / (float)Screen.width, (float)-1 / (float)Screen.height)));
        }

        /// <summary>
        /// Retire les matrices d'OpenGL
        /// </summary>
        public void PopGL()
        {
            GL.PopMatrix();
        }


        /*********************************
         ****** METHODES DE GESTION ******
         *********************************/


        /// <summary>
        /// Ajoute un widget à la liste
        /// </summary>
        /// <param name="obj"></param>
        public void AddWidget(Widget obj)
        {
            widgets.Add(obj);
        }

        /// <summary>
        /// Supprime un widget de la liste
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveWidget(Widget obj)
        {
            widgets.Remove(obj);
        }

        /// <summary>
        /// Supprime tous les widgets de la liste
        /// </summary>
        public void Clear()
        {
            foreach (Widget o in widgets)
            {
                o.OnDestroy();
            }
        }

        /// <summary>
        /// Crée un matériau pour l'affichage avec les fonctions d'OpenGL
        /// </summary>
        private static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }


        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        /// <summary>
        /// Appelée lors de l'affichage
        /// </summary>
        public void OnDraw()
        {
            CreateLineMaterial();
            lineMaterial.SetPass(0);
            this.PushGL();
			this.editor.OnDraw();
			this.primitivesCollection.OnDraw();
			this.teamSelection.OnDraw();
			this.drag_and_drop.OnDraw ();
			this.upper_container.OnDraw ();
            this.PopGL();
        }

        /// <summary>
        /// Appelée lors de l'update
        /// </summary>
        public void OnUpdate()
        {
            foreach (Widget o in widgets)
            {
                o.OnUpdate();
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement clavier
        /// </summary>
        /// <param name="keycode">touche associée à l'évènement</param>
        /// <param name="state">indique si la touche est appuyée ou non</param>
        public void OnKeyEvent(KeyCode keycode, bool state)
        {
            //print("[Key]: " + keycode + " = " + state);
			if (keycode == KeyCode.Escape && state)
			{
				this.UI_Text.SetActive (this.DisplayUI);
				this.DisplayUI = !this.DisplayUI;
			}

            if (this.upper_container.CanInteract())
                this.upper_container.OnKeyEvent(keycode, state);
            else {
                foreach (Widget o in widgets)
                {
                    if (this.upper_container != o)
                        o.OnKeyEvent(keycode, state);
                }
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement souris de bouton
        /// </summary>
        /// <param name="button">bouton associé à l'évènement</param>
        /// <param name="pressed">indique si le bouton est appuyé ou non</param>
        /// <param name="x">coordonnée en x de la souris</param>
        /// <param name="y">coordonnée en y de la souris</param>
        public void OnMouseEvent(int button, bool pressed, int x, int y)
        {
            //print("[Mouse]: " + button + " = " + pressed + " -> " + x + " , " + y);
            if (this.upper_container.CanInteract())
                this.upper_container.OnMouseEvent(button, pressed, x, y);
            else {
                foreach (Widget o in widgets)
                {
                    if (this.upper_container != o && o.GlobalArea.Contains(new Vector2(x, y)))
                        o.OnMouseEvent(button, pressed, x, y);
                }
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement souris de mouvement
        /// </summary>
        /// <param name="x">coordonnée en x de la souris</param>
        /// <param name="y">coordonnée en y de la souris</param>
        public void OnMotionEvent(int x, int y)
        {
            //print("[Motion]: " + x + " , " + y);
            if (this.upper_container.CanInteract())
                this.upper_container.OnMotionEvent(x, y);
            else {
                foreach (Widget o in widgets)
                {
                    if (this.upper_container != o && o.GlobalArea.Contains(new Vector2(x, y)))
                        o.OnMotionEvent(x, y);
                }
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement souris de scrolling
        /// </summary>
        /// <param name="delta">valeur de scrolling</param>
        public void OnScrollEvent(int delta)
        {
            //print("[Scroll]: " + delta);
            if (this.upper_container.CanInteract())
                this.upper_container.OnScrollEvent(delta);
            else {
                foreach (Widget o in widgets)
                {
                    if (this.upper_container != o && o.GlobalArea.Contains(this.mouse_position))
                        o.OnScrollEvent(delta);
                }
            }
        }
        
    }

}
