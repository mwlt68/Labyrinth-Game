using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Timer
{
    public float TickTime;

    public Tick Tick;

    private float sumDelta = 0;

    private bool start = false;

    /// <summary>
    /// 1.0f = 1 second
    /// </summary>
    /// <param name="tickTime"></param>
    /// <param name="tick"></param>
    public Timer(float tickTime, Tick tick)
    {
        TickTime = tickTime;
        Tick = tick;
    }

    /// <summary>
    /// Start Timer
    /// </summary>
    public void Start()
    {
        Reset();
        start = true;
    }

    /// <summary>
    /// Stop Timer
    /// </summary>
    public void Stop()
    {
        start = false;
        Reset();
    }

    /// <summary>
    /// Reset Timer
    /// </summary>
    public void Reset()
    {
        sumDelta = 0;
    }

    /// <summary>
    /// runs in every frame update
    /// </summary>
    public void Update()
    {
        if (start)
        {
            sumDelta += Time.deltaTime;

            if (sumDelta >= TickTime)
            {
                Reset();
                Tick();
            }
        }
    }
}

public delegate void Tick();