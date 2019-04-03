using System;
using System.Collections.Generic;
using Swift;
using Swift.Math;

namespace SCM
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo : SerializableData
    {
        // 胜场次数
        public int WinCount;

        // 败场次数
        public int LoseCount;

        // UserID
        public string ID;

        // 设备型号
        public string DeviceModel;

        protected override void Sync()
        {
            BeginSync();
            SyncInt(ref WinCount);
            SyncInt(ref LoseCount);
            SyncString(ref ID);
            EndSync();
        }
    }
}
