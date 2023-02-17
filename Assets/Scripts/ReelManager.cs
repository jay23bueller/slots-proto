using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReelManager : MonoBehaviour
{
    [SerializeField]
    Reel[] _reels;
    private int _reelReadyCount;
    private AudioSource _audioSource;
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private Button _spinButton;
    [SerializeField]
    private AudioClip _clickClip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        foreach(var reel in _reels)
        {
            reel.OnCompleted += ReelReady;
        }
        _reelReadyCount = _reels.Length;
        _scoreText.gameObject.SetActive(false);
    }
    
    public void PressSpin()
    {
        if(_reelReadyCount == _reels.Length)
        {
            _spinButton.interactable = false;
            _audioSource.PlayOneShot(_clickClip);
            _scoreText.gameObject.SetActive(false);
            _audioSource.Play();
            foreach (var reel in _reels)
            {
                _reelReadyCount--;
                reel.PressSpin();
                
            }
        }
    }

    public void ReelReady()
    {
        _reelReadyCount++;

        if(_reelReadyCount == _reels.Length)
        {
            OnCompletedSpin();
            
        }
    }

    public void OnCompletedSpin()
    {

        List<SlotColor[]> colorLists = new List<SlotColor[]>();
        foreach (var reel in _reels)
        {
            SlotColor[] colors = reel.GetSlotColors();
            colorLists.Add(colors);
        }
        _audioSource.Stop();
        for (int i = 0; i < colorLists[0].Length;i++)
        {
            if (colorLists[0][i] == colorLists[1][i] && colorLists[1][i] == colorLists[2][i])
            {
                _scoreText.text = "You Win"!;
                break;
            }

            _scoreText.text = "You Lose!";
            
        }

        _scoreText.gameObject.SetActive(true);
        StartCoroutine(EnableSpinButtonRoutine());
    }

    private IEnumerator EnableSpinButtonRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        _spinButton.interactable = true;
    }
}
