using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoaderSprites : MonoBehaviour
{
    [SerializeField] private string _links;
    [SerializeField] private string _linkDownloadUrlSprites = 
        "https://drive.google.com/uc?export=download&id=1UEBpojfuhj-Oxn_UFKSQ2yEF9-ofgSGd";

    [SerializeField] private Image _badConnectionToEthernetWindow;
    [SerializeField] private List<string> _listLinks;

    public List<Texture2D> _texturesSymbols;
    [SerializeField] private Texture2D _backCardTexture;

    void Awake()
    {
        StartLoadingProcess();
    }

    public void StartLoadingProcess()
    {
        StartCoroutine(LoadURLsFromGoogle(_linkDownloadUrlSprites));
        ActiveBadConnectionWindow(false);
    }

    private IEnumerator LoadURLsFromGoogle(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                ActiveBadConnectionWindow(true);
            }
            else
            {
                _links = uwr.downloadHandler.text;
            }
        }
        _listLinks = JsonConvert.DeserializeObject<List<string>>(_links);
        GetLinksFromJSON();
    }

    private void GetLinksFromJSON()
    {
        for (int i = 0; i < _listLinks.Count; i++)
        {
            StartCoroutine(LoadSprites(_listLinks[i]));
        }
    }

    private IEnumerator LoadSprites(string link)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(link))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                ActiveBadConnectionWindow(true);
            }
            else
            {
                if (link == _listLinks[0])
                    _backCardTexture = DownloadHandlerTexture.GetContent(uwr);
                else
                    _texturesSymbols.Add(DownloadHandlerTexture.GetContent(uwr));
                if(_texturesSymbols.Count >= 3)
                {
                    if(_backCardTexture != null)
                    {
                        FindObjectOfType<StartGame>().GetSpritesAndStartGame(_texturesSymbols, _backCardTexture);
                    }
                }
            }
        }
    }

    private void ActiveBadConnectionWindow(bool activeWindow)
    {
        if(_badConnectionToEthernetWindow.gameObject.activeSelf != activeWindow)
            _badConnectionToEthernetWindow.gameObject.SetActive(activeWindow);
    }

    public void OnExitHandler()
    {
        Application.Quit();
    }
}
