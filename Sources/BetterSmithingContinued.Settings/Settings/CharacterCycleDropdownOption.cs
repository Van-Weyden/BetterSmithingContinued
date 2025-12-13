using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.InputSystem;

namespace BetterSmithingContinued.Settings
{
    public class CharacterCycleDropdownOption
    {
        public enum OrderType
        {
            Default,
            SkillAsc,
            SkillDesc
        };

        public OrderType Type { get; }
        public string DisplayString { get; }

        public CharacterCycleDropdownOption(OrderType type, string displayString)
        {
            this.Type = type;
            this.DisplayString = displayString;
        }

        public override string ToString()
        {
            return this.DisplayString;
        }
    }
}
