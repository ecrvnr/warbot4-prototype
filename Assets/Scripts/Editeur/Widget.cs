using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
    /// Class abstraite dont les widgets héritent
    /// </summary>
    public abstract class Widget
    {


        /***********************
         ****** DELEGATES ******
         ***********************/


        /// <summary>
        /// Delegate permettant de gérer les évènements simples des widgets
        /// </summary>
        /// <param name="args">paramètre quelconque de l'évènement</param>
        public delegate void EventDelegate(Widget widget, object args);

        /// <summary>
        /// Delegate permettant de gérer les évènements de clic de souris sur des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="button">bouton de la souris</param>
        /// <param name="x">coordonnée en x du curseur</param>
        /// <param name="y">coordonnée en y du curseur</param>
        public delegate void EventMouseDelegate(Widget widget, int button, int x, int y);

        /// <summary>
        /// Delegate permettant de gérer les évènements de mouvement de souris sur des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="x">coordonnée en x du curseur</param>
        /// <param name="y">coordonnée en y du curseur</param>
        public delegate void EventMotionDelegate(Widget widget, int x, int y);

        /// <summary>
        /// Delegate permettant de gérer les évènements de scrolling de souris sur des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="delta">niveau de scrolling</param>
        public delegate void EventScrollDelegate(Widget widget, int delta);

        /// <summary>
        /// Delegate permettant de gérer les évènements de changement de focus de widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="focus">indique si le widget a le focus ou non</param>
        public delegate void EventFocusChangeDelegate(Widget widget, bool focus);

        /// <summary>
        /// Delegate permettant de gérer les évènements de pression de clavier lorsque des widgets on le focus
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="button">touche du clavier</param>
        public delegate void EventKeyDelegate(Widget widget, KeyCode key);

        /// <summary>
        /// Delegate permettant de gérer les évènements de mouvement curseur dans des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        public delegate void EventEnterCursorDelegate(Widget widget);

        /// <summary>
        /// Delegate permettant de gérer les évènements de mouvement curseur hors des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        public delegate void EventExitCursorDelegate(Widget widget);

        /// <summary>
        /// Delegate permettant de gérer les évènements de déplacement des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="x">position en x du widget</param>
        /// <param name="y">position en y widget</param>
        public delegate void EventMoveDelegate(Widget widget, int x, int y);

        /// <summary>
        /// Delegate permettant de gérer les évènements de redimensionnement des widgets
        /// </summary>
        /// <param name="widget">widget concerné par l'évènement</param>
        /// <param name="width">largeur du widget</param>
        /// <param name="height">hauteur du widget</param>
        public delegate void EventResizeDelegate(Widget widget, int width, int height);


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
        /// Incrémenteur des identifiants de widgets
        /// </summary>
        private static uint _id_increment = 0;


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
        /// Identifiant du widget
        /// </summary>
        protected uint id = Widget._id_increment++;

        /// <summary>
        /// Parent du widget
        /// </summary>
        protected Widget parent = null;

        /// <summary>
        /// Zone locale du widget
        /// </summary>
        protected Rect area = new Rect(new Vector2(0, 0), new Vector2(0, 0));

        /// <summary>
        /// Liste des widgets enfants
        /// </summary>
        protected List<Widget> childs = new List<Widget>();

        /// <summary>
        /// Indique si le widget est actif
        /// </summary>
        protected bool active = true;
        
        /// <summary>
        /// Indique si le widget a le focus
        /// </summary>
        protected bool focus = false;


        /******* ATTRIBUTS POUR GERER LES EVENEMENTS SPECIFIQUES ******/

        /// <summary>
        /// Indique la dernière position de la souris
        /// </summary>
        private Vector2 evnt_mouse_position = new Vector2(0xffffff, 0xffffff);


        /************************************
         ****** EVENEMENTS SPECIFIQUES ******
         ************************************/


        /// <summary>
        /// Appelé lors d'un clic sur le widget
        /// </summary>
        public event EventMouseDelegate Clic = null;

        /// <summary>
        /// Appelé lorsque le curseur se déplace sur le widget
        /// </summary>
        public event EventMotionDelegate Motion = null;

        /// <summary>
        /// Appelé lors d'un scroll sur le widget
        /// </summary>
        public event EventScrollDelegate Scroll = null;

        /// <summary>
        /// Appelé lors de la pression d'une touche alors que le widget a le focus
        /// </summary>
        public event EventKeyDelegate KeyPress = null;

        /// <summary>
        /// Appelé lorsque le curseur entre dans le widget
        /// </summary>
        public event EventEnterCursorDelegate CursorEnter = null;

        /// <summary>
        /// Appelé lorsque le curseur sort du widget
        /// </summary>
        public event EventExitCursorDelegate CursorExit = null;

        /// <summary>
        /// Appelé lorsque le focus du widget change
        /// </summary>
        public event EventFocusChangeDelegate FocusChange = null;

        /// <summary>
        /// Appelé lorsque le widget est déplacé
        /// </summary>
        public event EventMoveDelegate Move = null;

        /// <summary>
        /// Appelé lorsque le widget est redimensionné
        /// </summary>
        public event EventResizeDelegate Resize = null;


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
        /// Identifiant du widget
        /// </summary>
        public uint ID { get { return this.id; } }

        /// <summary>
        /// Parent du widget s'il en a un et null sinon
        /// </summary>
        public Widget Parent { get { return parent; } set { parent = value; } }

        /// <summary>
        /// Coordonnées locales et dimensions du widget
        /// </summary>
        public Rect LocalArea {
            get
            {
                return area;
            }
            set
            {
                bool pos_change = (this.Move != null && (value.x != area.x || value.y != area.y));
                bool size_change = (this.Resize != null && (value.width != area.width || value.height != area.height));
                area = value;
                if (pos_change)
                    this.Move(this, (int)area.x, (int)area.y);
                if (size_change)
                    this.Resize(this, (int)area.width, (int)area.height);
            }
        }

        /// <summary>
        /// Coordonnées globales et dimensions du widget
        /// </summary>
        public Rect GlobalArea {
            get
            {
                Vector2 pos = GlobalPosition;
                return new Rect(pos.x, pos.y, area.width, area.height);
            }
            set
            {
                bool size_change = (this.Resize != null && (value.width != area.width || value.height != area.height));
                GlobalPosition = new Vector2(value.x, value.y);
                area.width = value.width;
                area.height = value.height;
                if (size_change)
                    this.Resize(this, (int)area.width, (int)area.height);
            }
        }

        /// <summary>
        /// Coordonnées locales du widget
        /// </summary>
        public Vector2 LocalPosition
        {
            get
            {
                return new Vector2(area.x, area.y);
            }
            set
            {
                bool change = (this.Move != null && (area.x != value.x || area.y != value.y));
                area.x = value.x;
                area.y = value.y;
                if (change)
                    this.Move(this, (int)area.x, (int)area.y);
            }
        }

        /// <summary>
        /// Coordonnées globales du widget
        /// </summary>
        public Vector2 GlobalPosition
        {
            get
            {
                if (parent != null)
                    return parent.GlobalPosition + new Vector2(area.x, area.y);
                else
                    return new Vector2(area.x, area.y);
            }
            set
            {
                Vector2 gpos = value - GlobalPosition;
                bool change = (this.Move != null && (gpos.x != 0 || gpos.y != 0));
                area.x += gpos.x;
                area.y += gpos.y;
                if (change)
                    this.Move(this, (int)area.x, (int)area.y);
            }
        }

        /// <summary>
        /// Widgets contenus dans le widget actuel
        /// </summary>
        public Widget[] Childs { get { return childs.ToArray(); } }

        /// <summary>
        /// Détermine si le widget est actif ou non
        /// </summary>
        public bool Active { get { return active; } set { active = value; } }

        public bool Focus {
            get
            {
                return focus;
            }
            set
            {
                bool change = (focus != value);
                focus = value;
                if (change && FocusChange != null)
                    FocusChange(this, focus);
            }
        }


        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        /// <summary>
        /// Appelée lors de la suppression du widget
        /// </summary>
        public virtual void OnDestroy()
        {
            foreach (Widget w in childs)
            {
                w.OnDestroy();
            }
        }

        /// <summary>
        /// Appelée lors du rendu
        /// </summary>
        public virtual void OnDraw()
        {
            if (!this.Active) return;
            this.OnDrawWithGL();
            MainLayout.Actual.PopGL();
            this.OnDrawWithoutGL();
            MainLayout.Actual.PushGL();
            foreach (Widget w in childs)
            {
                w.OnDraw();
            }
        }

        /// <summary>
        /// Appelée lors du rendu de la scene avec les fonctions OpenGL
        /// </summary>
        public virtual void OnDrawWithGL() { }

        /// <summary>
        /// Appelée après le rendu de la scene avec les fonctions OpenGL
        /// </summary>
        public virtual void OnDrawWithoutGL() { }

        /// <summary>
        /// Appelée lors de la mise à jour des composants
        /// </summary>
        public virtual void OnUpdate()
        {
            if (!this.Active) return;
            foreach (Widget w in childs)
            {
                w.OnUpdate();
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement clavier
        /// </summary>
        /// <param name="keycode">touche associée à l'évènement</param>
        /// <param name="state">indique si la touche est appuyée ou non</param>
        public virtual void OnKeyEvent(KeyCode keycode, bool state)
        {
            if (!this.Active) return;
            if (state && this.KeyPress != null && this.Focus)
                this.KeyPress(this, keycode);

            foreach (Widget w in childs)
            {
                w.OnKeyEvent(keycode, state);
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement souris de bouton
        /// </summary>
        /// <param name="button">bouton associé à l'évènement</param>
        /// <param name="pressed">indique si le bouton est appuyé ou non</param>
        /// <param name="x">coordonnée en x de la souris</param>
        /// <param name="y">coordonnée en y de la souris</param>
        public virtual void OnMouseEvent(int button, bool pressed, int x, int y)
        {
            if (!this.Active) return;
            if (pressed)
            {
                if (this.GlobalArea.Contains(new Vector2(x, y)))
                {
                    if (this.Clic != null)
                        this.Clic(this, button, x, y);
                    this.Focus = true;
                }
                else
                {
                    this.Focus = false;
                }
            }

            for (int i = 0; i < this.childs.Count; i++)
            {
                this.childs[i].OnMouseEvent(button, pressed, x, y);
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement souris de mouvement
        /// </summary>
        /// <param name="x">coordonnée en x de la souris</param>
        /// <param name="y">coordonnée en y de la souris</param>
        public virtual void OnMotionEvent(int x, int y)
        {
            if (!this.Active) return;
            Rect zone = this.GlobalArea;
            if (zone.Contains(new Vector2(x, y)))
            {
                if (this.CursorEnter != null && !zone.Contains(this.evnt_mouse_position))
                    this.CursorEnter(this);
                if (this.Motion != null)
                    this.Motion(this, x, y);
            }
            else
            {
                if (this.CursorExit != null && zone.Contains(this.evnt_mouse_position))
                    this.CursorExit(this);
            }
            this.evnt_mouse_position = new Vector2(x, y);

            foreach (Widget w in childs)
            {
                w.OnMotionEvent(x, y);
            }
        }

        /// <summary>
        /// Appelée lors d'un évènement souris de scrolling
        /// </summary>
        /// <param name="delta">valeur de scrolling</param>
        public virtual void OnScrollEvent(int delta)
        {
            if (!this.Active) return;
            if (this.Scroll != null && this.GlobalArea.Contains(this.evnt_mouse_position))
                this.Scroll(this, delta);

            foreach (Widget w in childs)
            {
                w.OnScrollEvent(delta);
            }
        }


        /*********************************
         ****** METHODES DE GESTION ******
         *********************************/


        /// <summary>
        /// Ajoute un widget enfant au widget actuel
        /// </summary>
        /// <param name="widget">widget à ajouter</param>
        public virtual void AddChild(Widget widget)
        {
            if (!childs.Contains(widget))
            {
                widget.Parent = this;
                childs.Add(widget);
            }
        }

        /// <summary>
        /// Supprime un widget enfant du widget actuel
        /// </summary>
        /// <param name="widget">widget à supprimer</param>
        public virtual void RemoveChild(Widget widget)
        {
            if (childs.Contains(widget))
            {
                widget.Parent = null;
                childs.Remove(widget);
            }
        }

        /// <summary>
        /// Supprime tous les widgets enfants du widget actuel
        /// </summary>
        public virtual void RemoveAllChilds()
        {
            childs.Clear();
        }

        /// <summary>
        /// Détermine si un événement peut provoquer une intéraction avec le widget
        /// </summary>
        /// <returns></returns>
        public virtual bool CanInteract()
        {
            foreach (Widget w in childs)
            {
                if (w.Active)
                    return true;
            }
            return false;
        }

    }

}
