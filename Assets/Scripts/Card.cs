using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer creatureRenderer;

    [SerializeField] GameObject front;
    [SerializeField] GameObject creature;

    [SerializeField] private CreatureType creatureType;

    public CardHandler handler;
    private bool faceUp = false;

    public enum CreatureType
    {
        Whelppy,
        Spriglett,
        Cloddle,
        Dripryo,
        Embebe,
        Dregg,
        Irrifant,
        amount
    }

    public CreatureType Type
    { 
        get { return creatureType; } 
    }

    public void SetCreature(CreatureType type, Sprite sprite)
    {
        creatureType = type;
        creatureRenderer.sprite = sprite;
    }

    public void FlipCard(bool flippedUp)
    {
        front.SetActive(flippedUp);
        creature.SetActive(flippedUp);
        faceUp = flippedUp;
    }

    public void OnMouseDown()
    {
        if (!faceUp && handler.CanReveal)
        {
            FlipCard(true);
            handler.CardRevealed(this);
        }
    }
}
