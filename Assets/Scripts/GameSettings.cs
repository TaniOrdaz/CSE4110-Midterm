using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] CardHandler cardHandler;
    [SerializeField] GameObject gameBoard;
    [SerializeField] GameObject boardUI;
    [SerializeField] GameObject settingUI;

    [SerializeField] bool[] isCreatureInPlay;

    //amount of matches per creature type
    [SerializeField] int matchAmount = 1;
    //amount of cards needed for a match
    [SerializeField] int matchSize = 2;
    //amount of mistakes allowed per game
    [SerializeField] int maxMistakes = 1;

    //amount of card types in play
    private int cardTypeAmount = 1;
    //which creature types are on the board
    private Card.CreatureType[] typesInPlay;

    [SerializeField] Slider[] sliders;
    [SerializeField] Toggle[] inPlayToggles;
    private SliderHandleLabel[] sliderHandles;

    public int MatchAmount
    {
        get { return matchAmount; }
    }
    public int MatchSize
    {
        get { return matchSize; }
    }
    public int MaxMistakes
    {
        get { return maxMistakes; }
    }
    public int CardTypeAmount
    {
        get { return cardTypeAmount; }
    }
    public Card.CreatureType[] TypesInPlay
    {
        get { return typesInPlay; }
    }

    private bool[] nextCreaturesInPlay;
    private int nextMatchAmount;
    private int nextMatchSize;
    private int nextMaxMistakes;

    public void toggleUI(bool showGameBoard)
    {
        gameBoard.SetActive(showGameBoard);
        boardUI.SetActive(showGameBoard);

        settingUI.SetActive(!showGameBoard);
        if (showGameBoard)
        {

        }
        else
        {
            setSliderValues();
        }
    }

    public void toggleUI()
    {
        toggleUI(!gameBoard.activeSelf);
    }

    public void getSliderValues()
    {
        nextMatchAmount = (int)sliders[0].value;
        nextMatchSize = (int)sliders[1].value;
        nextMaxMistakes = (int)sliders[2].value;

        for (int i = 0; i < inPlayToggles.Length; i++)
        {
            nextCreaturesInPlay[i] = inPlayToggles[i].isOn;
        }
    }

    private void setSliderValues()
    {
        sliders[0].value = nextMatchAmount;
        sliders[1].value = nextMatchSize;
        sliders[2].value = nextMaxMistakes;

        for (int i = 0; i < sliderHandles.Length; i++)
        {
            sliderHandles[i].setText(sliders[i]);
        }

        for (int i = 0; i < inPlayToggles.Length; i++)
        {
            inPlayToggles[i].isOn = nextCreaturesInPlay[i];
        }
    }

    private void setTypesInPlay()
    {
        cardTypeAmount = getCardTypeAmount();

        //get each type thats in play
        typesInPlay = new Card.CreatureType[cardTypeAmount];
        int inPlayIndex = 0;
        for (int i = 0; i < (int)Card.CreatureType.amount; i++)
        {
            if (isCreatureInPlay[i])
            {
                typesInPlay[inPlayIndex] = (Card.CreatureType)i;
                inPlayIndex++;
            }
        }
    }

    public int getCardTypeAmount()
    {
        int amount = 0;

        for (int i = 0; i < (int)Card.CreatureType.amount; i++)
        {
            //Debug.Log($"{(Card.CreatureType)i} in play: {isCreatureInPlay[i]}");
            if (isCreatureInPlay[i])
            {
                amount++;
            }
        }

        return amount;
    }

    public bool Changed()
    {
        bool changed = false;

        //check and set matchAmount
        if (nextMatchAmount != matchAmount)
        {
            changed = true;
            matchAmount = nextMatchAmount;
        }
        //check and set matchSize
        if (nextMatchSize != matchSize)
        {
            changed = true;
            matchSize = nextMatchSize;
        }
        //dont check maxMistakes, but set it anyway
        if (nextMaxMistakes != maxMistakes)
        {
            maxMistakes = nextMaxMistakes;
        }

        //check and set creatures in play
        bool cardTypesChanged = false;
        for (int i = 0; i < (int)Card.CreatureType.amount; i++)
        {
            if (nextCreaturesInPlay[i] != isCreatureInPlay[i])
            {
                cardTypesChanged = true;
                isCreatureInPlay[i] = nextCreaturesInPlay[i];
            }
        }
        if (cardTypesChanged)
        {
            changed = true;
            setTypesInPlay();
        }

        //Debug.Log(changed);
        return changed;
    }

    public void Start()
    {
        //set next settings to starter settings
        //clear starter settings
        //this is so that the first match has to be built up from the starter settings
        nextMatchAmount = matchAmount;
        matchAmount = 0;
        nextMatchSize = matchSize;
        matchSize = 0;
        nextMaxMistakes = maxMistakes;
        maxMistakes = 0;

        sliderHandles = new SliderHandleLabel[3];
        sliderHandles[0] = sliders[0].GetComponentInChildren<SliderHandleLabel>();
        sliderHandles[1] = sliders[1].GetComponentInChildren<SliderHandleLabel>();
        sliderHandles[2] = sliders[2].GetComponentInChildren<SliderHandleLabel>();

        setTypesInPlay();
        nextCreaturesInPlay = isCreatureInPlay.Clone() as bool[];
        Array.Clear(isCreatureInPlay,0, isCreatureInPlay.Length);

        toggleUI(true);
    }
}
