using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Swift;
using Avocat;

/// <summary>
/// 战斗卡牌 UI
/// </summary>
public class BattleCardUI : MonoBehaviour
{
    public BattleCard Card
    {
        get
        {
            return card;
        }
        set
        {
            card = value;
        }
    } BattleCard card;
}
