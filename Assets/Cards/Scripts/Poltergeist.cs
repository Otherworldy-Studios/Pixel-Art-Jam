using UnityEngine;

[CreateAssetMenu(fileName = "Poltergeist", menuName = "Undead Cards/Poltergeist")]
public class Poltergeist : UndeadCard
{
    public CardSO cardBeingPossesed;
    public override bool DoSpecial(PlayerStats Owner, CardInstance target, CardInstance instanceOwner)
    {
        specialMessageText = $"{cardName} takes the form of {target.card.cardName}";
        cardBeingPossesed = target.card;
        instanceOwner.isPossessing = true;
        instanceOwner.Initialize(cardBeingPossesed);
        instanceOwner.StartCoroutine(instanceOwner.PlayEffect(cardEffect));
        return true;
    }

    public override void OnCardDiscarded(PlayerStats owner, CardInstance cardPlayed)
    {
        throw new System.NotImplementedException();
    }

    public override void OnCardPlayed(PlayerStats owner, CardInstance cardPlayed, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDamageTaken(PlayerStats owner, CardInstance damaged, CardInstance instanceOwner, int amount)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDeath(PlayerStats owner, CardInstance instanceOwner, CardInstance deadCard)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTurnEnd(PlayerStats owner, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTurnStart(PlayerStats owner, CardInstance instanceOwner)
    {
        throw new System.NotImplementedException();
    }
}
