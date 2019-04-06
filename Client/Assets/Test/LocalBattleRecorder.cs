using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Avocat;
using UnityEngine;

/// <summary>
/// 存储在本地的战斗录像机，开发测试用
/// </summary>
public class LocalBattleRecorder : MonoBehaviour
{
    readonly int MAX_REPLAYS_RECORD = 5;
    string SaveFile
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "LocalBattleRecords.replay");
        }
    }

    public List<BattleReplay> Replays
    {
        get
        {
            return replays;
        }
    } List<BattleReplay> replays = new List<BattleReplay>();

    public void AddReplay(BattleReplay replay)
    {
        if (inReplaying)
            return;
 
        replays.Add(replay);
        while (replays.Count > MAX_REPLAYS_RECORD)
            replays.RemoveAt(0);
    }

    public void SaveAll()
    {
        using (var w = new BinaryWriter(new FileStream(SaveFile, FileMode.Create)))
        {
            w.Write(replays.Count);
            var buff = new WriteBuffer();
            FC.ForEach(replays, (i, replay) => buff.Write(replay));
            w.Write(buff.Data);
            w.Close();
        }
    }

    public void LoadAll()
    {
        if (!File.Exists(SaveFile))
            return;

        using (var r = new BinaryReader(new FileStream(SaveFile, FileMode.Open)))
        {
            var count = r.ReadInt32();
            var data = r.ReadBytes((int)r.BaseStream.Length);
            var buff = new RingBuffer();
            buff.Write(data);

            FC.For(count, (i) =>
            {
                var replay = buff.Read<BattleReplay>();
                replays.Add(replay);
            });

            r.Close();
        }
    }

    bool inReplaying = false;
    public void Play(BattleReplay replay, Action<byte[]> msgHandler)
    {
        inReplaying = true;
        StartCoroutine(ImplPlaying(replay, msgHandler));
    }

    IEnumerator ImplPlaying(BattleReplay replay, Action<byte[]> msgHandler)
    {
        for (var i = 0; i < replay.Messages.Count; i++)
        {
            var msg = replay.Messages[i];
            msgHandler(msg);
            yield return new TimeWaiter(1000);
        }

        inReplaying = false;
    }
}
 