using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WarBotEngine.Editeur;

[assembly: AssemblyVersionAttribute("1.0")]
namespace Assets.Scripts.Editeur.Interpreter
{
    class TestInterpreter : MonoBehaviour
    {
        void Start()
        {
            /*A REECRIRE*/
            string teamName = "DoudouLaMalice";
            string unitName = "explorer";
            XMLInterpreter interpreter = new XMLInterpreter();
            
            List<Instruction> behavior = new List<Instruction>();
            /** Ecriture d'un fichier xml **/
/*            Condition bag = new IsBagEmpty();
            Action idle = new Idle();
            Action shoot = new Shoot();
            Action move = new Move();
            When when = new When();

            when.addCondition(bag);
            when.addAction(move);
            when.addAction(shoot);

            behavior.Add(when);
            behavior.Add(idle);

*/            interpreter.behaviorToXml(teamName, Constants.teamsDirectory, unitName, behavior);
            Debug.Log("fini");
            /*
            behavior = interpreter.xmlToUnitBehavior(teamName, Constants.teamsDirectory, unitName);
            Debug.Log(behavior.Count);
            foreach(Instruction i in behavior)
            {
                Debug.Log(i.xmlStructure().OuterXml);
            }
           /* */
        }

        void Update()
        {

        }
    }
}
