using System.Numerics;

public class PlayerHealthSystem : HealthSystem
{ 
    public void InitHealth(BigInteger hp)
    {
        maxHealth = hp;
        currentHealth = maxHealth;
    }

    public override void TakeDamage(BigInteger delta, bool isCri = false, bool isPlayer = true)
    {
        def = (int)Player.Instance.statHandler.def;
        base.TakeDamage(delta, isCri, isPlayer);
    }

    protected override void NoticeDead()
    {
        Player.Instance.fsm.ChangeState(Player.Instance.fsm.DieState);
    }
}