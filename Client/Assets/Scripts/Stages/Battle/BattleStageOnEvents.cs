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

            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                var warrior = avatar.Warrior;
                var hp = warrior.HP;
                var atk = warrior.BasicAttackValue;
                var es = warrior.ES;
                var actionDone = warrior.ActionDone;
                aniPlayer.Op(() => avatar.RefreshAttrs2(hp, atk, es, actionDone));
            });
        };

        // 角色攻击
        room.Battle.OnWarriorAttack += (Warrior attacker, Warrior target, Skill skill, List<string> flags) =>
        {
            var avatar = BattleStage.GetAvatarByWarrior(attacker);
            var targetAvatar = BattleStage.GetAvatarByWarrior(target);

            var hpAtr = attacker.HP;
            var atkAtr = attacker.BasicAttackValue;
            var esAtr = attacker.ES;
            var actionDoneAtr = attacker.ActionDone;

            var hpTar = target.HP;
            var atkTar = target.BasicAttackValue;
            var esTar = target.ES;
            var actionDoneTar = target.ActionDone;

            aniPlayer.MakeAttacking2(avatar, targetAvatar).OnEnded(
                () =>
                {
                    avatar.RefreshAttrs2(hpAtr, atkAtr, esAtr, actionDoneAtr);
                    targetAvatar.RefreshAttrs2(hpTar, atkTar, esTar, actionDoneTar);
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

            var hp = warrior.HP;
            var atk = warrior.BasicAttackValue;
            var es = warrior.ES;
            var actionDone = warrior.ActionDone;
            aniPlayer.MakeMovingOnPath(
                avatar.transform, 5,
                FC.ToArray(path, (i, p, doSkip) => i % 2 == 0 ? p + avatar.CenterOffset.x : p + avatar.CenterOffset.y)
            ).OnEnded(() =>
            {
                BattleStage.SetAvatarPosition(null, x, y);
                BattleStage.SetAvatarPosition(avatar, tx, ty);
                // avatar.RefreshAttrs2(hp, atk, es, actionDone);
            });
        };

        // 回合开始
        room.Battle.OnNextRoundStarted += (int team) =>
        {
            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                if (avatar.Warrior.Team != team)
                    return;

                var warrior = avatar.Warrior;
                var hp = warrior.HP;
                var atk = warrior.BasicAttackValue;
                var es = warrior.ES;
                var actionDone = warrior.ActionDone;
                aniPlayer.Op(() => avatar.RefreshAttrs2(hp, atk, es, actionDone));
            });
        };

        // 回合结束
        room.Battle.OnActionDone += (int team) =>
        {
            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                var op = BattleStage.CurrentOpLayer as InBattleOps;
                op.RemoveShowAttackRange();
                op.ClearPath();

                if (avatar.Warrior.Team != team)
                    return;

                var warrior = avatar.Warrior;
                var hp = warrior.HP;
                var atk = warrior.BasicAttackValue;
                var es = warrior.ES;
                var actionDone = warrior.ActionDone;
                aniPlayer.Op(() => avatar.RefreshAttrs2(hp, atk, es, actionDone));
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

            var hp = target.HP;
            var atk = target.BasicAttackValue;
            var es = target.ES;
            var actionDone = target.ActionDone;

            aniPlayer.MakeAttacking1(mapItem, avatar).OnEnded(
                () =>
                {
                    // avatar.RefreshAttrs2(hp, atk, es, actionDone);
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
            var hp = warrior.HP;
            var atk = warrior.BasicAttackValue;
            var es = warrior.ES;
            var actionDone = warrior.ActionDone;
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs2(hp, atk, es, actionDone));
        };

        bt.OnAddATK += (warrior, dATK) =>
        {
            var hp = warrior.HP;
            var atk = warrior.BasicAttackValue;
            var es = warrior.ES;
            var actionDone = warrior.ActionDone;
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs2(hp, atk, es, actionDone));
        };

        bt.OnAddES += (warrior, des) =>
        {
            var hp = warrior.HP;
            var atk = warrior.BasicAttackValue;
            var es = warrior.ES;
            var actionDone = warrior.ActionDone;
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs2(hp, atk, es, actionDone));
        };

        bt.OnTransfrom += (warrior, state) =>
        {
            var hp = warrior.HP;
            var atk = warrior.BasicAttackValue;
            var es = warrior.ES;
            var actionDone = warrior.ActionDone;
            aniPlayer.Op(() => BattleStage.GetAvatarByWarrior(warrior).RefreshAttrs2(hp, atk, es, actionDone));
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
