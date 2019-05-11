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
        room.Battle.OnWarriorPositionExchanged += (int fromX, int fromY, int toX, int toY) =>
        {
            var avFrom = BattleStage.Avatars[fromX, fromY];
            var avTo = BattleStage.Avatars[toX, toY];

            aniPlayer.Op(() =>
            {
                BattleStage.SetAvatarPosition(avFrom, toX, toY);
                BattleStage.SetAvatarPosition(avTo, fromX, fromY);
            });
        };

        // 回合开始
        room.Battle.OnNextRoundStarted += (int player) =>
        {
            if (player != room.PlayerMe)
                return;

            aniPlayer.Op(() =>
                BattleStage.ForeachAvatar((x, y, avatar) => avatar.RefreshAttrs())
            );
        };

        // 角色攻击
        room.Battle.OnWarriorAttack += (Warrior attacker, Warrior target, Skill skill, List<string> flags) =>
        {
            var avatar = BattleStage.GetAvatarByWarrior(attacker);
            var targetAvatar = BattleStage.GetAvatarByWarrior(target);
            aniPlayer.MakeAttacking2(avatar, targetAvatar).OnEnded(
                () =>
                {
                    avatar.RefreshAttrs();
                    targetAvatar.RefreshAttrs();
                }
            );
        };

        // 角色移动
        room.Battle.OnWarriorMovingOnPath += (Warrior warrior, int x, int y, List<int> path) =>
        {
            var tx = path[path.Count - 2];
            var ty = path[path.Count - 1];

            MapAvatar avatar = BattleStage.GetAvatarByWarrior(warrior);
            Debug.Assert(avatar != null && avatar.Warrior == warrior, "the avatar is not just on the start position");

            aniPlayer.MakeMovingOnPath(
                avatar.transform, 5,
                FC.ToArray(path, (i, p, doSkip) => i % 2 == 0 ? p + avatar.CenterOffset.x : p + avatar.CenterOffset.y)
            ).OnEnded(() =>
            {
                BattleStage.SetAvatarPosition(null, x, y);
                BattleStage.SetAvatarPosition(avatar, tx, ty);
                avatar.RefreshAttrs();
            });
        };

        // 回合开始
        room.Battle.OnNextRoundStarted += (int player) =>
        {
            aniPlayer.Op(() =>
            {
                BattleStage.ForeachAvatar((x, y, avatar) =>
                {
                    if (avatar.Warrior.Team == player)
                        avatar.RefreshAttrs();
                });
            });
        };

        // 回合结束
        room.Battle.OnActionDone += (int player) =>
        {
            aniPlayer.Op(() =>
            {
                BattleStage.ForeachAvatar((x, y, avatar) =>
                {
                    if (avatar.Warrior.Team == player)
                        avatar.RefreshAttrs();

                    var op = BattleStage.CurrentOpLayer as InBattleOps;
                    op.RemoveShowAttackRange();
                    op.ClearPath();
                });
            });
        };

        // 角色死亡
        room.Battle.OnWarriorDying += (Warrior warrior) =>
        {
            var avatar = BattleStage.GetAvatarByWarrior(warrior);
            aniPlayer.MakeDying(avatar).OnEnded(
                () =>
                {
                    BattleStage.SetAvatarPosition(null, avatar.X, avatar.Y);
                    avatar.transform.SetParent(null);
                    BattleStage.Destroy(avatar.gameObject);
                }
            );
        };

        // 使用道具
        (room.Battle as BattlePVE).OnUseItem2 += (UsableItem item, Warrior target) =>
        {
            var mapItem = BattleStage.GetMapItemByItem(item);
            var avatar = BattleStage.GetAvatarByWarrior(target);
            aniPlayer.MakeAttacking1(mapItem, avatar).OnEnded(
                () =>
                {
                    avatar.RefreshAttrs();
                    BattleStage.Items[mapItem.X, mapItem.Y] = null;
                    mapItem.transform.SetParent(null);
                    BattleStage.Destroy(mapItem.gameObject);
                }
            );
        };

        bt.OnPlayerPrepared += (int player) =>
        {
            aniPlayer.Op(() =>
            {
                if (room.Battle.AllPrepared)
                    BattleStage.StartFighting();
            });
        };

        bt.OnAddHP += (warrior, dhp) =>
        {
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs());
        };

        bt.OnAddATK += (warrior, dATK) =>
        {
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs());
        };

        bt.OnAddES += (warrior, des) =>
        {
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs());
        };

        bt.OnTransfrom += (warrior, state) =>
        {
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs());
        };

        bt.OnAddWarrior += (x, y, warrior) =>
        {
            aniPlayer.Op(() => BattleStage.CreateWarriorAvatar(x, y, warrior));
        };

        bt.OnTimeBackTriggered += (BattleReplay replay) =>
        {
            aniPlayer.Op(() => BattleStage.OnTimeBackTriggered(replay));
        };
    }
}
