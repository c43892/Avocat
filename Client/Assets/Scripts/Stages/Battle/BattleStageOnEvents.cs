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
            aniPlayer.Op(() =>
            {
                var avFrom = BattleStage.Avatars[fromX, fromY];
                var avTo = BattleStage.Avatars[toX, toY];

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
                var doRefresh = avatar.DelayRefreshAttrs();
                aniPlayer.Op(doRefresh);
            });
        };

        // 角色攻击
        room.Battle.OnWarriorAttack += (Warrior attacker, Warrior target, Skill skill, List<string> flags) =>
        {
            var attackerAvatar = BattleStage.GetAvatarByWarrior(attacker);
            var targetAvatar = BattleStage.GetAvatarByWarrior(target);
            var doAttackerRefresh = attackerAvatar.DelayRefreshAttrs();
            var doTargetRefresh = attackerAvatar.DelayRefreshAttrs();
            if (flags.Contains("SkillAttack"))
            {
                aniPlayer.SkillAttacking(attackerAvatar, targetAvatar).OnEnded(() => { doAttackerRefresh(); doTargetRefresh(); });
            }
            else {
                aniPlayer.MakeAttacking2(attackerAvatar, targetAvatar).OnEnded(() => { doAttackerRefresh(); doTargetRefresh(); });
            }
            
        };

        // 角色移动
        room.Battle.OnWarriorMovingOnPath += (Warrior warrior, int x, int y, List<int> path) =>
        {
            var tx = path[path.Count - 2];
            var ty = path[path.Count - 1];

            MapAvatar avatar = BattleStage.GetAvatarByWarrior(warrior);
            Debug.Assert(avatar != null && avatar.Warrior == warrior, "the avatar is not just on the start position");

            var doRefresh = avatar.DelayRefreshAttrs();
            aniPlayer.MakeMovingOnPath(
                avatar.transform, 5,
                FC.ToArray(path, (i, p, doSkip) => i % 2 == 0 ? p + avatar.CenterOffset.x : p + avatar.CenterOffset.y)
            ).OnEnded(() =>
            {
                BattleStage.SetAvatarPosition(null, x, y);
                BattleStage.SetAvatarPosition(avatar, tx, ty);
                doRefresh();
            });
        };

        // 回合开始
        room.Battle.OnNextRoundStarted += (int team) =>
        {
            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                if (avatar.Warrior.Team != team)
                    return;

                var doRefresh = avatar.DelayRefreshAttrs();
                aniPlayer.Op(doRefresh);
            });
        };

        // 每回合重置英雄释放技能的标记
        room.Battle.OnNextRoundStarted += (int team) =>
        {
            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                if (avatar.Warrior.Team != team)
                    return;

                avatar.Warrior.IsSkillReleased = false;
            });
        };

        // 回合结束
        room.Battle.OnActionDone += (int team) =>
        {
            aniPlayer.Op(() =>
            {
                var op = BattleStage.CurrentOpLayer as InBattleOps;
                op.RemoveShowAttackRange();
                op.RemoveMovingPathRange();
                op.ClearPath();
            });

            BattleStage.ForeachAvatar((x, y, avatar) =>
            {
                if (avatar.Warrior.Team != team)
                    return;

                var doRefresh = avatar.DelayRefreshAttrs();
                aniPlayer.Op(doRefresh);
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
        (room.Battle as BattlePVE).OnUseItem2 += (Avocat.ItemOnMap item, Warrior target) =>
        {
            var mapItem = BattleStage.GetMapItemByItem(item);
            var avatar = BattleStage.GetAvatarByWarrior(target);

            var doRefresh = avatar.DelayRefreshAttrs();
            aniPlayer.MakeAttacking1(mapItem, avatar).OnEnded(
                () =>
                {
                    doRefresh();
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
            var doRefresh = BattleStage.GetAvatarByWarrior(warrior).DelayRefreshAttrs();
            aniPlayer.Op(doRefresh);
        };

        bt.OnAddATK += (warrior, dATK) =>
        {
            var doRefresh = BattleStage.GetAvatarByWarrior(warrior).DelayRefreshAttrs();
            aniPlayer.Op(doRefresh);
        };

        bt.OnAddES += (warrior, des) =>
        {
            var doRefresh = BattleStage.GetAvatarByWarrior(warrior).DelayRefreshAttrs();
            aniPlayer.Op(doRefresh);
        };

        bt.OnTransfrom += (warrior, state) =>
        {
            var doRefresh = BattleStage.GetAvatarByWarrior(warrior).DelayRefreshAttrs();
            aniPlayer.Op(doRefresh);
        };

        bt.OnAddWarrior += (x, y, warrior) =>
        {
            aniPlayer.Op(() => BattleStage.CreateWarriorAvatar(x, y, warrior));
        };

        bt.OnTimeBackTriggered += (BattleReplay replay) =>
        {
            aniPlayer.Op(() => BattleStage.OnTimeBackTriggered(replay));
        };

        bt.BeforeFireSkill += (ActiveSkill skill) =>
        {
            aniPlayer.FireSkill(skill);
        };

        bt.BeforeFireSkillAt += (ActiveSkill skill, int x, int y) =>
        {
            aniPlayer.FireSkillAt(skill, x, y);
        };

        bt.AfterFireSkill += (ActiveSkill skill) =>
        {
            skill.Owner.IsSkillReleased = true;
        };

        bt.AfterFireSkillAt += (ActiveSkill skill, int x, int y) =>
        {
            skill.Owner.IsSkillReleased = true;
        };
    }
}
