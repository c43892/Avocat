using Avocat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

static class BattleTestUIOnEvents
{
    public static void SetupEventHandler(this CombatTestDriver CombatTestDriver, BattleRoomClient room)
    {
        var bt = CombatTestDriver.BattleStage.Battle as BattlePVE;

        room.Battle.OnPlayerPrepared.Add((int player) =>
        {
            if (room.Battle.AllPrepared)
            {
                CombatTestDriver.PreparingUI.SetActive(false);
            }
        });

        room.Battle.OnBattleEnded.Add((winner) =>
        {
            CombatTestDriver.GameOverUI.SetActive(true);
            CombatTestDriver.GameOverUI.transform.Find("Title").GetComponent<Text>().text = winner == room.PlayerMe ? "Win" : "Lose";
        });
    }
}
