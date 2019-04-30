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
                ["AttackingType"] = "physic",
                ["MaxHP"] = 200,
                ["ATK"] = 10,
                ["POW"] = 10,
                ["ARM"] = 0,
                ["RES"] = 0,
                ["AttackRange"] = new int[] { 1, 2 },
                ["MoveRange"] = 2,
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
                ["AttackRange"] = new int[] { 1, 2 },
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
                ["AttackRange"] = new int[] { 1, 2 },
                ["CardType"] = "ES", // 盾卡
            };

            // 游川隐
            dAttrs[typeof(YouYinChuan)] = new Dictionary<string, object>
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
                ["ArcherAttackRange"] = new int[] { 1, 2, 3 },
                ["LancerAttackRange"] = new int[] { 1 },
                ["State"] = "Lancer",
                ["CardType"] = "EN", // 能量卡
            };

            // 快速援护
            dAttrs[typeof(FastAssistance)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 50
            };

            // 蝶舞
            dAttrs[typeof(Butterfly)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 50
            };

            // 匠心
            dAttrs[typeof(ArtisanSpirit)] = new Dictionary<string, object>
            {
                ["ES2Add"] = 10
            };

            // 剑道
            dAttrs[typeof(Kendo)] = new Dictionary<string, object>
            {
                ["EffectRoundNum"] = 2
            };

            // 一闪
            dAttrs[typeof(FlashAttack)] = new Dictionary<string, object>
            {
                ["A"] = 20,
                ["X"] = 150,
                ["Y"] = 40
            };

            // EMP 炮台
            dAttrs[typeof(DeployEMPConnon)] = new Dictionary<string, object>
            {
                ["EnergyCost"] = 30
            };

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
