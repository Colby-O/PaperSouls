using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Skill
{
    internal class SkillTreeData : ScriptableObject
    {
        public string SkillTreeName;
        public List<SkillData> Skills;
    }
}
