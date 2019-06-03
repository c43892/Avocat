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
          //  warriors = new Warrior[w, h];
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

        // 地形材质
        public MapTile.MapMaterialtype [,] GridType;
        
        // Remove12
        public BattleMapItem GetItemAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return items[x, y];
        }

        // 查找指定角色的地图坐标
        public void FindXY(BattleMapItem item, out int px, out int py)
        {
            Debug.Assert(item.Map == this, "the item is not in the map");
            px = item2XY[item].Key;
            py = item2XY[item].Value;
        }

        // 所有角色，remove
        Warrior[,] warriors;

        // 合并warrior和item
        public T GetAt<T>(int x, int y) where T : BattleMapItem
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return items[x, y] as T;
        }

        // 判断是否为warrior
        public bool IsWarrior(int x, int y) 
        {
            if(items[x,y] is Warrior)
                 return true;
            return false;
        }

        public bool IsWarrior(int idInMap)
        {
            bool isWarrior = false;
            ForeachWarrior((x, y, warrior) =>
            {
                if (warrior.IDInMap == idInMap)
                    isWarrior = true;
            });
            return isWarrior;
        }

        //判断是否为Item
        public bool IsItem(int x, int y)
        {
            if (!(items[x, y] is Warrior))
                return true;
            return false;
        }

        public bool IsItem(int idInMap)
        {
            bool isItem = false;
            ForeachItem((x, y, item) =>
            {
                if (item.IDInMap == idInMap && !(item is Warrior))
                    isItem = true;
            });
            return isItem;
        }

        public void Remove(BattleMapItem item)
        {
            Debug.Assert(item == null || item.Map == this, "the item is not in the map");
            FindXYAt(item, out int x, out int y);
            items[x, y] = null;
        }

        public void SetAt(int x, int y, BattleMapItem item)
        {

            Debug.Assert(item == null || item.Map == this, "no item on the map specified");

            if (items[x, y] == item)
                return;

            if (item != null && item.Map == this)
                items[x, y] = null;

            if (items[x, y] != null)
                Remove(items[x, y]);

            items[x, y] = item;
        }

        public T GetByID<T>(int idInMap) where T : BattleMapItem
        {
            T target = null;
            ForeachItem((x, y, item) =>
            {
                if (item.IDInMap == idInMap)
                    target = item as T;
            }, () => target == null);

            return target;
        }

        public void FindXYAt(BattleMapItem target, out int px, out int py)
        {
            Debug.Assert(target.Map == this, "the item is not in the map");

            var founded = false;
            var tx = 0;
            var ty = 0;
            ForeachItem((x, y, item) =>
            {
                if (item == target)
                {
                    founded = true;
                    tx = x;
                    ty = y;
                }
            }, () => !founded);

            Debug.Assert(founded, "the item is not in the map");

            px = tx;
            py = ty;
        }

        void ForeachItem(Action<int, int, BattleMapItem> act, Func<bool> continueCondition = null)
        {
            FC.For2(Width, Height, (x, y) =>
            {
                var item = items[x, y];
                if (item != null)
                    act(x, y, item);

            }, continueCondition);
        }

        public void ForeachWarrior(Action<int, int, Warrior> act, Func<bool> continueCondition = null)
        {
            ForeachItem((x, y, item) =>
            {
                if (item is Warrior)
                    act(x, y, item as Warrior);
            }, continueCondition);
        }

        public void ForeachWarrior(int team, Action<int, int, Warrior> act, Func<bool> continueCondition = null)
        {
            ForeachItem((x, y, BattleMapItem) =>
            {
                var item = items[x, y];
                if (item != null && (item is Warrior)&& (item as Warrior).Team == team)
                    act(x, y, item as Warrior);

            }, continueCondition);
        }

        //Remove1
        public Warrior GetWarriorAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return warriors[x, y];
        }

        //Remove2
        public void RemoveWarrior(Warrior warrior)
        {
            Debug.Assert(warrior == null || warrior.Map == this, "the warrior is not in the map");
            FindXYAt(warrior, out int x, out int y);
            warriors[x, y] = null;
        }

        //Remove3
        public void RemoveItem(BattleMapItem item)
        {
            Debug.Assert(item == null || item.Map == this, "the item is not in the map");
            FindXY(item, out int x, out int y);
            items[x, y] = null;
        }
        //Remove4
        public void SetWarriorAt(int x, int y, Warrior warrior)
        {
            Debug.Assert(warrior == null || warrior.Map == this, "no warrior on the map specified");

            if (warriors[x, y] == warrior)
                return;

            if (warrior != null && warrior.Map == this)
                warriors[x, y] = null;

            if (warriors[x, y] != null)
                Remove(warriors[x, y]);

            warriors[x, y] = warrior;
        }
        //Remove5
        public void SetItemAt(int x, int y, BattleMapItem item)
        {
            Debug.Assert(item == null || item.Map == this, "no item on the map specified");

            if (items[x, y] == item)
                return;

            if (item != null && item.Map == this)
                items[x, y] = null;

            if (items[x, y] != null)
                Remove(items[x, y]);

            items[x, y] = item;
        }

        //Remove6
        // 根据 id 寻找指定角色
        public Warrior GetWarriorByID(int idInMap)
        {
            Warrior target = null;
            ForeachWarriors((x, y, warrior) =>
            {
                if (warrior.IDInMap == idInMap)
                    target = warrior;
            }, () => target == null);

            return target;
        }

        //Remove7
        // 根据 id 寻找指定道具
        public BattleMapItem GetItemByID(int idInMap)
        {
            BattleMapItem target = null;
            ForeachItems((x, y, item) =>
            {
                if (item.IDInMap == idInMap)
                    target = item;
            }, () => target == null);

            return target;
        }

        //Remove8
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


        //Remove9
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

        //Remove10
        // 迭代所有非空战斗角色
        public void ForeachWarriors(int team, Action<int, int, Warrior> act, Func<bool> continueCondition = null)
        {
            FC.For2(Width, Height, (x, y) =>
            {
                var warrior = warriors[x, y];
                if (warrior != null && warrior.Team == team)
                    act(x, y, warrior);

            }, continueCondition);
        }

        //Remove11
        // 迭代所有非空道具
        public void ForeachItems(Action<int, int, BattleMapItem> act, Func<bool> continueCondition = null)
        {
            FC.For2(Width, Height, (x, y) =>
            {
                var item = items[x, y];
                if (item != null)
                    act(x, y, item);

            }, continueCondition);
        }

        // 判断指定位置是否已经被占据
        public bool BlockedAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return true;

            return ((items[x, y] != null && items[x, y].IsObstacle));
        }

        // 寻找最近目标
        public Warrior FindNearestTarget(Warrior warrior)
        {
            Warrior nearestTarget = null;
            int tx = 0;
            int ty = 0;

            warrior.GetPosInMap(out int fx, out int fy);
            warrior.Map.ForeachWarrior((x, y, target) =>
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
        public List<int> FindPath(int fx, int fy, int tx, int ty, int maxSteps)
        {
            var path = new List<int>();

            var nodes = pathFinder.Search(fx, fy, tx, ty, maxSteps, true);
            if (nodes != null)
            {
                nodes.RemoveFirst(); // remove the src node
                FC.ForEach(nodes, (i, n) =>
                {
                    path.Add((int)n.Pos.x);
                    path.Add((int)n.Pos.y);
                }, () => path.Count < maxSteps * 2);
            }

            return path;
        }

        #endregion
    }
}
