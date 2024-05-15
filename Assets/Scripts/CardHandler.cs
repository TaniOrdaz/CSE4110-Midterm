using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class CardHandler : MonoBehaviour
{
    [SerializeField] GameSettings settings;
    [SerializeField] GameBoard board;
    [SerializeField] MistakeTracker mistakeTracker;
    [SerializeField] Sprite[] creatureSprites;

    public float mismatchFlipTime = 1f;
    public float flipTime = 1f;
    public float flipStayTime = 1f;

    private Card[] revealedCards;

    private BoxCollider2D cardBox;

    private int matches = 0;


    private bool canReveal = false;
    public bool CanReveal
    {
        get { return canReveal; }
    }

    public void CardRevealed(Card card)
    {
        for (int i = 0; i < settings.MatchSize; i++)
        {
            if (revealedCards[i] == null)
            {
                revealedCards[i] = card;
                StartCoroutine(CheckMatch());
                break;
            }
        }
    }

    public void clearRevealedCards(bool unrevealCards)
    {
        for (int i = 0; i < settings.MatchSize; i++)
        {
            //if an empty entry is found: stop clearing
            if (revealedCards[i] == null)
            {
                break;
            }
            if (unrevealCards)
            {
                revealedCards[i].FlipCard(false);
            }
            revealedCards[i] = null;
        }
    }

    private IEnumerator CheckMatch()
    {
        for (int card = 1; card < settings.MatchSize; card++)
        {
            //if a revealed card is null: stop check
            if (revealedCards[card] == null)
            {
                //Debug.Log($"revealed card {card} is null: match is not complete");
                break;
            }

            //if a revealed card does not match the card before it: unreveal cards
            if (revealedCards[card].Type != revealedCards[card - 1].Type)
            {
                canReveal = false;

                mistakeTracker.mistakeMade();

                //Debug.Log($"revealed card {card} does not match card {card - 1}: match failed");
                yield return new WaitForSeconds(mismatchFlipTime);
                clearRevealedCards(true);

                canReveal = true;
                break;
            }
        }

        //if a matching card is the last entry of revealedCards: match is made
        if (revealedCards[settings.MatchSize - 1] != null)
        {
            //Debug.Log($"match complete!");
            clearRevealedCards(false);

            board.updateScore(board.Score + 1);

            matches++;
            if (matches >= settings.CardTypeAmount * settings.MatchAmount)
            {
                matches = 0; 

                yield return new WaitForSeconds(mismatchFlipTime);
                board.newRound();
            }
        }
    }

    private IEnumerator flipCardsSlowly(bool faceUp, bool flipBack)
    {
        for (int card = 0; card < board.CardAmount; card++)
        {
            yield return new WaitForSeconds(flipTime / board.CardAmount);

            board.Cards[card].FlipCard(faceUp);
        }

        if (flipBack)
        {
            yield return new WaitForSeconds(flipStayTime);

            for (int card = 0; card < board.CardAmount; card++)
            {
                yield return new WaitForSeconds(flipTime / board.CardAmount);

                board.Cards[card].FlipCard(!faceUp);
            }
        }
        
    }

    public void flipAllCards(bool faceUp, bool instant, bool flipBack, bool canRevealAfter)
    {
        canReveal = false;

        if (instant)
        {
            for (int card = 0; card < board.CardAmount; card++)
            {
                board.Cards[card].FlipCard(faceUp);
            }
        }
        else
        {
            StartCoroutine(flipCardsSlowly(faceUp, flipBack));
        }

        canReveal = canRevealAfter;
    }

    public void setCards()
    {

        //make copies of each type in play to fill board
        Card.CreatureType[] cardTypes = new Card.CreatureType[board.CardAmount];
        for (int x = 0; x < board.GridColumns; x++)
        {
            for (int y = 0; y < board.GridRows; y++)
            {
                int index = x + (y * board.GridColumns);
                cardTypes[index] = settings.TypesInPlay[index % settings.CardTypeAmount];
            }
        }

        //shuffle cards types
        Card.CreatureType[] shuffledCards = cardTypes.Clone() as Card.CreatureType[];
        for (int i = 0; i < shuffledCards.Length; i++)
        {
            Card.CreatureType temp = shuffledCards[i];
            int rand = UnityEngine.Random.Range(i, shuffledCards.Length);

            shuffledCards[i] = shuffledCards[rand];
            shuffledCards[rand] = temp;
        }

        //set card types
        for (int i = 0; i < board.CardAmount; i++)
        {
            board.Cards[i].SetCreature(shuffledCards[i], creatureSprites[(int)shuffledCards[i]]);
        }

        revealedCards = new Card[settings.MatchSize];
    }
}
