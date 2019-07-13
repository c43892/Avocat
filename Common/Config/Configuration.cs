using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    /// <summary>
    /// 配置管理，目前只是临时用
    /// </summary>
    public class Configuration
    {
        static readonly Dictionary<Type, Dictionary<string, object>> dAttrs = new Dictionary<Type, Dictionary<string, object>>();

        static Configuration()
        {
            // 野猪
            dAttrs[typeof(Boar)] = new Dictionary<string, object>
            {
                ["AttackingType"] = "physic", // 攻击类型, physic, magic
                ["MaxHP"] = 200, // 最大血量，如果没有设置 HP，默认初始血量就是最大血量
                ["ATK"] = 10,
                ["POW"] = 10,
                ["ARM"] = 0,
                ["RES"] = 0,
                ["AttackRange"] = new int[] { 1 }, // 攻击范围，如果不是 1 或者不止一个范围，就认为是远程攻击
                ["MoveRange"] = 2, // 最大移动距离
            };

            // 黛丽万
            dAttrs[typeof(DaiLiWan)] = new Dictionary<string, object>
            {
                ["AttackingType"] = "magic",
                ["MaxHP"] = 243,
                ["MaxES"] = 260,
                ["ATK"] = 0,
                ["POW"] = 64,
                ["ARM"] = 17,
                ["RES"] = 32,
                ["AttackRange"] = new int[] { 2 },
                ["CardType"] = "PT", // 药水卡
            };

            // 洛里斯
            dAttrs[typeof(LuoLiSi)] = new Dictionary<string, object>
            {
                ["AttackingType"] = "magic",
                ["MaxHP"] = 186,
                ["MaxES"] = 240,
                ["ATK"] = 0,
                ["POW"] = 83,
                ["ARM"] = 22,
                ["RES"] = 20,
                ["AttackRange"] = new int[] { 2 },
                ["CardType"] = "ES", // 盾卡
            };

            // 游川隐
            dAttrs[typeof(YouChuanYin)] = new Dictionary<string, object>
            {
                ["AttackingType"] = "physic",
                ["MaxHP"] = 329,
                ["MaxES"] = 94,
                ["ATK"] = 91,
                ["POW"] = 0,
                ["ARM"] = 34,
                ["RES"] = 28,
                ["AttackRange"] = new int[] { 1 },
                ["CardType"] = "ATK", // 攻击卡
            };

            // 巴洛克
            dAttrs[typeof(BaLuoKe)] = new Dictionary<string, object>
            {
                ["AttackingType"] = "magic",
                ["MaxHP"] = 280,
                ["MaxES"] = 232,
                ["ATK"] = 0,
                ["POW"] = 71,
                ["ARM"] = 20,
                ["RES"] = 20,
                ["ArcherAttackRange"] = new int[] { 3 },
                ["LancerAttackRange"] = new int[] { 1 },
                ["State"] = "Lancer",
                ["CardType"] = "EN", // 能量卡
            };

            // 快速援护
            dAttrs[typeof(FastAssistance)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 15 // 能量消耗
            };

            // 蝶舞单体治疗
            dAttrs[typeof(ButterflySingle)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 20,
            };

            // 蝶舞 AOE 治疗
            dAttrs[typeof(ButterflyAOE)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 50
            };

            // 匠心
            dAttrs[typeof(ArtisanSpirit)] = new Dictionary<string, object>
            {
                ["EN2Add"] = 10 // 每次加多少能量
            };

            // 剑道
            dAttrs[typeof(Kendo)] = new Dictionary<string, object>
            {
                ["EffectRoundNum"] = 2 // 创伤效果持续回合数
            };

            // 一闪
            dAttrs[typeof(FlashAttack)] = new Dictionary<string, object>
            {
                // 伤害计算参数
                ["A"] = 20,
                ["X"] = 150,
                ["Y"] = 40
            };

            // 放置 EMP 炮台的技能
            dAttrs[typeof(DeployEMPCannon)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 30,
                ["Range"] = 2
            };

            // EMP 炮台
            dAttrs[typeof(EMPCannon)] = new Dictionary<string, object>
            {
                ["AttackingType"] = "physic",
                ["AttackRange"] = new int[] { 2,3,4,5 }
            };

            dAttrs[typeof(GainENOnEMPDestroyed)] = new Dictionary<string, object>
            {
                ["ES2Add"] = 5
            };

            // 用于显示buff的技能
            dAttrs[typeof(BuffToDisplay)] = new Dictionary<string, object>
            {
                ["BuffsToDisplay"] = new string [] { "CardATK" , "CounterAttack" , "Faint" , "Untreatable" }
            };

            //// 时光倒流
            //dAttrs[typeof(TimeBack)] = new Dictionary<string, object>
            //{
            //    ["EnergyCost"] = 1
            //};

            // 树干
            dAttrs[typeof(Trunk)] = new Dictionary<string, object>
            {
                ["EffectRoundNum"] = 2
            };

            // 石头
            dAttrs[typeof(Rock)] = new Dictionary<string, object>
            {
                ["EffectRoundNum"] = 2
            };
        }

        public static T Config<T>(T target)
        {
            var type = target.GetType();
            if (!dAttrs.ContainsKey(type))
                return target;

            var attrs = dAttrs[type];
            if (attrs.ContainsKey("MaxHP") && !attrs.ContainsKey("HP"))
                attrs["HP"] = attrs["MaxHP"];

            foreach (var attrName in attrs.Keys)
            {
                var pInfo = type.GetProperty(attrName);
                Debug.Assert(pInfo != null, type.FullName + " has no such an attribute: " + attrName);
                pInfo.SetValue(target, attrs[attrName]);
            }

            return target;
        }
    }
}
