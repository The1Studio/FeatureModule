namespace TheOneStudio.HyperCasual.StateMachines.Game.Interfaces
{
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;

    public interface IGameState : IState
    {
    }

    public interface IGameState<T> : IGameState
    {
        T Model { get; set; }
    }
}