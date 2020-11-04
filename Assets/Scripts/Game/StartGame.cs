using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
	[SerializeField] private int _gridRows = 2;
	[SerializeField] private int _gridCols = 3;
	[SerializeField] private float _offsetX = 500f;
	[SerializeField] private float _offsetY = 450f;

	[SerializeField] private Card _originalCard;
	[SerializeField] private Text _scoreLabel;
	[SerializeField] private Text _timerToStartGame;
	[SerializeField] private List<Card> _cards;

	private Card _firstRevealed;
	private Card _secondRevealed;
	private int _score = 0;

	[SerializeField] private List<Texture2D> _texturesSymbols;
	private Texture2D _backCardTexture;

	private void Start()
	{
		_originalCard.gameObject.SetActive(false);
		_scoreLabel.gameObject.SetActive(false);
		_timerToStartGame.gameObject.SetActive(false);
		_offsetX = _offsetX / 1920 * Screen.width;
		_offsetY = _offsetY / 1080 * Screen.height;
	}

	public bool canReveal
	{
		get { return _secondRevealed == null; }
	}

	public void GetSpritesAndStartGame(List<Texture2D> textures, Texture2D texture)
	{
		_originalCard.gameObject.SetActive(true);
		_scoreLabel.gameObject.SetActive(true);
		_texturesSymbols = textures;
		_backCardTexture = texture;
		CreateCards();
	}

	private void CreateCards()
	{
		Vector3 startPos = _originalCard.transform.position;
		
		int[] numbers = { 0, 0, 1, 1, 2, 2};
		numbers = ShuffleArray(numbers);

		for (int i = 0; i < _gridCols; i++)
		{
			for (int j = 0; j < _gridRows; j++)
			{
				Card card;

				if (i == 0 && j == 0)
				{
					card = _originalCard;
				}
				else
				{
					card = Instantiate(_originalCard, transform);
				}

				int index = j * _gridCols + i;
				int id = numbers[index];
				card.SetCard(id, _backCardTexture, _texturesSymbols[id]);

				float posX = (_offsetX * i) + startPos.x;
				float posY = -(_offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);
				card.GetComponent<Button>().enabled = false;
				_cards.Add(card);				
			}
		}
		StartCoroutine(AnimText());
	}

	IEnumerator AnimText()
	{
		_timerToStartGame.gameObject.SetActive(true);
		for (int i=5; i > 0; i--)
		{
			_timerToStartGame.text = i.ToString();
			yield return new WaitForSeconds(1f);
		}
		_timerToStartGame.gameObject.SetActive(false);
		for(int i = 0; i < _cards.Count; i++)
		{
			_cards[i].StartViewAnim();
			_cards[i].GetComponent<Button>().enabled = true;
		}
	}

	private int[] ShuffleArray(int[] numbers)
	{
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++)
		{
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(Card card)
	{
		if (_firstRevealed == null)
		{
			_firstRevealed = card;
		}
		else
		{
			_secondRevealed = card;
			StartCoroutine(CheckMatch());
		}
	}

	private IEnumerator CheckMatch()
	{
		if (_firstRevealed.Id == _secondRevealed.Id)
		{
			yield return new WaitForSeconds(1f);
			_score++;
			_scoreLabel.text = _score.ToString();
			_firstRevealed.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.1f);
			_secondRevealed.gameObject.SetActive(false);
			if(_score%3 == 0)
			{
				NextLevel();
			}
		}

		else
		{
			yield return new WaitForSeconds(1f);
			_firstRevealed.StandartViewAnim();
			yield return new WaitForSeconds(0.1f);
			_secondRevealed.StandartViewAnim();
		}

		_firstRevealed = null;
		_secondRevealed = null;
	}

	private void NextLevel()
	{
		int[] numbers = { 0, 0, 1, 1, 2, 2 };
		numbers = ShuffleArray(numbers);
		for (int i = 0; i < _cards.Count; i++)
		{
			int id = numbers[i];
			_cards[i].gameObject.SetActive(true);
			_cards[i].SetCard(id, _backCardTexture, _texturesSymbols[id]);
			//_cards[i].gameObject.transform.rotation = new Quaternion(0, 180, 0, 1);
			_cards[i].Onclick = false;
			_cards[i].AnimActive = false;
			_cards[i].GetComponent<Button>().enabled = false;
		}
		StartCoroutine(AnimText());
	}

	public void Restart()
	{
		SceneManager.LoadScene(1);
	}
}
