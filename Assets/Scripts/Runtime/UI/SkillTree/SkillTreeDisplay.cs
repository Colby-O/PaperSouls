using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Skill;

namespace PaperSouls.Runtime.UI.SkillTreeUI
{
    internal sealed class SkillTreeDisplay : MonoBehaviour
    {
        [SerializeField] private SkillTreeData _skillTreeData;
        [SerializeField] private GameObject _skillElement;

        private SkillTree _skillTree;

        /// <summary>
        /// Display's a m-ary tree based on the Reingold and Tilford algorithm.
        /// <para>Source: Reingold, E. M., Tilford, J. S. (1981). Tidier drawings of trees. IEEE Transactions on Software Engineering, SE-7(2), 223–228. https://doi.org/10.1109/tse.1981.234519 </para> 
        /// </summary>
        private void Display()
        {

        }

        private void Awake()
        {
            _skillTree = new(_skillTreeData.Skills);
            _skillTree.ConstructAdjacencyList();
        }
    }
}
