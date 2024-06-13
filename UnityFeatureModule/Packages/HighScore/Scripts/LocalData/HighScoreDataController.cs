#nullable enable
namespace TheOneStudio.HighScore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TheOneStudio.HighScore.Models;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    internal sealed class HighScoreDataController : IHighScoreManager
    {
        #region Constructor

        private readonly HighScoreConfig config;
        private readonly HighScoreData   highScoreData;

        [Preserve]
        public HighScoreDataController(HighScoreConfig config, HighScoreData highScoreData)
        {
            this.config        = config;
            this.highScoreData = highScoreData;
        }

        #endregion

        public event OnNewHighScore? OnNewHighScore;

        public void Submit(string key, HighScoreType type, int score)
        {
            var highScores = this.highScoreData[key, type, GetCurrentTime(type)];
            var index      = 0;
            while (index < highScores.Count && highScores[index] > score) ++index;
            highScores.Insert(index, score);
            if (highScores.Count > this.config.CacheSize)
            {
                highScores.RemoveAt(highScores.Count - 1);
            }
            if (index == 0)
            {
                var oldHighScore = highScores.Skip(1).FirstOrDefault();
                this.OnNewHighScore?.Invoke(key, type, oldHighScore, score);
            }
        }

        public void SubmitAll(string key, int score)
        {
            Enum.GetValues(typeof(HighScoreType))
                .Cast<HighScoreType>()
                .ForEach(type => this.Submit(key, type, score));
        }

        public int GetHighScore(string key, HighScoreType type)
        {
            return this.GetAllHighScores(key, type).FirstOrDefault();
        }

        public IEnumerable<int> GetAllHighScores(string key, HighScoreType type)
        {
            return this.highScoreData[key, type, GetCurrentTime(type)];
        }

        private static DateTime GetCurrentTime(HighScoreType type) => type switch
        {
            HighScoreType.Daily   => DateTime.UtcNow.Date,
            HighScoreType.Weekly  => DateTime.UtcNow.GetFirstDayOfWeek(),
            HighScoreType.Monthly => DateTime.UtcNow.GetFirstDayOfMonth(),
            HighScoreType.Yearly  => DateTime.UtcNow.GetFirstDayOfYear(),
            HighScoreType.AllTime => DateTime.MinValue,
            _                     => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}