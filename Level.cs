using System.Collections.Generic;

public enum ExitCondition
{
    KillAllEnemies,
    FindKeyAndExit,
    ReachExit
    //maybe add another tasks
}

public class LevelData
{
    public int[,] Map;
    public ExitCondition Condition;
    public float StartX;
    public float StartY;
    public List<Enemy> Enemies;
    public List<Pickup> Pickups;

    public LevelData(int[,] map, ExitCondition condition, float startX, float startY)
    {
        Map = map;
        Condition = condition;
        StartX = startX;
        StartY = startY;
        Enemies = new List<Enemy>();
        Pickups = new List<Pickup>();   
    }
}