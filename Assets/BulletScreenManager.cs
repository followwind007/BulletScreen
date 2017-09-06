using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BulletScreenManager : MonoBehaviour {

    [Header("弹幕GameObject，需包含Text组件")]
    [SerializeField]private GameObject _textElement;
    [Header("弹幕字号")]
    [SerializeField]private int _fontSize = 30;
    [Header("弹幕所占屏幕比例")]
    [SerializeField]private float _usedPercentOfScreen = 1f;
    [Header("弹幕的行间距")]
    [SerializeField]private float _elementSpace = 10f;
    [Header("每行弹幕的最大数量")]
    [SerializeField]private int _lineMaxElement = 5;
    [Header("弹幕运动基础速度")]
    [SerializeField]private float _baseSpeed = 100f;
    

    private float _screenWidth = 0;
    private float _screenHeight = 0;

    private Text _text;
    private float _elementHeight = 0;
    private string[] _elementArray;
    private int _elementsMax = 50;

    private float _checkInterval = 0f;

    public float ScreenWidth{get{return _screenWidth;}}
    public float UsedScreenHeight{get{return _screenHeight * _usedPercentOfScreen % _screenHeight; }}
    public float LineHeight{get{return _elementHeight + _elementSpace;}}
    public int LineMaxElement{get{return _lineMaxElement;}}
    public float BaseSpeed {get{return _baseSpeed;}}

    //for test
    private int _tmpCount = 0;
    private bool _isSimulate = false;
    //end

    private void Awake ()
    {
        _elementArray = new string[_elementsMax];
        for (int i = 0; i < _elementsMax; i++)
        {
            _elementArray[i] = "";
        }
    }

    private void Start ()
    {
        InitBulletScreenTextElement();
        RectTransform panelRect = transform as RectTransform;
        _screenWidth = panelRect.rect.width;
        _screenHeight = panelRect.rect.height;
    }

    private void InitBulletScreenTextElement ()
    {
        if (_textElement == null)
        {
            Debug.LogWarning("弹幕元素为空");
            return;
        }
        //初始化Text组件
        _text = _textElement.GetComponent<Text>();
        if (_text == null)
        {
            Debug.LogWarning("弹幕元素未添加Text组件");
            return;
        }
        _text.rectTransform.pivot = new Vector2(0, 1);
        _text.rectTransform.anchorMin = new Vector2(0f, 1f);
        _text.rectTransform.anchorMax = new Vector2(0f, 1f);
        _text.fontSize = _fontSize;
        //初始化ContentSizeFitter组件
        ContentSizeFitter fitter = _textElement.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = _textElement.AddComponent<ContentSizeFitter>();
        }
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //初始化BulletScreenTextElement组件
        BulletScreenTextElement bSTElement = _textElement.GetComponent<BulletScreenTextElement>();
        if (bSTElement == null)
        {
            _textElement.AddComponent<BulletScreenTextElement>();
        }
        _textElement.transform.SetParent(transform);
        _textElement.transform.localScale = Vector3.one;
        //获取文本高度
        StartCoroutine(GetTextElementHeight());
    }

    private IEnumerator GetTextElementHeight ()
    {
        _text.text = "获取弹幕高度的占位文本-Aa19!！}";
        while (_elementHeight <= 0)
        {
            yield return new WaitForSeconds(0.1f);
            _elementHeight = _text.rectTransform.sizeDelta.y;
            BulletScreenArranger.Instance.Init(this);
            _textElement.SetActive(false);
        }
    }

    public BulletScreenTextElement AddBulletScreenTextElement(Vector2 position, int lineIndex)
    {
        //创建TextGameObject
        GameObject newTextElement = Instantiate(_textElement) as GameObject;
        newTextElement.transform.SetParent(transform);
        newTextElement.transform.localScale = Vector3.one;
        newTextElement.SetActive(false);
        
        Text text = newTextElement.GetComponent<Text>();
        text.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        text.text = "";
        //设置弹幕运行的相关属性
        BulletScreenTextElement bullet = newTextElement.GetComponent<BulletScreenTextElement>();
        bullet.startPos = position;
        bullet.lineIndex = lineIndex;
        bullet.Init();
        //弹幕开始运动
        return bullet;
    }

    //获取可以添加弹幕的位置，添加弹幕
    private void GetAvailablePositions ()
    {
        for (int i = 0; i < _elementsMax; i++)
        {
            if (!string.IsNullOrEmpty(_elementArray[i]))
            {
                bool result = BulletScreenArranger.Instance.ArrangePositionsForBulletElement(_elementArray[i]);
                if (result)
                {
                    _elementArray[i] = "";
                }
                return;
            }
        }
    }

    public string GetBulletElementText()
    {
        for (int i = 0; i < _elementsMax; i++)
        {
            if (!string.IsNullOrEmpty(_elementArray[i]))
            {
                string content = _elementArray[i];
                _elementArray[i] = "";
                return content;
            }
        }
        return null;
    }

    public void InsertBulletTextElement (string[] contents)
    {
        int length = contents.Length > _elementsMax ? _elementsMax : contents.Length;
        for (int i = 0; i < length; i++)
        {
            _elementArray[i] = contents[i];
        }
    }

    public void InsertBulletTextElement (string content)
    {
        string[] contents = { content };
        InsertBulletTextElement(contents);
    }


    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isSimulate = !_isSimulate;
        }

        _checkInterval += Time.deltaTime;
        if (_checkInterval > 0.1f)
        {
            SimulateAdd();
            GetAvailablePositions();
            _checkInterval = 0;
        }
    }

    private void SimulateAdd ()
    {
        if (!_isSimulate)
        {
            return;
        }

        _tmpCount++;
        int rep = Random.Range(1, 10);
        string repStr = "";
        for (int j = 0; j < rep; j++)
        {
            repStr += "占";
        }
        string content = "动态弹幕" + repStr + ":" + _tmpCount.ToString();
        InsertBulletTextElement(content);

    }
   



}
