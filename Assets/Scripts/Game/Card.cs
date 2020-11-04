using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Sprite _cardback;
    [SerializeField] private Sprite _cardsymbol;
    [SerializeField] private StartGame _startGame;
    [SerializeField] private int _timer = 0;

    public int Id;
    public bool AnimActive = false;
    public bool Onclick = false;

    public void SetCard(int id, Texture2D back, Texture2D symbol)
    {
        Id = id;
        SetCardBackSprite(back);
        SetCardSymbolSprite(FindObjectOfType<LoaderSprites>()._texturesSymbols[Id]);
    }

    private void SetCardBackSprite(Texture2D texture)
    {
        _cardback = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1.0f);
    }

    private void SetCardSymbolSprite(Texture2D texture)
    {
        _cardsymbol = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1.0f);
        GetComponent<Image>().sprite = _cardsymbol;
    }
    
    public void OnClickAnim()
    {
        if (Onclick == false && _startGame.canReveal && AnimActive == false)
        {
            StartCoroutine(AnimClick());
            _startGame.CardRevealed(this);
        }
    }

    private void Flip()
    {
        if (Onclick == false)
        {
            GetComponent<Image>().sprite = _cardsymbol;
        }
        else if(Onclick == true)
        {
            GetComponent<Image>().sprite = _cardback;
        }
    }
    
    private IEnumerator AnimClick()
    {
        AnimActive = true;
        for (int i = 0; i < 60; i++)
        {
            yield return null;
            transform.Rotate(new Vector3(0,3,0));
            _timer += 3;

            if(_timer == 90 || _timer == -90)
            {
                Flip();
            }
        }
        if(Onclick == true)
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        else
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
        Onclick = !Onclick;
        _timer = 0;
        AnimActive = false;
    }

    public void StartViewAnim()
    {
        Onclick = true;
        StartCoroutine(AnimClick());
    }

    public void StandartViewAnim()
    {
        if(Onclick == true)
        {
            StartCoroutine(AnimClick());
        }
    }
}
