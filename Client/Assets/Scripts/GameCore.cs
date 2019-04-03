using System.Collections;
using System.Collections.Generic;
using Swift;
using SCM;
using Swift.Math;
using System;

public class GameCore : Core
{
    public static GameCore Instance { get { return gc; } }
    static GameCore gc = new GameCore();

    bool inited = false;
    public override void Initialize()
    {
        if (inited)
            return;

        BuildBaseComponents();
        BuildLogicComponents();

        // 初始化所有模块
        base.Initialize();
        inited = true;
    }

    // 默认方式创建给定模块
    T BC<T>() where T : Component, new()
    {
        var c = new T();
        Add(typeof(T).Name, c);
        return c;
    }

    // 通用基础模块
    public void BuildBaseComponents()
    {
        var nc = BC<NetCore>(); // 网络
        BC<ServerPort>(); // 消息端口
        ServerConnectionExt.ServerMessageHandler = "UserPort";
        nc.OnDisconnected += OnDisconnected;

        BC<CoroutineManager>(); // 协程
    }

    // 网络连接断开
    public event Action<Connection, string> OnMainConnectionDisconnected = null;
    private void OnDisconnected(Connection conn, string reason)
    {
        OnMainConnectionDisconnected.SC(conn, reason);
    }

    // 逻辑模块
    public void BuildLogicComponents()
    {
    }

    // 服务器连接对象
    public Connection ServerConnection { get; set; }

    // 玩家自身信息
    public string MeID { get; set; }
    public UserInfo MeInfo { get; set; }

    // 自己在战斗中的 player 编号
    public int MePlayer { get; set; }

    public BattleRoomClient Room = null;
    public override void RunOneFrame(int timeElapsed)
    {
        base.RunOneFrame(timeElapsed);
    }
}