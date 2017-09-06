using UnityEngine;
using System.Collections.Generic;

public class LineInfo {
    public float lastSpeed;
    public float borderPostion;
    public int movingCount;
    public int informedCount;
}
 
public class BulletScreenArranger {

    private static BulletScreenArranger _arranger;

    private BulletScreenManager _manager;
    
    private int _lineMax;
    private float _speedMax = 10000f;

    private LineInfo[] _lineArray;
    private List<BulletScreenTextElement>[] _lineElementArray;

    public static BulletScreenArranger Instance
    {
        get
        {
            if (_arranger == null)
            {
                _arranger = new BulletScreenArranger(); 
            }
            return _arranger;
        }
    }
    
    public void Init (BulletScreenManager manager)
    {
        _manager = manager;

        _lineMax = (int)(_manager.UsedScreenHeight / _manager.LineHeight);
        _lineArray = new LineInfo[_lineMax];
        _lineElementArray = new List<BulletScreenTextElement>[_lineMax];
        
        for (int i = 0; i < _lineMax; i++)
        {
            LineInfo line = new LineInfo();
            line.borderPostion = _manager.ScreenWidth;
            line.lastSpeed = _speedMax;
            line.movingCount = 0;
            line.informedCount = 0;
            List<BulletScreenTextElement> lineElements = new List<BulletScreenTextElement>();
            for (int j = 0; j < _manager.LineMaxElement; j++)
            {
                BulletScreenTextElement bullet = 
                    _manager.AddBulletScreenTextElement(new Vector2(_manager.ScreenWidth, -_manager.LineHeight * i), i);
                lineElements.Add(bullet);
            }
            _lineElementArray[i] = lineElements;
            _lineArray[i] = line;
        }
    }


    //更新行弹幕信息
    public void RefreshElementInfo (BulletElementInfo info)
    {
        int index = info.lineIndex;
        _lineArray[index].informedCount++;
        _lineArray[index].lastSpeed = info.speed;
    }
    //更新行信息
    public void RefreshLineInfo (bool moveState, int lineIndex)
    {
        if (moveState == true)
        {
            _lineArray[lineIndex].movingCount++;
        }
        else
        {
            _lineArray[lineIndex].movingCount--;
            _lineArray[lineIndex].informedCount--;
            if (_lineArray[lineIndex].movingCount == 0)
            {
                _lineArray[lineIndex].lastSpeed = _speedMax;
            }
        }
    }

    //获取可以使用的弹幕载体
    public bool ArrangePositionsForBulletElement (string content)
    {
        float speed = GetSpeedForContent(content);
        for (int i = 0; i < _lineMax; i++)
        {
            //该行是否可显示弹幕
            int freeElementsCount = _manager.LineMaxElement - _lineArray[i].movingCount;
            bool lastElementEnter = _lineArray[i].movingCount == _lineArray[i].informedCount;
            
            if (lastElementEnter && freeElementsCount > 0 && _lineArray[i].lastSpeed >= speed)
            {
                //在行内找到合适的位置来放置弹幕
                for (int j = 0; j < _lineElementArray[i].Count; j++)
                {
                    if (_lineElementArray[i][j].AvailableToMove == false)
                    {
                        _lineElementArray[i][j].SetTextAndMove(content, speed);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private float GetSpeedForContent (string content)
    {
        return _manager.BaseSpeed + content.Length * 10f;
    }


}

