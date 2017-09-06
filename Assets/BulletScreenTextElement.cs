using UnityEngine;
using UnityEngine.UI;

public struct BulletElementInfo
{
    public float positionX;
    public int lineIndex;
    public float speed;

    public BulletElementInfo (float elmementPositionX, int elementLineIndex, float elementSpeed)
    {
        positionX = elmementPositionX;
        lineIndex = elementLineIndex;
        speed = elementSpeed;
    }
}

public class BulletScreenTextElement : MonoBehaviour {

    private float _speed = 100f;
    private Text _text;
    private RectTransform _rect;

    private bool _availableToMove;
    private bool _informed = false;

    public Vector2 startPos;
    public int lineIndex;

    public bool AvailableToMove{get{return _availableToMove;}}

    public void SetAvailableToMove (bool available)
    {
        _availableToMove = available;
        if (_availableToMove)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            _informed = false;
            _rect.anchoredPosition = startPos;
        }
        BulletScreenArranger.Instance.RefreshLineInfo(_availableToMove, lineIndex);
    }

    public void SetTextAndMove (string content, float speed)
    {
        _text.text = content;
        _speed = speed;
        SetAvailableToMove(true);
    }

    public void Init ()
    {
        _text = GetComponent<Text>();
        _rect = _text.rectTransform;
    }

    private void Update ()
    {
        if (!_availableToMove)
        {
            return;
        }
        Vector2 oldPos = _rect.anchoredPosition;
        float deltaX =  -(_speed * Time.deltaTime);
        _rect.anchoredPosition = new Vector2(oldPos.x + deltaX, oldPos.y);
        
        Vector2 position = new Vector2(_rect.anchoredPosition.x + _rect.rect.width, startPos.y);
        //右边界进入屏幕内，右侧可以显示弹幕
        if (position.x < startPos.x && _informed == false)
        {
            BulletElementInfo info = new BulletElementInfo(position.x, lineIndex, _speed);
            BulletScreenArranger.Instance.RefreshElementInfo(info);
            _informed = true;
        }
        //右边界出屏幕边缘，隐藏弹幕，回复初始位置
        if (_rect.anchoredPosition.x + _rect.rect.width < 0)
        {
            SetAvailableToMove(false);
        }
    }

}
