using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWords : MonoBehaviour
{
    // Update is called once per frame
    public GraphicsDM myText;
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";


    private void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    // Update is called once per frame
    void Update()
    {
        myText.CreateWord();
        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            myText.word = string.Format(display, m_CurrentFps);
            
        }
    }
}
