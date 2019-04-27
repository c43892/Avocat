using Avocat;
using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class BattleStageOnEvents
{
    public static void SetupEventHandler(this BattleStage BattleStage, BattleRoomClient room)
    {
        var bt = BattleStage.Battle as BattlePVE;
        var aniPlayer = BattleStage.GetComponent<MapAniPlayer>();

        // 角色位置变化
        room.Battle.OnWarriorPositionExchanged.Add((int fromX, int fromY, int toX, int toY) =>
        {
            var avFrom = BattleStage.Avatars[fromX, fromY];
            var avTo = BattleStage.Avatars[toX, toY];

            BattleStage.SetAvatarPosition(avFrom, toX, toY);
            BattleStage.SetAvatarPosition(avTo, fromX, fromY);
        });

        // 回合开始
        room.Battle.OnNextRoundStarted.Add((int player) =>
        {
            if (player != room.PlayerMe)
                return;

            BattleStage.ForeachAvatar((x, y, avatar) => avatar.RefreshAttrs());
        });

        // 角色攻击
        IEnumerator OnAttacking(Warrior attacker, Warrior target, Skill skill, List<string> flags)
        {
            var avatar = BattleStage.GetAvatarByWarrior(attacker);
            var targetAvatar = BattleStage.GetAvatarByWarrior(target);
            yield return aniPlayer.MakeAttacking2(avatar, targetAvatar);
            avatar.RefreshAttrs();
            targetAvatar.RefreshAttrs();
        }
        room.Battle.OnWarriorAttack.Add(OnAttacking);

        // 角色移动
        IEnumerator OnWarriorMovingOnPath(Warrior warrior, int x, int y, List<int> path)
        {
            var avatar = BattleStage.Avatars[x, y];
            Debug.Assert(avatar != null && avatar.Warrior == warrior, "moving target conflicted");

            var tx = path[path.Count - 2];
            var ty = path[path.Count - 1];

            yield return aniPlayer.MakeMovingOnPath(avatar.transform, 5, FC.ToArray(path, (i, p, doSkip) => i % 2 == 0 ? p + avatar.CenterOffset.x : p + avatar.CenterOffset.y));

            BattleStage.SetAvatarPosition(BattleStage.Avatars[tx, ty], x, y);
            BattleStage.SetAvatarPosition(avatar, tx, ty);

            avatar.RefreshAttrs();
        }
        room.Battle.OnWarriorMovingOnPath.Add(OnWarriorMovingOnPath);

        // 回合开始
        room.Battle.OnNextRoundStarted.Add((int player) =>
        {
            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                if (avatar.Warrior.Team == player)
                    avatar.RefreshAttrs();
            });
        });

        // 回合结束
        room.Battle.OnActionDone.Add((int player) =>
        {
            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                if (avatar.Warrior.Team == player)
                    avatar.RefreshAttrs();
            });
        });

        // 角色死亡
        IEnumerator OnWarriorDying(Warrior warrior)
        {
            var avatar = BattleStage.GetAvatarByWarrior(warrior);
            yield return aniPlayer.MakeDying(avatar);
            BattleStage.Avatars[avatar.X, avatar.Y] = null;
            avatar.transform.SetParent(null);
            BattleStage.Destroy(avatar.gameObject);
        }
        room.Battle.OnWarriorDying.Add(OnWarriorDying);

        // 使用道具
        IEnumerator OnItemUsed2(UsableItem item, Warrior target)
        {
            var mapItem = BattleStage.GetMapItemByItem(item);
            var avatar = BattleStage.GetAvatarByWarrior(target);
            yield return aniPlayer.MakeAttacking1(mapItem, avatar);
            avatar.RefreshAttrs();
            BattleStage.Items[mapItem.X, mapItem.Y] = null;
            mapItem.transform.SetParent(null);
            BattleStage.Destroy(mapItem.gameObject);
        }
        (room.Battle as BattlePVE).OnUseItem2.Add(OnItemUsed2);

        bt.OnPlayerPrepared.Add((int player) =>
        {
            if (room.Battle.AllPrepared)
                BattleStage.StartFighting();
        });

        bt.OnAddHP.Add((warrior, dhp) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnAddATK.Add((warrior, dATK) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnAddES.Add((warrior, des) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnTransfrom.Add((warrior, state) =>
        {
            BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs();
        });

        bt.OnAddWarrior.Add((x, y, warrior) =>
        {
            BattleStage.CreateWarriorAvatar(x, y, warrior);
        });
    }
}
