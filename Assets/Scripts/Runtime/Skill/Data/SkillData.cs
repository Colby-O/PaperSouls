using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Skill
{
    [System.Serializable,  CreateAssetMenu(fileName = "SkillData", menuName = "Skills/Skill", order = 1)]
    public class SkillData : ScriptableObject
    {
        [Header("Identification and Description")]
        public int ID = -1;
        public string SkillName;
        [TextArea(4, 4)] public string Description;
        public UnityEngine.Sprite Icon;

        [Header("Requierments and Sub Skills")]
        public SkillData Parnet;
        public List<SkillData> Children;

        private bool _isUnlocked = false;

        public bool IsUnlocked => _isUnlocked;

        /// <summary>
        /// Swaps the unlocked state of the skill
        /// </summary>
        public void UpdateUnlockedState()
        {
            _isUnlocked = !IsUnlocked;
        }
    }
}
