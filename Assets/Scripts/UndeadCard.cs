using UnityEngine;

[CreateAssetMenu(fileName = "UndeadCard", menuName = "Scriptable Objects/UndeadCard")]
public class UndeadCard : CardSO
{
    public override void Attack(GameObject owner, GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public override void DoSpecial()
    {
        throw new System.NotImplementedException();
    }

    public override void DoSpecial(GameObject Owner, int manaCost)
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
