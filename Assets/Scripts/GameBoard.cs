using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameSettings settings;
    [SerializeField] CardHandler cardHandler;
    [SerializeField] MistakeTracker mistakeTracker;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] TMP_Text scoreLabel;

    public float offsetX = 0f;
    public float offsetY = 0f;
    public float cardScale = 1f;

    private float gridCenterX = 0f;
    private float gridCenterY = 0f;
    private float adjustedCardScale = 0f;

    private int gridRows = 1;
    public int GridRows
    {
        get { return gridRows; }
    }
    private int gridColumns = 1;
    public int GridColumns
    {
        get { return gridColumns; }
    }

    private int score = 0;
    public int Score
    {
        get { return score; }
    }

    private Card[] cards;
    public Card[] Cards
    {
        get { return cards; }
    }
    private int cardAmount;
    public int CardAmount
    {
        get { return cardAmount; }
    }
    private BoxCollider2D cardBox;
    public void startGame()
    {
        if (settings.Changed())
        {
            if (cards != null)
            {
                removeCards(cards.Clone() as Card[]);
            }
            newBoard();
        }

        mistakeTracker.setUpMistakeCounter();

        updateScore(0);
        newRound();
    }

    public void removeCards(Card[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            Destroy(cards[i].gameObject);
        }
    }

    public void updateScore(int value)
    {
        score = value;
        scoreLabel.text = $"Score - {score}";
    }


    public void newRound()
    {
        cardHandler.StopAllCoroutines();
        cardHandler.setCards();

        cardHandler.flipAllCards(false,true,false,false);
        cardHandler.flipAllCards(true,false,true, true);
    }

    public void newBoard()
    {
        //calculate and sort grid dimensions
        //copyAmount = Amount of copies of each creature type
        //Debug.Log($"match amount: {settings.MatchAmount}, match size: {settings.MatchSize}, card type amount: {settings.CardTypeAmount}");
        int copyAmount = settings.MatchAmount * settings.MatchSize;
        int[] gridDimensions = new int[] { copyAmount, settings.CardTypeAmount };
        Array.Sort(gridDimensions);

        //columns gets the larger value, rows get the shorter value
        gridColumns = gridDimensions[1];
        gridRows = gridDimensions[0];

        cardAmount = gridColumns * gridRows;
        cards = new Card[cardAmount];

        gridCenterX = (gridColumns - 1) * .5f;
        gridCenterY = (gridRows - 1) * .5f;

        //card scale get larger the less columns there are
        adjustedCardScale = cardScale / gridColumns;

        //loop through each card in the grid
        for (int x = 0; x < gridColumns; x++)
        {
            for (int y = 0; y < gridRows; y++)
            {
                //indexs increase with value on grid
                int index = x + (y * gridColumns);

                cards[index] = Instantiate(cardPrefab, cardHandler.transform).GetComponent<Card>();
                cards[index].handler = cardHandler;

                //add dimensions of cardBox to offset
                //scale offset by card scale
                float offsetXScaled = (offsetX + cardBox.size.x) * adjustedCardScale;
                float offsetYScaled = (offsetY + cardBox.size.y) * adjustedCardScale;
                //card position is position in grid scalled by offset
                float posX = x * offsetXScaled;
                float posY = y * offsetYScaled;
                //offset each card by the center of the board scaled by card scale
                posX -= gridCenterX * offsetXScaled;
                posY -= gridCenterY * offsetYScaled;

                //cards are scaled cardscale adjusted for amount of rows
                cards[index].transform.localPosition = new Vector3(posX, posY, 0f);
                cards[index].transform.localScale = Vector3.one * adjustedCardScale;
            }
        }
    }

    private void Start()
    {
        cardBox = cardPrefab.GetComponent<BoxCollider2D>();
    }
}
