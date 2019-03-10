using XLua;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace CustomHotfix {
    public static class HotfixConfig {

        [Hotfix]
        public static List<Type> hotfixList
        {
            get
            {
                List<string> allowNames = new List<string>();//设定为白名单的类名列表
                allowNames.Add("GameManager");
                allowNames.Add("AnimationManager");
                allowNames.Add("CardManager");
                return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
                        where allowNames.Contains(type.Name) select type).ToList();
            }
        }
    }
}
