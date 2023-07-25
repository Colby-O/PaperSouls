using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.DungeonGeneration;

namespace PaperSouls.Runtime.MonoSystems.DungeonGeneration
{
    internal sealed class GenerateDungeonMessage : IMessage
    {
        public int Seed;

        public GenerateDungeonMessage(int seed)
        {
            Seed = seed;
        }
    }

    internal sealed class LoadDungeonMessage : IMessage
    {
        public Dungeon Dungeon;

        public LoadDungeonMessage(Dungeon dungeon)
        {
            Dungeon = dungeon;
        }
    }

    internal sealed class DestoryDungeonMessage : IMessage
    {
        public DestoryDungeonMessage()
        {

        }
    }

    internal sealed class StartChunkLoadingMessage : IMessage
    {
        public StartChunkLoadingMessage()
        {

        }
    }

    internal sealed class StopChunkLoadingMessage : IMessage
    {
        public StopChunkLoadingMessage()
        {

        }
    }
}
