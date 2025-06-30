using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SongSlector : MonoBehaviour
{
    [Serializable]
    class SongInfo
    {
        public GameObject songCard;
        public SongSpec songSpec;
    }

    public int selectedIndex
    {
        get => _selectedIndex;
        set
        {
            _songInfos[_selectedIndex].songCard.SetActive(false); // 이전 카드 비활성화
            _selectedIndex = value;
            _songInfos[_selectedIndex].songCard.SetActive(true); // 현재 카드 활성화
            _songTilte.text = _songInfos[_selectedIndex].songSpec.title; // 노래 제목 바꿔줌
        }
    }

    [SerializeField] private TMP_Text _songTilte;
    [SerializeField] private SongInfo[] _songInfos;
    [SerializeField] private Button _next;
    [SerializeField] private Button _prev;
    [SerializeField] private Button _play;
    private int _selectedIndex;

    private void Start()
    {
        for(int i = 0; i < _songInfos.Length; i++)
        {
            _songInfos[i].songCard.SetActive(i == _selectedIndex);
        }

        _songTilte.text = _songInfos[_selectedIndex].songSpec.title;
    }

    private void OnEnable()
    {
        _next.onClick.AddListener(Next);
        _prev.onClick.AddListener(Prev);
        _play.onClick.AddListener(Play);
    }

    private void OnDisable()
    {
        _next.onClick.RemoveListener(Next);
        _prev.onClick.RemoveListener(Prev);
        _play.onClick.RemoveListener(Play);
    }

    public void Next()
    {
        selectedIndex = (_selectedIndex + 1) % _songInfos.Length;
    }

    public void Prev()
    {
        selectedIndex = (_selectedIndex + _songInfos.Length - 1) % _songInfos.Length;
    }

    public void Play()
    {
        GameManager.gameSession.selectedSongSpec = _songInfos[_selectedIndex].songSpec;
        SceneManager.LoadScene("InGame");
    }
}
