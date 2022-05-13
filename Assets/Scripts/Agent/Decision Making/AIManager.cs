using Monte;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    //Which AI is being used: Movement.cs; MCTS pack or ParetoMCTS 
    public enum AIPick { ORIGINAL, MONTE, PARETO };
    public AIPick aIPick;

    //Which type of agent needs to be enabled
    [HideInInspector]
    public AIAgent agent;
    [HideInInspector]
    public Movement movement;
    [HideInInspector]
    public MonsterAI monsterAI;

    //Configuration needed. 0 for default, 1 for Pareto
    [HideInInspector]
    public int configIndx = 0;

    void Awake()
    {
        if (aIPick == AIPick.ORIGINAL)
        {
            //activate Movement.cs
            movement = GetComponent<Movement>();
            movement.enabled = true;
        }
        else
        {
            //activate 
            monsterAI = GetComponent<MonsterAI>();
            monsterAI.enabled = true;

            //change configurations if Pareto is being used
            if (aIPick == AIPick.PARETO)
            {
                configIndx = 1;
            }

            //Model and settings
            Model model = new Model(AlgorithmInfo.modelFiles[configIndx]);
            string settings = AlgorithmInfo.settingsFiles[configIndx];

            //Initialise the right agent with the model and settings.
            if (AlgorithmInfo.selectedAgent == 0) agent = new RandomAgent();
            else if (AlgorithmInfo.selectedAgent == 1) agent = new ModelBasedAgent(model);
            else if (AlgorithmInfo.selectedAgent == 2) agent = new MCTSSimpleAgent(settings);
            else if (AlgorithmInfo.selectedAgent == 3) agent = new MCTSWithPruning(model, settings);
            else if (AlgorithmInfo.selectedAgent == 4) agent = new MCTSWithSoftPruning(model, settings);
            else if (AlgorithmInfo.selectedAgent == 5) agent = new MCTSWithLearning(model, settings);
            else agent = new MCTSSimpleAgent(settings);
            //TODO: Add Pareto in the previous clauses


            //Once the game has been made set the rest of the game up
            monsterAI.ai = agent;
        }

    }
}
