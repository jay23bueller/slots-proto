using Unity.VisualScripting;
using UnityEngine;

public enum SlotColor
{
    Red,
    Blue,
    Green,
    Purple,
    Yellow,
    DarkGreen
}
public class Slot : MonoBehaviour
{
    [SerializeField]
    public float _speed = 1f;
    private Animator _anim;
    private SpriteRenderer _spriteRender;
    [SerializeField]
    private SlotColor _color;
    public SlotColor color { get => _color; }

    private void Awake()
    {
        _spriteRender = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }
    public void Initialize(Reel reel)
    {
        reel.OnAligned += PlayAlignedAnimation;
        reel.ChangeSpeed += UpdateSpeed;
    }

    public void UpdateSpeed(float speed)
    {
        _speed = speed;
    }

    private void PlayAlignedAnimation()
    {
        _anim.SetTrigger("isAligned");
    }

    public bool checkIfAligned(Vector3 endpointPosition)
    {
        if (Mathf.Abs(_spriteRender.bounds.center.y - _spriteRender.bounds.extents.y - endpointPosition.y) < .1f)
        {
            return true;
        }

        return false;
    }

    public int CheckIfInBound(Bounds reelBound)
    {
        float height = reelBound.extents.y * 2;

        if(_spriteRender.bounds.center.y < reelBound.max.y && _spriteRender.bounds.center.y > reelBound.min.y)
        {
            float distanceFromBottom =  _spriteRender.bounds.center.y - reelBound.min.y;

            if (distanceFromBottom < height * .25f)
                return 3;

            if (distanceFromBottom < height * .5f)
                return 2;

            if (distanceFromBottom < height * .75f)
                return 1;

            return 0;
           
            
        }

        return -1;
    }


    private void OnDrawGizmosSelected()
    {
        if(GetComponent<SpriteRenderer>() != null)
        {

            Gizmos.DrawSphere(_spriteRender.bounds.center, .1f);

            Debug.Log(_spriteRender.bounds.extents.y);
        }

    }



    public void UpdateSlotPosition()
    {
        if(_spriteRender.bounds.center.y - _spriteRender.bounds.extents.y < Camera.main.ViewportToWorldPoint(Vector3.zero).y)
        {
            transform.position = _spriteRender.bounds.center + (Vector3.up * _spriteRender.bounds.extents.y * 12f);
        }
        transform.Translate(Vector3.down * _speed * Time.deltaTime);    
    }


}
