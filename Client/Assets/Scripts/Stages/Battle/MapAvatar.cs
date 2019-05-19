using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Avocat;
using Spine.Unity;

public class MapAvatar : MonoBehaviour
{
    public BattleStage BattleStage { get; set; }
    public Vector2 CenterOffset = new Vector2(0.5f, 0.5f);

    public TextMesh NameText;
    public TextMesh HpText;
    public TextMesh PowerText;
    public TextMesh ShieldText;

    // 对应的角色
    public Warrior Warrior { get; set; }

    // 延迟刷新属性显示，值按照当前属性值算
    public Action DelayRefreshAttrs()
    {
        var hp = Warrior.HP;
        var atk = Warrior.BasicAttackValue;
        var es = Warrior.ES;
        var actionDone = Warrior.ActionDone;
        return () =>
        {
            if (this != null) // may has been destroyed.
                RefreshAttrs2(hp, atk, es, actionDone);
        };


    }

    // 刷新属性值到指定值
    public void RefreshAttrs2(int hp, int basicAttackValue, int es, bool actionDone)
    {
        HpText.text = hp.ToString();
        PowerText.text = basicAttackValue.ToString();
        ShieldText.text = es.ToString();
        ShieldText.transform.parent.gameObject.SetActive(es > 0);
        Color = actionDone ? ColorActionDone : ColorDefault;
    }

    // 刷新属性值
    public void RefreshAttrs()
    {
        NameText.text = Warrior.DisplayName;
        RefreshAttrs2(Warrior.HP, Warrior.BasicAttackValue, Warrior.ES, Warrior.ActionDone);
    }

    public int X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
            transform.localPosition = new Vector3(x + CenterOffset.x, y + CenterOffset.y);
        }
    } int x;

    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
            transform.localPosition = new Vector3(x + CenterOffset.x, y + CenterOffset.y);
        }
    } int y;

    public static readonly Color ColorDefault = Color.black;
    public static readonly Color ColorSelected = Color.blue;
    public static readonly Color ColorActionDone = Color.gray;

    public Color Color
    {
        get
        {
            return transform.Find("Name").Find("Name").GetComponent<TextMesh>().color;
        }
        set
        {
            transform.Find("Name").Find("Name").GetComponent<TextMesh>().color = value;
        }
    }

    void SetAni(Warrior warrior, string path)
    {
        var atlasName = path + warrior.Name + ".atlas";
        var skeletonName = path + warrior.Name;
        var atlas = Resources.Load<TextAsset>(atlasName);
        var skeletonJson = Resources.Load<TextAsset>(skeletonName);
        var atlasdata = ScriptableObject.CreateInstance<SpineAtlasAsset>();

        // 提取出atlas文件中所有的materials
        string atlasStr = atlas.ToString();
        string[] atlasLines = atlasStr.Split('\n');
        List<string> _lsPng = new List<string>();
        for (int i = 0; i < atlasLines.Length - 1; i++)
        {
            if (atlasLines[i].Length == 0)
                _lsPng.Add(atlasLines[i + 1]);
        }
        Material[] maters = new Material[_lsPng.Count];
        for (int i = 0; i < _lsPng.Count; i++)
        {
            maters[i] = new Material(Shader.Find("Spine/Skeleton"));
            maters[i].mainTexture = Resources.Load<Texture2D>(path + _lsPng[i].Substring(0, _lsPng[i].Length - 4));
        }

        // 设置skeletonAnimation相关参数
        atlasdata.materials = maters;
        var runtimeAtlasAsset = SpineAtlasAsset.CreateRuntimeInstance(atlas, atlasdata.materials, true);
        var runtimeSkeletonDataAsset = SkeletonDataAsset.CreateRuntimeInstance(skeletonJson, runtimeAtlasAsset, true);
        var skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.skeletonDataAsset = runtimeSkeletonDataAsset;
        skeletonAnimation.Initialize(true);
        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
    }

    public void SetAnimation(Warrior warrior) {
        string path;
        if (warrior is ITransformable)
        {
            path = "Animation/" + warrior.Name + "/" + (warrior as ITransformable).State + "/";
        }
        else {
            path = "Animation/" + warrior.Name + "/";
        }
        SetAni(warrior, path);
    }
}
