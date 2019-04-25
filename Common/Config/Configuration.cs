using System;
using System.Collections.Generic;
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
                ["AttackRange"] = 1,
                ["MoveRange"] = 2,
                ["MaxHP"] = 10,
                ["HP"] = 10,
                ["ATK"] = 1
            };

            // 黛丽万
            dAttrs[typeof(DaiLiWan)] = new Dictionary<string, object>
            {
                ["AttackRange"] = 3,
                ["MoveRange"] = 3,
                ["MaxHP"] = 10,
                ["HP"] = 10,
                ["MaxES"] = 10,
                ["ATK"] = 1,
            };

            // 洛里斯
            dAttrs[typeof(LuoLiSi)] = new Dictionary<string, object>
            {
                ["AttackRange"] = 1,
                ["MoveRange"] = 3,
                ["MaxHP"] = 10,
                ["HP"] = 10,
                ["MaxES"] = 10,
                ["ATK"] = 3
            };

            // 游川隐
            dAttrs[typeof(YouYinChuan)] = new Dictionary<string, object>
            {
                ["AttackRange"] = 1,
                ["MoveRange"] = 3,
                ["MaxHP"] = 10,
                ["HP"] = 10,
                ["MaxES"] = 10,
                ["ATK"] = 3
            };

            // 巴洛克
            dAttrs[typeof(BaLuoKe)] = new Dictionary<string, object>
            {
                ["MaxHP"] = 10,
                ["HP"] = 10,
                ["MaxES"] = 10,
                ["ArcherAttackRange"] = 5,
                ["ArcherATK"] = 1,
                ["LancerAttackRange"] = 1,
                ["LancerATK"] = 3,
                ["State"] = "Lancer"
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
                ["Shield2Add"] = 10
            };

            // 剑道
            dAttrs[typeof(Kendo)] = new Dictionary<string, object>
            {
                ["EffectRoundNum"] = 2
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
            var type = typeof(T);
            if (!dAttrs.ContainsKey(type))
                return target;

            var attrs = dAttrs[type];
            foreach (var attrName in attrs.Keys)
            {
                var pInfo = type.GetProperty(attrName);
                pInfo.SetValue(target, attrs[attrName]);
            }

            return target;
        }
    }
}
