using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    protected override void TurnSetUp()
    {
        base.TurnSetUp();
    }
    protected override void PiceCange(BaseEntity newPice)
    {
        base.PiceCange(newPice);
    }
    public override void StartTurn()
    {
        base.StartTurn();
    }
    protected override IEnumerator TakeTurn()
    {
        return base.TakeTurn();
    }
    public override void Death()
    {
        base.Death();
    }
}
