using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MistakeTracker : MonoBehaviour
{
    [SerializeField] GameSettings settings;
    [SerializeField] GameObject mistakePrefab;
    [SerializeField] CardHandler cardHandler;

    public Color activeColor;
    public Color inactiveColor;

    public float xOffset = 0f;

    private int mistakes;
    private SpriteRenderer[] lights;

    public void setUpMistakeCounter()
    {
        mistakes = 0;

        //if amount of mistake lights is less that the max amount of mistakes
        if (lights.Length < settings.MaxMistakes)
        {
            //create a new set of lights
            SpriteRenderer[] newMistakeLights = new SpriteRenderer[settings.MaxMistakes];
            for (int i = 0; i < newMistakeLights.Length; i++)
            {
                //if there is already an existing light add it to the new set
                if (i < lights.Length)
                {
                    newMistakeLights[i] = lights[i];
                }
                //if not, make a new one
                else
                {
                    newMistakeLights[i] = Instantiate(mistakePrefab, transform).GetComponent<SpriteRenderer>();
                }
            }

            lights = newMistakeLights;
        }

        //find center of all the lights
        float centerX = (settings.MaxMistakes - 1) * .5f;

        //loop through mistake lights
        for (int i = 0;i < lights.Length;i++)
        {
            //if a light is needed to display the max, activate the light
            if (i < settings.MaxMistakes)
            {
                lights[i].gameObject.SetActive(true);
                lights[i].color = activeColor;

                float xPos = (i - centerX);
                lights[i].transform.localPosition = new Vector3(xPos, 0, 0);
            }
            //if not, deactivate it
            else
            {
                lights[i].gameObject.SetActive(false);
            }
        }
    }

    public void mistakeMade()
    {
        mistakes++;

        for (int i = 0; i < settings.MaxMistakes; i++)
        {
            if (i < settings.MaxMistakes - mistakes)
            {
                lights[i].color = activeColor;
            }
            else
            {
                lights[i].color = inactiveColor;
            }
        }

        if ( mistakes >= settings.MaxMistakes)
        {
            cardHandler.StopAllCoroutines();
            cardHandler.flipAllCards(true, false, false, false);
        }
    }

    private void Start()
    {
        lights = new SpriteRenderer[0];
    }
}
