/*
Author: Charlie Ringer, found at https://github.com/charlieringer/Monte 
Library released under MIT License
*/

using System;
using System.IO;
using System.Xml;

namespace Monte
{
    //Base class for all the MCTS agents
    public abstract class MCTSMasterAgent : AIAgent
    {
        //Numb simualtions = number to games to simulate before the best most is selected
        protected int numbSimulations;
        //This effects how much we weight the UCT funtion
        protected double exploreWeight;
        //That value to assign a draw in the rollout
        protected double drawScore;
        //How far do we rollout before we call it a draw
        protected int maxRollout;
        //Epsilon
        protected double epsilon;

        /* MACRO ACTION MANAGEMENT */

        //Length of the macro action
        protected int macroActionLength;
        //Current action in the macro action being execut
        protected int remainingMASteps; //m_currentMacroAction
        //Flag to reset algorithm when new action is decided.
        protected bool resetMacro;
        //Last macro action to be executed
        protected int lastMADecision; //m_lastMacroAction;
        //Is first decision ever (this can be removed if we can get this from the game state).
        protected bool firstDecision;


        //Constructors for the Agent
        protected MCTSMasterAgent() { parseXML("Assets/Monte/DefaultSettings.xml"); }
        protected MCTSMasterAgent(string fileName) { parseXML(fileName); }
        protected MCTSMasterAgent(int _numbSimulations, double _exploreWeight, int _maxRollout, double _drawScore)
        {
            numbSimulations = _numbSimulations;
            exploreWeight = _exploreWeight;
            maxRollout = _maxRollout;
            drawScore = _drawScore;
            remainingMASteps = -1;
            lastMADecision = -1;
            firstDecision = true;
            resetMacro = false;
        }
        //Reads the settings files and sets various values
        private void parseXML(string filePath)
        {
            remainingMASteps = -1;
            lastMADecision = -1;
            firstDecision = true;
            resetMacro = false;

            //Try to read it.
            try
            {
                XmlDocument settings = new XmlDocument();
                settings.Load(filePath);

                XmlNode root = settings.DocumentElement;

                XmlNode node = root.SelectSingleNode("descendant::MCTSSettings");

                numbSimulations = int.Parse(node.Attributes.GetNamedItem("NumberOfSimulations").Value);
                exploreWeight = double.Parse(node.Attributes.GetNamedItem("ExploreWeight").Value);
                maxRollout = int.Parse(node.Attributes.GetNamedItem("MaxRollout").Value);
                drawScore = double.Parse(node.Attributes.GetNamedItem("DrawScore").Value);
                epsilon = double.Parse(node.Attributes.GetNamedItem("Epsilon").Value);
                macroActionLength = int.Parse(node.Attributes.GetNamedItem("MacroActionLength").Value);
            }
            //If the file was not found
            catch (FileNotFoundException)
            {
                numbSimulations = 500;
                exploreWeight = 1.45;
                maxRollout = 64;
                drawScore = 0.5;
                epsilon = 1e-6;
                macroActionLength = 15;
                Console.WriteLine("Monte Error: could not find file when constructing MCTS base class. Default settings values used (NumberOfSimulations = 500, ExploreWeight = 1.45, MaxRollout = 64, DrawScore = 0.5). File:" + filePath);
            }
            //Or it was malformed
            catch
            {
                numbSimulations = 500;
                exploreWeight = 1.45;
                maxRollout = 64;
                drawScore = 0.5;
                epsilon = 1e-6;
                macroActionLength = 15;
                Console.WriteLine(
                    "Monte, Error reading settings file when constructing MCTS base class, perhaps it is malformed. Default settings values used (NumberOfSimulations = 500, ExploreWeight = 1.45, MaxRollout = 64, DrawScore = 0.5). File:" + filePath);
            }
        }

    }
}