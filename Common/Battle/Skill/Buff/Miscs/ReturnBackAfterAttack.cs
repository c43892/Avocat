using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 攻击后返回原位
    /// </summary>
    public class ReturnBackAfterAttack : BuffWithOwner
    {
        public override string ID => "ReturnBackAfterAttack";
        public ReturnBackAfterAttack(Warrior owner) : base(owner) { }

        void OnAfterMoveOnPathAndAttack(Warrior attacker, Warrior target, int fx, int fy, List<int> pathList)
        {
            if (attacker != Owner)
                return;

            var reversedPathList = new List<int>();
            for (var i = 0; i < pathList.Count; i += 2)
            {
                reversedPathList.Add(pathList[pathList.Count - i - 2]);
                reversedPathList.Add(pathList[pathList.Count - i - 1]);
            }

            reversedPathList.Add(fx);
            reversedPathList.Add(fy);

            attacker.MovingPath.Clear();
            attacker.MovingPath.AddRange(reversedPathList);
            Battle.MoveOnPath(attacker, true);
        }

        public override void OnAttached()
        {
            Battle.AfterMoveOnPathAndAttack += OnAfterMoveOnPathAndAttack;
            base.OnAttached();
        }

        public override void OnDetached()
        {
            Battle.AfterMoveOnPathAndAttack -= OnAfterMoveOnPathAndAttack;
            base.OnDetached();
        }
    }
}
