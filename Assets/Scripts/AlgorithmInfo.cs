/*
Written by: Charlie Ringer, found at https://github.com/charlieringer/MonteExamples 
Library released under MIT License
Modified by: Cristiana Pacheco
*/

using UnityEngine;

public class AlgorithmInfo : MonoBehaviour
{
    //Defaults for the player indx and selected AI agent
    static public int playerIndex = 0; //TODO: Is this needed or used?
    static public int selectedAgent = 2;

    //File paths for the model and settings.
    static public string[] modelFiles = { "Assets/Monte/Model.model", "Assets/Monte/Pareto_Model.model" };
    static public string[] settingsFiles = { "Assets/Monte/DefaultSettings.xml", "Assets/Monte/ParetoSettings.xml" };

    //This just stores the player index so it can be passed from the front end to the game
    public void setPlayerIndex(int indx) { playerIndex = indx; }

    //Set which agent the game is using
    public void setAgentIndex(int indx) { selectedAgent = indx; }

    //Set the model file path for TicTakToe
    public void setModelPath(int i, string path) { modelFiles[i] = path; }

    //Set the settings path for TicTacToe
    public void setSettingsPath(int i, string path) { settingsFiles[i] = path; }
}

