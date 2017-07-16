using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

namespace WarBotEngine.Editeur
{

	/// <summary>
	/// Drop-down
	/// </summary>
	public class Selector : Widget
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
        /// Largeur du trait
        /// </summary>
        private static readonly int DIM_PADDING = 2;
        /// <summary>
        /// Hauteur d'un élément
        /// </summary>
        private static readonly int DIM_ELEMENT_HEIGHT = 22;
        /// <summary>
        /// Taille de la marge d'un élément
        /// </summary>
        private static readonly int DIM_ELEMENT_MARGIN = 15;
        /// <summary>
        /// Dimension du triangle
        /// </summary>
        private static readonly float DIM_TRIANGLE_SIZE = 0.6f;

        /// <summary>
        /// Couleur 1
        /// </summary>
        private static readonly Color COLOR_1 = new Color((float)0x45 / 255, (float)0x3c / 255, (float)0x38 / 255); //453c38
        /// <summary>
        /// Couleur 2
        /// </summary>
        private static readonly Color COLOR_2 = new Color((float)0x6a / 255, (float)0x5c / 255, (float)0x55 / 255); //6a5c55
        /// <summary>
        /// Couleur 3
        /// </summary>
        private static readonly Color COLOR_3 = new Color((float)0xf6 / 255, (float)0x6e / 255, (float)0x00 / 255); //f66e00
        /// <summary>
        /// Couleur 4
        /// </summary>
        private static readonly Color COLOR_CONTAINER_COLOR = new Color((float)0xeb / 255, (float)0xe9 / 255, (float)0xf6 / 255); //ebe9f6


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
        /// Sélection actuelle
        /// </summary>
        protected int selection = -1;

        /// <summary>
        /// Conteneur interne
        /// </summary>
        protected Container container;

        /// <summary>
        /// Hauteur du conteneur
        /// </summary>
        protected int max_height;

        /// <summary>
        /// Texte sur le sélecteur
        /// </summary>
        protected Label main_label;


        /************************************
         ****** EVENEMENTS SPECIFIQUES ******
         ************************************/


        /// <summary>
        /// Appelé lors de la sélection du item
        /// </summary>
        public event Widget.EventDelegate SelectItem = null;

        /// <summary>
        /// Appelé lors du déploiement ou du repli du sélecteur
        /// </summary>
        public event Widget.EventDelegate DeployOrTuck = null;


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
        /// Liste des éléments du sélecteur
        /// </summary>
        public string[] Elements
        {
            get
            {
                int size = this.container.Childs.Length;
                string[] res = new string[size];
                for (int i = 0; i < size; i++)
                {
                    res[i] = ((Label)this.container.Childs[i]).Text;
                }
                return res;
            }
            set
            {
                this.container.RemoveAllChilds();
                for (int i = 0; i < value.Length; i++)
                {
                    Label label = new Label(new Rect(0, i * DIM_ELEMENT_HEIGHT, this.area.width - Scrollbar.DIM_WIDTH, DIM_ELEMENT_HEIGHT), value[i]);
                    label.TextAlign = TextAnchor.MiddleLeft;
                    label.Clic += OnSelect;
                    label.Margin = DIM_ELEMENT_MARGIN;
                    this.container.AddChild(label);
                }
                if (value.Length == 0) this.container.LocalArea = new Rect(this.GlobalArea.x, this.GlobalArea.y + this.area.height, this.area.width, DIM_ELEMENT_HEIGHT);
                else if (value.Length * DIM_ELEMENT_HEIGHT > this.max_height) this.container.LocalArea = new Rect(this.GlobalArea.x, this.GlobalArea.y + this.area.height, this.area.width, this.max_height);
                else this.container.LocalArea = new Rect(this.GlobalArea.x, this.GlobalArea.y + this.area.height, this.area.width, value.Length * DIM_ELEMENT_HEIGHT);

                if (value.Length == 0) this.selection = -1;
                else this.selection = 0;
                this.main_label.Text = this.Selection;
            }
        }

        /// <summary>
        /// Sélection actuelle
        /// </summary>
        public string Selection
        {
            get
            {
                if (this.selection == -1)
                    return "";
                else
                    return this.Elements[this.selection];
            }
            set
            {
                int size = this.container.Childs.Length;
                for (int i = 0; i < size; i++)
                {
                    if (((Label)this.container.Childs[i]).Text == value)
                    {
                        this.selection = i;
                        this.main_label.Text = this.Selection;
                        return;
                    }
                }
                this.selection = -1;
                this.main_label.Text = "";
            }
        }


        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/


        /// <summary>
        /// Constructeur de base du sélecteur
        /// </summary>
        /// <param name="area">zone du widget</param>
        /// <param name="height">hauteur de déploiement</param>
        /// <param name="parent">widget parent</param>
        public Selector(Rect area, int height, Widget parent)
        {
            this.area = area;
            this.parent = parent;
            this.max_height = height;
            this.container = new Container(new Rect(this.LocalArea.x, this.LocalArea.y + this.area.height, this.area.width, DIM_ELEMENT_HEIGHT));
            MainLayout.Actual.UpperContainer.AddChild(this.container);

            this.container.Parent = MainLayout.Actual.UpperContainer;
            this.container.AllowScrollbar = true;
            this.container.Background = COLOR_CONTAINER_COLOR;

            this.main_label = new Label(new Rect(DIM_PADDING, DIM_PADDING, this.area.width - this.area.height - DIM_PADDING, this.area.height - 2 * DIM_PADDING), "");
            this.main_label.Parent = this;
            this.main_label.Color = COLOR_3;
            this.main_label.Background = COLOR_2;
            this.main_label.Margin = DIM_ELEMENT_MARGIN;
            this.main_label.TextStyle = FontStyle.Bold;
            this.main_label.TextAlign = TextAnchor.MiddleLeft;

            this.container.Active = false;
            this.container.FocusChange += this.OnContainerFocusChange;
        }

        /// <summary>
        /// Appelée lors du clic sur un élément du sélecteur
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="button"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void OnSelect(Widget widget, int button, int x, int y)
        {
            this.Selection = ((Label)widget).Text;
            this.Tuck();
            if (this.SelectItem != null)
                SelectItem(this, this.Selection);
        }

        /// <summary>
        /// Déploie le sélecteur
        /// </summary>
        public void Deploy()
        {
            if (this.container.Active == true) return;
            this.container.Active = true;
            if (this.DeployOrTuck != null)
                this.DeployOrTuck(this, true);
        }

        /// <summary>
        /// Déploie ou replie le sélecteur
        /// </summary>
        /// <param name="state">état du sélecteur</param>
        public void Deploy(bool state)
        {
            if (this.container.Active == state) return;
            this.container.Active = state;
            if (this.DeployOrTuck != null)
                this.DeployOrTuck(this, state);
        }

        /// <summary>
        /// Repli le sélecteur
        /// </summary>
        public void Tuck()
        {
            if (this.container.Active == false) return;
            this.container.Active = false;
            if (this.DeployOrTuck != null)
                this.DeployOrTuck(this, false);
        }


        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/
         

        public override void OnDrawWithGL()
        {
            if (!this.active) return;
            GL.Begin(GL.QUADS);
            GL.Color(COLOR_1);
            Rect rect = this.GlobalArea;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);
            GL.End();
            this.main_label.OnDraw();

            rect.x += rect.width - rect.height;
            rect.width = DIM_TRIANGLE_SIZE * rect.height;
            rect.height = 0.7f * rect.width;
            rect.x += (this.area.height - rect.width) / 2;
            rect.y += (this.area.height - rect.height) / 2;
            GL.Begin(GL.TRIANGLES);
            GL.Color(COLOR_2);
            if (this.container.Active)
            {
                //Flèche "bas"
                GL.Vertex3(rect.xMin, rect.yMin, 0);
                GL.Vertex3((rect.xMin + rect.xMax) / 2, rect.yMax, 0);
                GL.Vertex3(rect.xMax, rect.yMin, 0);
            }
            else
            {
                //Flèche "haut"
                GL.Vertex3(rect.xMin, rect.yMax, 0);
                GL.Vertex3((rect.xMin + rect.xMax) / 2, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMax, 0);
            }
            GL.End();
        }

        public override void OnMouseEvent(int button, bool pressed, int x, int y)
        {
            if (!this.active) return;
            base.OnMouseEvent(button, pressed, x, y);
            if (pressed)
            {
                if (this.GlobalArea.Contains(new Vector2(x, y)))
                {
                    this.Deploy(!this.container.Active);
                    this.container.Focus = true;
                }
                else if (!this.container.GlobalArea.Contains(new Vector2(x, y)))
                    this.Tuck();
            }
        }

        protected void OnContainerFocusChange(Widget w, bool focus)
        {
            if (!focus)
                this.Tuck();
        }

    }
}