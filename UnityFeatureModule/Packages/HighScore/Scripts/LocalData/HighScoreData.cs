#nullable enable
namespace TheOneStudio.HighScore
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using TheOneStudio.HighScore.Models;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    internal sealed class HighScoreData : ILocalData
    {
        [Preserve]
        public HighScoreData()
        {
        }

        [JsonProperty] private readonly Dictionary<string, Dictionary<HighScoreType, Dictionary<DateTime, List<int>>>> data = new();

        public List<int> this[string key, HighScoreType type, DateTime time] => this.data.GetOrAdd(key).GetOrAdd(type).GetOrAdd(time);

        void ILocalData.Init() { }
    }
}