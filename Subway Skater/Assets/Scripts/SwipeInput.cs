using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInput : MonoBehaviour {

    public enum Direction
    {
        Center = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }
    public Direction SwipeDirection { get { return swipeDirection; } }
    private Direction swipeDirection;

    public static SwipeInput Instance { get { return instance; } }
    private static SwipeInput instance;


    private Vector2 startPosition;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        swipeDirection = Direction.Center;


        #if UNITY_EDITOR || UNITY_STANDALONE
                MouseInput();   // 滑鼠偵測
        #elif UNITY_ANDROID
		        MobileInput();  // 觸碰偵測
        #endif
    }

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            swipeDirection = HandDirection(startPosition, pos);
            Debug.Log("swipeDirection : " + swipeDirection.ToString());
        }
    }

    void MobileInput()
    {
        if (Input.touchCount <= 0)
            return;

        //1個手指觸碰螢幕
        if (Input.touchCount == 1)
        {

            //開始觸碰
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                Debug.Log("Began");
                //紀錄觸碰位置
                startPosition = Input.touches[0].position;

                //手指移動
            }
            else if (Input.touches[0].phase == TouchPhase.Moved)
            {
                Debug.Log("Moved");
                //移動攝影機
                //Camera.main.transform.Translate (new Vector3 (-Input.touches [0].deltaPosition.x * Time.deltaTime, -Input.touches [0].deltaPosition.y * Time.deltaTime, 0));
            }


            //手指離開螢幕
            if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                Debug.Log("Ended");
                Vector2 endPosition = Input.touches[0].position;

                swipeDirection = HandDirection(startPosition, endPosition);
                Debug.Log("swipeDirection : " + swipeDirection.ToString());
            }
            
        }
        /*  //攝影機縮放，如果1個手指以上觸碰螢幕
        else if (Input.touchCount > 1)
        {

            //記錄兩個手指位置
            Vector2 finger1 = new Vector2();
            Vector2 finger2 = new Vector2();

            //記錄兩個手指移動距離
            Vector2 move1 = new Vector2();
            Vector2 move2 = new Vector2();
            
            //是否是小於2點觸碰
            for (int i = 0; i < 2; i++)
            {
                Touch touch = Input.touches[i];

                if (touch.phase == TouchPhase.Ended)
                    break;

                if (touch.phase == TouchPhase.Moved)
                {
                    //每次都重置
                    float move = 0;

                    //觸碰一點
                    if (i == 0)
                    {
                        finger1 = touch.position;
                        move1 = touch.deltaPosition;
                        //另一點
                    }
                    else
                    {
                        finger2 = touch.position;
                        move2 = touch.deltaPosition;

                        //取最大X
                        move = (finger1.x > finger2.x) ? move1.x : move2.x;

                        //取最大Y，並與取出的X累加
                        move += (finger1.y > finger2.y) ? move1.y : move += move2.y;

                        //當兩指距離越遠，Z位置加的越多，相反之
                        Camera.main.transform.Translate(0, 0, move * Time.deltaTime);
                    }
                }
            }//end for
        }//end else if */
    }//end void

    Direction HandDirection(Vector2 startPosition, Vector2 endPosition)
    {
        Direction tempDirection;

        if (startPosition == endPosition)
            return Direction.Center;

        //手指水平移動
        if (Mathf.Abs(startPosition.x - endPosition.x) > Mathf.Abs(startPosition.y - endPosition.y))
        {
            if (startPosition.x > endPosition.x)
            {
                //手指向左滑動
                tempDirection = SwipeInput.Direction.Left;
            }
            else
            {
                //手指向右滑動
                tempDirection = SwipeInput.Direction.Right;
            }
        }
        else
        {
            if (startPosition.y > endPosition.y)
            {
                //手指向下滑動
                tempDirection = SwipeInput.Direction.Down;
            }
            else
            {
                //手指向上滑動
                tempDirection = SwipeInput.Direction.Up;
            }
        }
        return tempDirection;
    }
}