using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    public static class ClientConfiguration
    {
        static readonly Dictionary<Type,Dictionary<string,object>> SkillAttribute= new Dictionary<Type, Dictionary<string, object>>();
        static ClientConfiguration()
        {
            SkillAttribute[typeof(ButterflySingle)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "蝶舞",
                ["SkillDescription"] = "治疗单个友方单位 50% 最大生命值"
            };

            SkillAttribute[typeof(ButterflyAOE)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "蝶舞",
                ["SkillDescription"] = "治疗所有友方单位 50% 最大生命值"
            };

            SkillAttribute[typeof(StarsTears)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "星之泪",
                ["SkillDescription"] = "行动阶段前，每当友方单位受到治疗时，为受到治疗的友方单位提供护盾"
            };

            SkillAttribute[typeof(DeployEMPCannon)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "EMP炮台",
                ["SkillDescription"] = "在空地部署EMP炮台"
            };

            SkillAttribute[typeof(ArtisanSpirit)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "匠心",
                ["SkillDescription"] = "行动阶段前，为团队提供能量"
            };

            SkillAttribute[typeof(GainENOnEMPDestroyed)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "回收",
                ["SkillDescription"] = "EMP 炮台摧毁时增加能量"
            };

            SkillAttribute[typeof(Kendo)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "剑道",
                ["SkillDescription"] = "攻击时施加创伤效果，使目标不可被治疗"
            };

            SkillAttribute[typeof(FlashAttack)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "一闪",
                ["SkillDescription"] = "一闪技能描述一闪技能描述"
            };

            SkillAttribute[typeof(FastAssistance1)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "快速援护",
                ["SkillDescription"] = "切换巴洛克形态"
            };

            SkillAttribute[typeof(FastAssistance2)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "快速援护",
                ["SkillDescription"] = "巴洛克始终处于近战状态，技能效果变为加满护盾"
            };

            SkillAttribute[typeof(TacticalCommand)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "战术指挥",
                ["SkillDescription"] = "行动阶段前，生成一张指令卡"
            };

            SkillAttribute[typeof(TacticalCommandImpl1)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "战术指挥",
                ["SkillDescription"] = "行动阶段前，生成一张指令卡"
            };

            SkillAttribute[typeof(TacticalCommandImpl2)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "战术指挥",
                ["SkillDescription"] = "行动阶段前，全体魔力和攻击力提升一层"
            };

            SkillAttribute[typeof(DaiLiWan)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "黛丽万"
            };

            SkillAttribute[typeof(LuoLiSi)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "洛里斯"
            };

            SkillAttribute[typeof(YouChuanYin)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "游川隐"
            };

            SkillAttribute[typeof(BaLuoKe)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "巴洛克"
            };

            SkillAttribute[typeof(Boar)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "野猪"
            };

            SkillAttribute[typeof(EMPCannon)] = new Dictionary<string, object>
            {
                ["DisplayName"] = "EMP炮台"
            };
        }

        public static Y GetAttribute<T,Y>(T target, string keyName)
        {
            var type = target.GetType();
            Debug.Assert(SkillAttribute[type] != null, type.FullName + " has no such an attribute: " + keyName);
            var attri = SkillAttribute[type];
            Y value = (Y)attri[keyName];
            return value;
        }
    }
}

