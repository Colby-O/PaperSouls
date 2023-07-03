using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Skill
{
    public class SkillTree
    {
        public List<SkillData> Skills;

        public List<List<int>> AdjacencyList;

        public SkillTree(List<SkillData> skills)
        {
            Skills = skills;
        }

        /// <summary>
        /// Adds a directional connection between two skills in the Skill Tree i.e. parnet skill to child skill
        /// </summary>
        private void AddConnection(int parnetID, int childID)
        {
            AdjacencyList[parnetID].Add(childID);
        }

        /// <summary>
        /// Constructs the Skill Tree as an Adjacency List
        /// </summary>
        public void ConstructAdjacencyList()
        {
            AdjacencyList = new();

            for (int i = 0; i < Skills.Count; i++) {
                AdjacencyList.Add(new());
                foreach (SkillData child in Skills[i].Children)
                {
                    AddConnection(i, Skills.IndexOf(child));
                }
            }
        }
    }
}
