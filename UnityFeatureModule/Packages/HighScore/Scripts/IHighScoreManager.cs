#nullable enable
namespace TheOneStudio.HighScore
{
    using System.Collections.Generic;
    using TheOneStudio.HighScore.Models;

    public delegate void OnNewHighScore(string key, HighScoreType type, int oldHighScore, int newHighScore);

    /// <summary>
    ///     Control local high score data
    /// </summary>
    public interface IHighScoreManager
    {
        public event OnNewHighScore OnNewHighScore;

        /// <summary>
        ///     Submit score. If a new high score reached, invoke <see cref="OnNewHighScore"/>.
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="Submit(HighScoreType, int)"/>.
        /// </param>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <param name="score">
        ///     Possible new high score
        /// </param>
        /// <returns>
        ///     True if a new high score reached
        /// </returns>
        public bool Submit(string key, HighScoreType type, int score);

        /// <summary>
        ///     Submit score for all types. If a new high score reached, invoke <see cref="OnNewHighScore"/>.
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="SubmitAll(int)"/>.
        /// </param>
        /// <param name="score">
        ///     Possible new high score
        /// </param>
        public void SubmitAll(string key, int score);

        /// <summary>
        ///     Get high score
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="GetHighScore(HighScoreType)"/>.
        /// </param>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>Highest score</returns>
        public int GetHighScore(string key, HighScoreType type);

        /// <summary>
        ///     Get all high scores
        /// </summary>
        /// <param name="key">
        ///     Use this to separate GameModes, Characters, ...
        ///     If you don't have any, use <see cref="DEFAULT_KEY"/> or <see cref="GetAllHighScores(HighScoreType)"/>.
        /// </param>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>All high scores order from highest to lowest</returns>
        public IEnumerable<int> GetAllHighScores(string key, HighScoreType type);

        #region Default

        /// <summary>
        ///     Default Key if you don't have multiple GameModes, Characters, ...
        /// </summary>
        public const string DEFAULT_KEY = "DEFAULT";

        /// <summary>
        ///     Submit score. If a new high score reached, invoke <see cref="OnNewHighScore"/>.
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <param name="score">
        ///     Possible new high score
        /// </param>
        /// <returns>
        ///     True if a new high score reached
        /// </returns>
        public bool Submit(HighScoreType type, int score) => this.Submit(DEFAULT_KEY, type, score);

        /// <summary>
        ///     Submit score for all types. If a new high score reached, invoke <see cref="OnNewHighScore"/>.
        /// </summary>
        /// <param name="score">
        ///     Possible new high score
        /// </param>
        public void SubmitAll(int score) => this.SubmitAll(DEFAULT_KEY, score);

        /// <summary>
        ///     Get high score
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>Highest score</returns>
        public int GetHighScore(HighScoreType type) => this.GetHighScore(DEFAULT_KEY, type);

        /// <summary>
        ///     Get all high scores
        /// </summary>
        /// <param name="type">
        ///     <see cref="HighScoreType"/>
        /// </param>
        /// <returns>All high scores order from highest to lowest</returns>
        public IEnumerable<int> GetAllHighScores(HighScoreType type) => this.GetAllHighScores(DEFAULT_KEY, type);

        #endregion
    }
}