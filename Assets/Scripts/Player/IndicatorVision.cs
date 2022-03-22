using UnityEngine;

public class IndicatorVision : MonoBehaviour
{
    FieldOfView[] fov;
    CanvasGroup AIindicators;
    [HideInInspector]
    public ViewIndicator agent;

    private void Awake()
    {
        fov = GetComponents<FieldOfView>();
        AIindicators = GameObject.Find("Monster Indicators").GetComponent<CanvasGroup>();
        agent = GameObject.Find("Monster").GetComponentInChildren<ViewIndicator>();
    }

    private void Update()
    {
        if (fov[0].targetDetected || fov[1].targetDetected)
        {
            AIindicators.alpha = 1;
            agent.detected = true;
        }
        else
        {
            AIindicators.alpha = 0;
            agent.detected = false;
        }
    }
}
