using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.Managers;

namespace WarBotEngine.WarBots
{

    /// <summary>
    /// Messages controller
    /// </summary>
    public class MessageController : MonoBehaviour
    {


        ////// CLASS DEFINITION //////


        /// <summary>
        /// Message
        /// </summary>
        public class Message
        {


            ////// STATIC ATTRIBUTES //////


            /// <summary>
            /// Separate message multi-contents for the single getter
            /// </summary>
            public static readonly string SEPARATOR = " ";


            ////// ATTRIBUTES //////


            /// <summary>
            /// The sender of the message
            /// </summary>
            protected GameObject sender;

            /// <summary>
            /// Array of string that represent the content of the message
            /// </summary>
            protected string[] msg;


            ////// ACCESSORS //////


            /// <summary>
            /// Public getter for the sender
            /// </summary>
            public GameObject Sender { get { return sender; } }

            /// <summary>
            /// Public getter for the content of the message
            /// </summary>
            public string[] Content { get { return msg; } }

            /// <summary>
            /// Public getter for the content of the message in a single string
            /// </summary>
            public string SingleContent
            {
                get
                {
                    string res = msg[0];
                    for (uint i = 1; i < msg.Length; i++)
                        res += (SEPARATOR + msg[i]);
                    return res;
                }
            }


            ////// CONSTRUCTOR //////


            /// <summary>
            /// Basic constructor of a message
            /// </summary>
            /// <param name="sender">the sender of the message</param>
            /// <param name="msg">the content of the message</param>
            public Message(GameObject sender, string msg)
            {
                this.sender = sender;
                this.msg = new string[1];
                this.msg[0] = msg;
            }

            /// <summary>
            /// Constructor of a message with multiple content
            /// </summary>
            /// <param name="sender">the sender of the message</param>
            /// <param name="msg">the content of the message</param>
            public Message(GameObject sender, string[] msg)
            {
                this.sender = sender;
                this.msg = (string[])msg.Clone();
            }

        }


        ////// STATIC ATTRIBUTES //////

            
        /// <summary>
        /// Matériel pour dessiner lignes de message
        /// </summary>
        private static Material lineMaterial = null;


        ////// ATTRIBUTES //////


        /// <summary>
        /// List of messages
        /// </summary>
        protected List<Message> messages = new List<Message>();


        ////// ACCESSORS //////


        /// <summary>
        /// Public getter for messages
        /// </summary>
        public Message[] Messages { get { return messages.ToArray(); } }


        ////// UNITY METHODS //////


        // Use this for initialization
        void Start()
        {
            MessageController.CreateLineMaterial();
        }

        // Update is called once per frame
        void Update()
        {
            
        }


        ////// SPECIFIC METHODS //////


        /// <summary>
        /// Add a message on the list
        /// </summary>
        /// <param name="msg">the message which must to add</param>
        public void AddMessage(Message msg)
        {
            messages.Add(msg);
        }

        /// <summary>
        /// Send a message to an agent
        /// </summary>
        /// <param name="msg">the message which must to send</param>
        /// <param name="agent">the agent to send</param>
        public void SendMessage(Message msg, GameObject agent)
        {
            agent.GetComponent<MessageController>().AddMessage(msg);

            if (WarBotEngine.UI.Toggle_MessageViewer.ToggleMessageViewer && this.GetComponent<WarBotController>().TeamManager != null)
            {
                DrawLine(this.transform.position, agent.transform.position, this.GetComponent<WarBotController>().TeamManager.color, 0.05f);
            }
        }

        /// <summary>
        /// Clear messages list
        /// </summary>
        public void Clear()
        {
            messages.Clear();
        }

        /// <summary>
        /// Draw a line on the scene
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="end">end position</param>
        /// <param name="color">color of the line</param>
        /// <param name="duration">duration (seconds)</param>
        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = MessageController.lineMaterial;
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = 1f;
            lr.endWidth = 1f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(myLine, duration);
        }


        ////// STATIC METHODS //////


        /// <summary>
        /// Create a material
        /// </summary>
        private static void CreateLineMaterial()
        {
            if (lineMaterial == null)
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

    }

}
