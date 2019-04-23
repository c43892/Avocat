using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Swift;
using Swift.AStar;
using Swift.Math;

namespace Avocat
{
    /// <summary>
    /// 战斗地图
    /// </summary>
    public class BattleMap
    {
        public Battle Battle { get; set; }

        // 用来给加入地图的 item 进行 id 分配
        public int ItemIDInMap
        {
            get
            {
                return ++itemInMapIndex;
            }
        } int itemInMapIndex = 0;

        public BattleMap(int w, int h)
        {
            Width = w;
            Height = h;
            grids = new int[w, h];
            items = new BattleMapItem[w, h];
            warriors = new Warrior[w, h];
            BuildPathFinder();
        }

        // 地图尺寸
        public int Width { get; private set; }
        public int Height { get; private set; }

        // 所有地块底图
        public int[, ] Grids
        {
            get
            {
                return grids;
            }
        } int[,] grids;

        // 所有道具
        BattleMapItem[,] items;
        Dictionary<int, BattleMapItem> id2Items = new Dictionary<int, BattleMapItem>();
        Dictionary<BattleMapItem, KeyValuePair<int, int>> item2XY = new Dictionary<BattleMapItem, KeyValuePair<int, int>>();

        public BattleMapItem GetItemAt(int x, int y)
        {
            return items[x, y];
        }

        // 查找指定角色的地图坐标
        public void FindXY(BattleMapItem item, out int px, out int py)
        {
            Debug.Assert(item.Map == this, "the item is not in the map");
            px = item2XY[item].Key;
            py = item2XY[item].Value;
        }

        // 所有角色
        Warrior[,] warriors;

        public Warrior GetWarriorAt(int x, int y)
        {
            return warriors[x, y];
        }

        public void RemoveWarrior(Warrior warrior)
        {
            Debug.Assert(warrior == null || warrior.Map == this, "the warrior is not in the map");
            FindXY(warrior, out int x, out int y);
            warriors[x, y] = null;
        }

        public void SetWarriorAt(int x, int y, Warrior warrior)
        {
            Debug.Assert(warrior == null || warrior.Map == this, "no warrior on the map specified");

            if (warriors[x, y] == warrior)
                return;

            // refresh the fast search  cache

            if (warrior != null && warrior.Map == this)
                warriors[x, y] = null;

            if (warriors[x, y] != null)
                RemoveWarrior(warriors[x, y]);

            warriors[x, y] = warrior;
        }

        // 根据 id 寻找指定角色
        public Warrior GetWarriorsByID(int idInMap)
        {
            Warrior target = null;
            ForeachWarriors((x, y, warrior) =>
            {
                if (warrior.IDInMap == idInMap)
                    target = warrior;
            }, () => target == null);

            return target;
        }

        // 查找指定角色的地图坐标
        public void FindXY(Warrior target, out int px, out int py)
        {
            Debug.Assert(target.Map == this, "the warrior is not in the map");

            var founded = false;
            var tx = 0;
            var ty = 0;
            ForeachWarriors((x, y, warrior) =>
            {
                if (warrior == target)
                {
                    founded = true;
                    tx = x;
                    ty = y;
                }
            }, () => !founded);

            Debug.Assert(founded, "the warrior is not in the map");

            px = tx;
            py = ty;
        }

        // 迭代所有非空战斗角色
        public void ForeachWarriors(Action<int, int, Warrior> act, Func<bool> continueCondition = null)
        {
            FC.For2(Width, Height, (x, y) =>
            {
                var warrior = warriors[x, y];
                if (warrior != null)
                    act(x, y, warrior);

            }, continueCondition);
        }

        // 判断指定位置是否已经被占据
        public bool BlockedAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return true;

            return (warriors[x, y] != null && warriors[x, y].IsObstacle) || (items[x, y] != null && items[x, y].IsObstacle);
        }

        // 寻找最近目标
        public Warrior FindNearestTarget(Warrior warrior)
        {
            Warrior nearestTarget = null;
            int tx = 0;
            int ty = 0;

            warrior.GetPosInMap(out int fx, out int fy);
            warrior.Map.ForeachWarriors((x, y, target) =>
            {
                // 过滤队友和已经死亡的敌人
                if (target.IsDead || target.Team == warrior.Team)
                    return;

                if (nearestTarget == null || MU.ManhattanDist(fx, fy, x, y) < MU.ManhattanDist(fx, fy, tx, ty))
                {
                    tx = x;
                    ty = y;
                    nearestTarget = target;
                }
            });

            return nearestTarget;
        }

        #region 寻路相关

        // 检查指定区域是否被占用
        public bool CheckSpareSpace(int cx, int cy, int r)
        {
            var free = true;
            FC.RectFor(cx, cx, cy, cy, (x, y) =>
            {
                free = x >= 0 && x < Width
                        && y >= 0 && y < Height
                        && !BlockedAt(x, y);
            }, () => free);

            return free;
        }

        class PathNode : IPathNode<int>
        {
            public Vec2 Pos { get; private set; }
            Func<int, int, int, bool> CheckSpareSpace = null;

            public PathNode(int x, int y, Func<int, int, int, bool> checker)
            {
                Pos = new Vec2(x, y);
                CheckSpareSpace = checker;
            }

            public Boolean IsWalkable(int ps)
            {
                return CheckSpareSpace((int)Pos.x, (int)Pos.y, ps);
            }
        }

        PathNode[,] pathNodes = null;
        SpatialAStar<PathNode, int> pathFinder = null;

        // 构建寻路器
        void BuildPathFinder()
        {
            pathNodes = new PathNode[Width, Height];
            FC.For2(Width, Height, (x, y) => pathNodes[x, y] = new PathNode(x, y, CheckSpareSpace));
            pathFinder = new SpatialAStar<PathNode, int>(pathNodes) { Only4Adjacent = true };
        }

        // 寻路
        public List<int> FindPath(int fx, int fy, int tx, int ty, int radius)
        {
            var path = new List<int>();

            var nodes = pathFinder.Search(fx, fy, tx, ty, radius, true);
            if (nodes != null)
            {
                nodes.RemoveFirst(); // remove the src node
                foreach (var n in nodes)
                {
                    path.Add((int)n.Pos.x);
                    path.Add((int)n.Pos.y);
                }
            }

            return path;
        }

        #endregion
    }
}
