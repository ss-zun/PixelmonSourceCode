public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public interface IData
{
    string Rcode { get; }
}

public interface IDamagable
{
    void OnChangeHealth(float delta);
}