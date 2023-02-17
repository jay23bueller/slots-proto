using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineState
{
    Ready,
    Spinning,
    Aligning,
    Animating
}
public class Reel : MonoBehaviour
{

    [SerializeField]
    private float _finishDelay = 0f;
    public MachineState _machineState;

    [SerializeField]
    private Transform _endpoint;

    [SerializeField]
    private SpriteRenderer _borderRenderer;

    [SerializeField]
    private Slot[] _slots;

    public delegate void Aligned();
    public Aligned OnAligned;

    public delegate void Completed();
    public Completed OnCompleted;

    public Action<float> ChangeSpeed;

    private float _alignAnimationDuration;

    private AudioSource _clickSource;

    private void Start()
    {
        _clickSource = GetComponent<AudioSource>();
        AnimationClip[] clips = _slots[0].GetComponent<Animator>().runtimeAnimatorController.animationClips;
        foreach(var clip in clips)
        {
            if(clip.name == "SlotAligned")
            {
                _alignAnimationDuration = clip.length - .35f;
                break;
            }
        }
        foreach (var slot in _slots)
            slot.Initialize(this);
    }


    public void PressSpin()
    {
        if(_machineState == MachineState.Ready)
        {
            _machineState = MachineState.Spinning;
            StartCoroutine(AlignReelRoutine());
            
        }
    }

    public void StopSpin()
    {
        if(_machineState == MachineState.Spinning)
            _machineState = MachineState.Aligning;
    }

    private void Update()
    {
        switch (_machineState)
        {
            case MachineState.Spinning:
            case MachineState.Aligning:

                foreach (var slot in _slots)
                {
                    slot.UpdateSlotPosition();
                }

                if(_machineState == MachineState.Aligning)
                {
                    foreach (var slot in _slots)
                    {
                        if (slot.checkIfAligned(_endpoint.position))
                        {
                            _machineState = MachineState.Animating;
                            OnAligned();
                            StartCoroutine(StopReelRoutine());
                            break;
                        }
                    }
                    
                }
                break;
        }
    }


    private IEnumerator AlignReelRoutine()
    {
        yield return new WaitForSeconds(_finishDelay);

        if (_machineState == MachineState.Spinning)
            _machineState = MachineState.Aligning;
        
    }

    public SlotColor[] GetSlotColors()
    {
        SlotColor[] colors = new SlotColor[4];
        foreach(var slot in _slots)
        {
            int index = slot.CheckIfInBound(_borderRenderer.bounds);
            if (index != -1)
                colors[index] = slot.color;
        }
        return colors;
    }

    private IEnumerator StopReelRoutine()
    {
        yield return new WaitForSeconds(_alignAnimationDuration);
        _clickSource.Play();
        ChangeSpeed(UnityEngine.Random.Range(8f, 15f));
        _machineState = MachineState.Ready;
        OnCompleted();
        
    }

}
