using UnityEngine;

public class Teststar : MonoBehaviour
{
    private void Start()
    {
        WinCondition.King?.Invoke(false);
        WinCondition.MinTurns(5);
        WinCondition.KillEnemy?.Invoke(0);
        WinCondition.Win?.Invoke();
    }


}
