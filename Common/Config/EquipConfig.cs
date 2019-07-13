using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Avocat
{
    // 单个装备配置信息
    struct EquipConfig
    {
        public EquipType type;
        public int hp;
        public int shield;
        public int attack;
        public int magic_power;
        public int armor;
        public int resistance;

        public Dictionary<string, int> attrs;
    }

    /// <summary>
    /// 装备配置相关
    /// </summary>
    public class EquipConfiguration
    {
        // 所有装备配置
        Dictionary<string, EquipConfig> configs;

        // 缓存反射的类型和构造函数
        readonly Dictionary<string, ConstructorInfo> constructors = new Dictionary<string, ConstructorInfo>();

        public void ParseFromText(string txt)
        {
            configs = JsonMapper.ToObject<Dictionary<string, EquipConfig>>(txt);
        }

        public Equip CreateEquip(string id)
        {
            if (!configs.ContainsKey(id))
                throw new Exception("no such equipment: " + id);

            var cfg = configs[id];
            var equip = new Equip
            {
                EquipType = cfg.type,
                MaxHP = cfg.hp,
                MaxES = cfg.shield,
                ATK = cfg.attack,
                POW = cfg.magic_power,
                ARM = cfg.armor,
                RES = cfg.resistance
            };

            foreach (var attrKey in cfg.attrs.Keys)
            {
                ConstructorInfo c = null;
                if (!constructors.ContainsKey(attrKey))
                {
                    var type = Type.GetType("Avocat." + attrKey);
                    c = type.GetConstructor(new Type[0]);
                    constructors[attrKey] = c;
                }
                else
                    c = constructors[attrKey];

                var attr = c.Invoke(new object[0]) as EquipAttr;
                attr.P0 = cfg.attrs[attrKey];
                equip.AddAttr(attr);
            }

            return equip;
        }
    }
}
