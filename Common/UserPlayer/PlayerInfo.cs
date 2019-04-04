using System;
using System.Collections.Generic;
using Swift;
using Swift.Math;

namespace Avocat
{
    /// <summary>
    /// 玩家信息
    /// </summary>
    public class PlayerInfo : SerializableData
    {
        public string ID;
        public string Name;

        protected override void Sync()
        {
            BeginSync();
            SyncString(ref ID);
            SyncString(ref Name);
            EndSync();
        }
    }
}
