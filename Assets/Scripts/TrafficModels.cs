using System;
using System.Collections.Generic;


[Serializable]
public class TrafficResponse
{
    public TrafficStatus current_status;
    public List<PredictedEntry> predicted_status;
}

[Serializable]
public class TrafficStatus
{
    public float vehicleDensity;
    public float averageSpeed;
    public string weather;
}


[Serializable]
public class PredictedEntry
{
    public int estimated_time;

    public TrafficStatus predictions;
}
