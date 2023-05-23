using UnityEngine;

public class PlayerPointsController
{

    private int _totalPoints;

    public PlayerPointsController()
    {
        _totalPoints = 0;
    }

    public void AddPoints(int amount)
    {
        _totalPoints += amount;
    }

    public void RemovePoints(int amount)
    {
        _totalPoints -= amount;
    }
}
