//----------------------------------------------
// Runtime Curve Editor
// Copyright © 2013 Rus Artur PFA
// rus_artur@yahoo.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Draws lines inside curve editor and manage user interaction with curves and keys
/// </summary>
public class CurveLines : MonoBehaviour
{
    public Material lineMaterial;//material used for drawing lines
                                 //	static void CreateLineMaterial() {
                                 //	    if( !lineMaterial ) {
                                 //	        lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
                                 //			    "Properties {" +
                                 //				"_Color (\"Main Color\", Color) = (1,0,0,1)"+
                                 //				"}"+
                                 //	            "SubShader { Pass { " +
                                 //	            "    Blend SrcAlpha OneMinusSrcAlpha " +
                                 //			    "    Colormask RGBA Lighting Off " +
                                 //			    "    ZTest LEqual  ZWrite Off Cull Off Fog { Mode Off }  Color[_Color]" +
                                 //	            //"    BindChannels {" +
                                 //	            //"      Bind \"vertex\", vertex Bind \"color\", color }" +
                                 //	            "} } }" );
                                 //			
                                 //	        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                                 //	        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                                 //	    }
                                 //	}

    CurveWindow crvWindow;

    List<CurveStruct> curveStructList = new List<CurveStruct>();

    //possible colors when drawing more curves 
    Color[] colors = { Color.red, Color.green, Color.yellow, Color.blue, Color.magenta, Color.cyan };
    List<Color> listColors = new List<Color>();//list with the colors currently used

    Rect mWindowBottom;//keeps the rect for the curves with basic shapes
    public Rect windowBottom
    {
        set { mWindowBottom = value; }
        get { return mWindowBottom; }
    }

    Rect mGridRect;//the rect of the grid ,in world coordinates
    public Rect gridRect
    {
        set { mGridRect = value; }
        get { return mGridRect; }
    }

    float mUnit;//unit in world coordinates(constant in pixels),used for calculating the number of lines that should be shown in the grid
    public float unit
    {
        set { mUnit = value; }
    }

    ///use a dictionary to keep the place of the row and the coresponding gradation number
    Dictionary<float, decimal> mRows = new Dictionary<float, decimal>();
    public Dictionary<float, decimal> rows
    {
        get { return mRows; }
    }

    int mColls;
    public int colls
    {
        get { return mColls; }
    }

    float mRezid = 0;//rezidual value,used when calculating the number of horyzontal lines to be displayed
    public float Rezid
    {
        get { return mRezid; }
    }

    int mHorLines = 2;//have to know how many lines are displayed for the current size of the grid(actually lines+1 will be total number of displayed lines)

    static Color lineColor = new Color(0.10f, 0.10f, 0.10f, 1.0f);

    Rect mGradRect = new Rect(0, 0, 0, 0);//Rect given by the gradations limits
    Rect mPrevGradRect;//prev grad rect,so to know when the grad rect got changed	
    public Rect gradRect
    {
        get
        {
            return mGradRect;
        }
        set
        {
            mPrevGradRect = mGradRect;
            mGradRect = value;
            if (mGradRect != mPrevGradRect)
            {
                if (mGradRect.yMax > 0 && mGradRect.yMin < 0)
                {
                    mHorLines = GetHorLines(mGradRect.yMax);
                    mHorLines += GetHorLines(-mGradRect.yMin);
                }
                else
                {
                    mHorLines = GetHorLines(mGradRect.yMax - mGradRect.yMin);
                }

                if (curveStructList.Count > 0)
                {
                    if (mActiveCurveStr.gradRect != mGradRect)
                    {
                        mActiveCurveStr.gradRect = mGradRect;
                        UpdateCurveStruct(mActiveCurveStr);
                    }
                }
            }
        }
    }

    static CurveStruct nullCurve = new CurveStruct(null, null, Color.clear);
    CurveStruct mActiveCurveStr = nullCurve;// keeps the struct with the active curve
    public CurveStruct activeCurveStr
    {
        get { return mActiveCurveStr; }
    }

    bool lineSelected = false;//true if the user's now selecting a line(e.g. when the user drags it)
    bool keySelected = false;//true,only when the user,mouse down on a key,for moving that key(this is different to 'selectedKey > -1') 
    int selectedKey = -1;//the index of the selected key,-1 if none is selected(if keySelected is true,than this is the key whose's moved by the user)
    bool tangentSelected = false;//true if the user's now selecting a line
    bool leftTangetSelected;//if true the left tangent is selected, else the right tangent is selected(this is used only when tangentSelected is true)

    public Texture2D textureNS;//the cursor used when draging the whole line(curve)

    bool showContextMenu = false;//true when the context menu is shown,on a curve
    bool showContextMenuKey = false;//true when the context menu on a key si shown

    Vector3 contextMenuPos;//position of the context menu
                           //context menu option size
    int menuOptionWidth;
    int menuOptionHeight;

    public Texture arrow;//texture used in the context menu of a key
    public GUIStyle contextMenuStyle;//gui style for the context menu	
    public GUIStyle selectionStyle;//gui style for showing the selected context menu option
    public GUIStyle checkedStyle;//gui style for showing the checked context menu option 

    Vector2 closestPoint = Vector2.zero; //closest point to the line from the mouse selection
    float closestPointTValue = 0;//the t value corsesponding to the closest point on the bezier curve ,and it will be used if the user proceed with
                                 //the adding of a new point

    bool middHor = true;//particular use when choosing how dense the grid horyzontal lines will be displayed
    bool middVer = true;//same for grid vertical lines

    float mZ = -1.0f;
    float mRatio;// gridHeight and mUnit ratio	
    Rect mPrevGridRect = new Rect(0, 0, 0, 0);//keeps a prev grid rect ,so to know when it's been changed	

    static float tangPixelsLength = 50f;//the length of tangents when the respective key is selected 
    public static float tangFixedLength;
    static float marginPixels = 5f;//needed when mouse selecting lines ,points,tangents...
    static float margin;//marginPixels in object coordinates	
    public static float marginNoDpi;

    const float marginErr = 1E-5f;

    int screenHeight = 0;
    float basicShapeWidthPixels = 35f;//the width of the rectangles keeping the curves basic shapes	
    float basicShapeWidth;
    float basicShapesSpacePixels = 6f;//the space in pixels between two consecutive basic shapes		
    float basicShapesSpace;
    const int shapesCount = 9;
    AnimationCurve[] basicShapes = new AnimationCurve[shapesCount];
    Rect[] basicShapesRect = new Rect[shapesCount];
    Rect normalRect = new Rect(0, 0, 1, 1);//defines basic animation curves in this rect

    //Context Menu options
    string[] options = { "Delete Key", "Auto", "Free Smooth", "Flat", "Broken", "Left Tangent", "Right Tangent", "Both Tangents" };
    string[] tangOptions = { "Free", "Linear", "Constant" };

    const int leftTangIndex = 5;
    const int rightTangIndex = 6;
    const int bothTangIndex = 7;
    int showTangOptions = -1;
    bool somethingHovered;//auxiliar member, to know when the user clicks on the context menu

    bool showCursorNormal = true;

    ContextMenuManager contextMenuManager;

    bool mWindowClosed = true;
    public bool windowClosed
    {
        set { mWindowClosed = value; }
        get { return mWindowClosed; }
    }

    enum ContextOptions
    {
        auto = 0,
        freesmooth = 1,
        broken = 2
    }
    ContextOptions lastSelectedOption = ContextOptions.freesmooth;

    bool mAlteredData = false;
    internal bool alteredData
    {
        get { return mAlteredData; }
        set { mAlteredData = value; }
    }

    void Awake()
    {
        FillListColor();
        gradRect = Rect.MinMaxRect((float)CurveStruct.xMin, (float)CurveStruct.yMin,
                                    (float)CurveStruct.xMaxim, (float)CurveStruct.yMaxim);//exchanged ymin to ymax
        crvWindow = transform.parent.Find("CurveEditor").GetComponent<CurveWindow>();
        contextMenuManager = new ContextMenuManager();
    }

    void FillListColor()
    {
        foreach (Color color in colors)
        {
            listColors.Add(color);
        }
    }

    void Start()
    {
        //CreateLineMaterial();
        //size of the menu option
        menuOptionHeight = 2 * contextMenuStyle.fontSize;
        menuOptionWidth = 150;

        //create the animation curves coresponding the basic shapes
        basicShapes[0] = new AnimationCurve();
        basicShapes[0].AddKey(0f, 0.5f);

        basicShapes[1] = new AnimationCurve();
        basicShapes[1].AddKey(0f, 0f);
        basicShapes[1].AddKey(1f, 1f);

        basicShapes[2] = new AnimationCurve();
        basicShapes[2].AddKey(0f, 1f);
        basicShapes[2].AddKey(1f, 0f);

        basicShapes[3] = new AnimationCurve();
        Keyframe keyframe = new Keyframe(0, 0);
        keyframe.outTangent = 0;
        basicShapes[3].AddKey(keyframe);
        keyframe = new Keyframe(1, 1);
        keyframe.inTangent = 2;
        basicShapes[3].AddKey(keyframe);

        basicShapes[4] = new AnimationCurve();
        keyframe = new Keyframe(0, 1);
        keyframe.outTangent = -2;
        basicShapes[4].AddKey(keyframe);
        keyframe = new Keyframe(1, 0);
        keyframe.inTangent = 0;
        basicShapes[4].AddKey(keyframe);

        basicShapes[5] = new AnimationCurve();
        keyframe = new Keyframe(0, 0);
        keyframe.outTangent = 2;
        basicShapes[5].AddKey(keyframe);
        keyframe = new Keyframe(1, 1);
        keyframe.inTangent = 0;
        basicShapes[5].AddKey(keyframe);

        basicShapes[6] = new AnimationCurve();
        keyframe = new Keyframe(0, 1);
        keyframe.outTangent = 0;
        basicShapes[6].AddKey(keyframe);
        keyframe = new Keyframe(1, 0);
        keyframe.inTangent = -2;
        basicShapes[6].AddKey(keyframe);


        basicShapes[7] = new AnimationCurve();
        keyframe = new Keyframe(0, 0);
        keyframe.outTangent = 0;
        basicShapes[7].AddKey(keyframe);
        keyframe = new Keyframe(1, 1);
        keyframe.inTangent = 0;
        basicShapes[7].AddKey(keyframe);
        basicShapes[8] = new AnimationCurve();
        keyframe = new Keyframe(0, 1);
        keyframe.outTangent = 0;
        basicShapes[8].AddKey(keyframe);
        keyframe = new Keyframe(1, 0);
        keyframe.inTangent = 0;
        basicShapes[8].AddKey(keyframe);

        Curves.dictCurvesContextMenus = contextMenuManager.dictCurvesContextMenus;
        Curves.lineMaterial = lineMaterial;
    }

    public void ShowWindow()
    {
        if (mWindowClosed)
        {
            crvWindow.windowClosed = mWindowClosed = false;//show both the curve lines and the curve window
            mAlteredData = true;
        }
    }

    public void CloseWindow()
    {
        if (!mWindowClosed)
        {
            crvWindow.windowClosed = mWindowClosed = true;
            mAlteredData = true;
        }
    }

    void UpdatePixelLengths()
    {
        if (screenHeight != Screen.height)
        {
            screenHeight = Screen.height;
            basicShapeWidth = 2f * basicShapeWidthPixels / (float)screenHeight;
            basicShapesSpace = 2f * basicShapesSpacePixels / (float)screenHeight;
            margin = 2f * marginPixels / screenHeight;
            marginNoDpi = margin;
            if (Screen.dpi != 0) margin *= (Screen.dpi / 96f);
            tangFixedLength = 2f * tangPixelsLength / (float)screenHeight;
        }
    }

    internal void UpdateCurveStruct(CurveStruct crvStruct)
    {
        CurveStruct oldCrvStruct = curveStructList.Find(x => x.curve1 == crvStruct.curve1);
        int index = curveStructList.IndexOf(oldCrvStruct);
        curveStructList.RemoveAt(index);
        curveStructList.Insert(index, crvStruct);
    }

    void UpdateCurveKeys(AnimationCurve animCurve)
    {
        float ratio = (mGradRect.height / mGradRect.width) * (mPrevGradRect.width / mPrevGradRect.height);
        for (int i = 0; i < animCurve.length; ++i)
        {
            Keyframe keyframe = animCurve[i];
            keyframe.value = (keyframe.value - mPrevGradRect.yMin) * mGradRect.height / mPrevGradRect.height + mGradRect.yMin;
            keyframe.inTangent *= ratio; keyframe.outTangent *= ratio;
            animCurve.MoveKey(i, keyframe);
        }
    }

    public void UpdateActiveCurveKeys()
    {
        if (mGradRect.width == 0f || mPrevGradRect.height == 0f) return;
        if (mActiveCurveStr.curve1 == null)
        {
            return;
        }
        UpdateCurveKeys(mActiveCurveStr.curve1);
        if (mActiveCurveStr.curve2 != null)
        {
            UpdateCurveKeys(mActiveCurveStr.curve2);
        }
    }

    /// <summary>
    /// Adds a new curve struct,with references to the given curve1(the usual case is of a single curve shown,when the curve2 is null).
    /// </param>
    public void AddCurveStruct(AnimationCurve curve1, AnimationCurve curve2)
    {
        //if there is a curve struct having the curve1,then only the second curve is updated
        CurveStruct crvStruct = curveStructList.Find(x => x.curve1 == curve1);
        bool switchToDefaultGrad = false;
        if (crvStruct.curve1 == null)
        {
            switchToDefaultGrad = (mActiveCurveStr.curve1 != null);
            crvStruct = new CurveStruct(curve1, curve2, listColors[0]);
            curveStructList.Add(crvStruct);
            listColors.RemoveAt(0);
            if (listColors.Count == 0)
            {
                FillListColor();
            }
            selectedKey = -1;
        }
        else if ((crvStruct.curve2 == null))
        {
            if (curve2 != null)
            {
                curveStructList.Remove(crvStruct);
                crvStruct.curve2 = curve2;
                curveStructList.Add(crvStruct);
            }
        }
        else if (curve2 == null)
        {
            curveStructList.Remove(crvStruct);
            crvStruct.curve2 = curve2;
            if (!crvStruct.firstCurveSelected)
            {
                selectedKey = -1;
                crvStruct.firstCurveSelected = true;
            }
            curveStructList.Add(crvStruct);
        }

        mActiveCurveStr = crvStruct;

        if (switchToDefaultGrad)
        {//just make sure to switch to default grad rect
         //(needed for the case, the active curve has a different grad rect,and for this new curve the default grad rect will be used)
            gradRect = crvStruct.gradRect;
        }
        AddContextMenuStructs(curve1);
        AddContextMenuStructs(curve2);

        mAlteredData = true;

    }

    internal void ChangeActiveCurve(AnimationCurve curve)
    {
        if (curve == mActiveCurveStr.curve1) return;
        CurveStruct crvStruct = curveStructList.Find(x => x.curve1 == curve);
        if (crvStruct.curve1 != null)
        {
            mActiveCurveStr = crvStruct;
            gradRect = mActiveCurveStr.gradRect;
        }

        mAlteredData = true;
    }

    void AddContextMenuStructs(AnimationCurve curve)
    {
        contextMenuManager.AddContextMenuStructs(curve);
    }

    /// <summary>
    /// Remove the curve struct related to the given curve.
    /// </param>
    public void RemoveCurve(AnimationCurve curve)
    {
        CurveStruct crvStruct = curveStructList.Find(x => x.curve1 == curve);
        if (crvStruct.curve1 != null)
        {
            listColors.Insert(0, crvStruct.color);
            curveStructList.Remove(crvStruct);
            contextMenuManager.Remove(curve);
        }
        if (curveStructList.Count == 0)
        {
            mActiveCurveStr = nullCurve;
            gradRect = mActiveCurveStr.gradRect;
        }
        else
        {
            UpdateActiveCurveStruct(curveStructList[0]);
        }
        selectedKey = -1;
        mAlteredData = true;
    }

    /// <summary>
    /// Replace the active curve when the user clicks on a basic shape.
    /// </param>
    void ReplaceActiveCurve(AnimationCurve curve)
    {
        if (mActiveCurveStr.curve1 != null)
        {
            AnimationCurve newCurve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;
            while (newCurve.length > 0) newCurve.RemoveKey(0);//remove all the keys
            float ratio = (mGradRect.height / mGradRect.width) * (normalRect.width / normalRect.height);
            for (int i = 0; i < curve.length; ++i)
            {
                Keyframe keyframe = curve[i];
                keyframe.value = (keyframe.value - normalRect.yMin) * mGradRect.height / normalRect.height + mGradRect.yMin;
                keyframe.time = (keyframe.time - normalRect.xMin) * mGradRect.width / normalRect.width + mGradRect.xMin;
                keyframe.inTangent *= ratio; keyframe.outTangent *= ratio;
                newCurve.AddKey(keyframe);
            }
            if (mActiveCurveStr.firstCurveSelected)
            {
                mActiveCurveStr.curve1KeysCount = newCurve.length;
            }
            else
            {
                mActiveCurveStr.curve2KeysCount = newCurve.length;
            }
            UpdateCurveStruct(mActiveCurveStr);

            contextMenuManager.UpdateContextMenuList(newCurve);

        }
        mAlteredData = true;
    }

    /// <summary>
    /// True if the given curve is added to the editor.
    /// </param>
    public bool CurveShown(AnimationCurve curve)
    {
        CurveStruct crvStruct = curveStructList.Find(x => x.curve1 == curve);
        return ((crvStruct.curve1 != null) && (crvStruct.curve2 == null));
    }

    /// <summary>
    /// True if the given curves are added as a path to the editor.
    /// </param>
    public bool CurveShown(AnimationCurve curve1, AnimationCurve curve2)
    {
        CurveStruct crvStruct = curveStructList.Find(x => x.curve1 == curve1);
        return ((crvStruct.curve1 != null) && (crvStruct.curve2 != null));
    }

    /// <summary>
    /// Here's all drawing takes place.
    /// </summary>
    void OnPostRender()
    {
        if (mWindowClosed) return;
        //lineMaterial.SetPass( 0);		
        DrawGrid();
        DrawCurves();
        DrawBasicShapes();
    }

    void DrawGrid()
    {
        if (mGradRect.height == 0f) return;

        if (mPrevGridRect != mGridRect)
        {
            //calculate how many horyzontal lines should be drawn (based of the grid size, and the gradations ranges)
            mRatio = mGridRect.height / mUnit;
            if (mGradRect.yMax > 0 && mGradRect.yMin < 0)
            {
                mHorLines = GetHorLines(mGradRect.yMax);
                mHorLines += GetHorLines(-mGradRect.yMin);
            }
            else
            {
                mHorLines = GetHorLines(mGradRect.yMax - mGradRect.yMin);
            }
            mPrevGridRect = mGridRect;
        }
        //horyzontal lines(it was tested for gradations that ranges from ymin = 0(or '-ymax') to ymax = 'posivitve value' )		
        float gradation;
        mRows.Clear();

        float gridRectHeight = (1f - mRezid) * mGridRect.height;
        float sample = gridRectHeight / mHorLines;
        float alpha = (mRatio / mHorLines - 0.2f) / (1 - 0.2f);//the intermediate lines are more transparent

        int zeroline = Mathf.RoundToInt((-mGradRect.yMin / mGradRect.height) * mHorLines);//should be zero, if the yMin is 0,if -yMin == yMax , the zeroline should be the middle 	

        for (int i = 0; i <= mHorLines; i++)
        {
            if (Mathf.Abs(i - zeroline) % (middHor ? 5 : 2) == 0 || i == mHorLines || i == 0)
            {
                lineColor.a = 1.0f;
            }
            else
            {
                lineColor.a = alpha;
            }
            GL.Begin(GL.LINES);
            lineMaterial.color = lineColor;
            lineMaterial.SetPass(0);
            //GL.Color(lineColor);
            gradation = mGridRect.yMin * (1f - mRezid) + i * sample;
            GL.Vertex3(mGridRect.xMin, gradation, mZ);
            GL.Vertex3(mGridRect.xMax, gradation, mZ);
            GL.End();

            if (lineColor.a > 0.3f)
            {
                //display the gradation coresponding to this line
                mRows[gradation] = (decimal)((float)i * (1f - mRezid) / mHorLines);
            }
        }
        if (Mathf.Abs(mRezid) > 1E-7)
        {
            lineColor.a = 1.0f;
            GL.Begin(GL.LINES);
            lineMaterial.color = lineColor;
            lineMaterial.SetPass(0);
            //GL.Color(lineColor);
            gradation = mGridRect.yMax;
            GL.Vertex3(mGridRect.xMin, gradation, mZ);
            GL.Vertex3(mGridRect.xMax, gradation, mZ);
            GL.End();
            mRows[gradation] = 1;
        }

        if (zeroline > 0)
        {
            if (Mathf.Abs(mRezid) > 1E-7)
            {
                lineColor.a = 1.0f;
                GL.Begin(GL.LINES);
                lineMaterial.color = lineColor;
                lineMaterial.SetPass(0);
                //GL.Color(lineColor);
                gradation = mGridRect.yMin;
                GL.Vertex3(mGridRect.xMin, gradation, mZ);
                GL.Vertex3(mGridRect.xMax, gradation, mZ);
                GL.End();
            }
        }

        //vertical lines(it was tested only with default values for xMin and xMax)
        float ratio = mGridRect.width / mUnit;
        int lines = 1;
        middVer = true;
        while (ratio >= lines)
        {
            lines *= (middVer ? 5 : 2);
            middVer = !middVer;
        }
        mColls = lines;
        alpha = (ratio / lines - 0.2f) / (1 - 0.2f);
        if (alpha < 0.5f)
        {
            mColls /= (!middVer ? 5 : 2);//it's time to show the line number? 
        }

        for (int i = 0; i <= lines; i++)
        {
            if (i % (middVer ? 2 : 5) == 0)
            {
                lineColor.a = 1.0f;
            }
            else
            {
                lineColor.a = alpha;
            }
            GL.Begin(GL.LINES);
            lineMaterial.color = lineColor;
            lineMaterial.SetPass(0);
            //GL.Color(lineColor);
            gradation = mGridRect.x + i * (mGridRect.width / lines);
            GL.Vertex3(gradation, mGridRect.yMin, mZ);
            GL.Vertex3(gradation, mGridRect.yMax, mZ);
            GL.End();
        }
    }

    void DrawBasicShapes()
    {
        if (mGradRect.height == 0f) return;
        Color color = new Color(0.45f, 0.45f, 0.45f, 0.5f);
        Color light = new Color(0.75f, 0.75f, 0.75f, 0.8f);
        UpdatePixelLengths();

        float alignMiddle = 0f;
        if (mGridRect.width > shapesCount * (basicShapeWidth + basicShapesSpace))
        {
            alignMiddle = mGridRect.width * 0.5f - (shapesCount * 0.5f * (basicShapeWidth + basicShapesSpace));
        }
        for (int i = 0; i < 9; ++i)
        {
            float shapeMin = (basicShapeWidth + basicShapesSpace) * i;
            if (mGridRect.xMin + shapeMin > mGridRect.xMax)
            {
                break;
            }

            float shapeMax = basicShapeWidth + shapeMin;

            if (mGridRect.xMin + shapeMax > mGridRect.xMax)
            {
                shapeMax = mGridRect.xMax - mGridRect.xMin;
            }
            Rect shapeRect = Rect.MinMaxRect(mGridRect.xMin + shapeMin + alignMiddle, mWindowBottom.yMax,
                                            mGridRect.xMin + shapeMax + alignMiddle, mWindowBottom.yMin);

            GL.Begin(GL.QUADS);
            //GL.Color(color);
            lineMaterial.color = color;
            lineMaterial.SetPass(0);
            GL.Vertex3(shapeRect.xMin, mWindowBottom.yMin, mZ);
            GL.Vertex3(shapeRect.xMin, mWindowBottom.yMax, mZ);
            GL.Vertex3(shapeRect.xMax, mWindowBottom.yMax, mZ);
            GL.Vertex3(shapeRect.xMax, mWindowBottom.yMin, mZ);
            GL.End();

            float clip = shapeRect.width;
            shapeRect.xMax = mGridRect.xMin + basicShapeWidth + shapeMin + alignMiddle;
            if (clip < shapeRect.width)
            {
                clip = clip / shapeRect.width;
            }
            else
            {
                clip = 1f;
            }
            basicShapesRect[i] = shapeRect;
            Curves.DrawCurve(light, basicShapes[i], null, false, false, -1, shapeRect, normalRect, true, clip);
        }
    }


    /// <summary>
    /// Distance from one point to a given animation curve(in the given gradiations ranges) .
    /// </summary>
    /// <returns>
    /// The square of the distance.
    /// </returns>
    float pointLineDist(Vector2 point, AnimationCurve animCurve, Rect curveGradRect)
    {
        Vector2 keyframeWorld = Convert(new Vector2(animCurve[0].time, animCurve[0].value), mGridRect, curveGradRect);
        if (keyframeWorld.x > point.x)
        {
            closestPoint = point;
            float dist = Mathf.Abs(keyframeWorld.y - point.y);
            return dist * dist;
        }
        keyframeWorld = Convert(new Vector2(animCurve[animCurve.length - 1].time, animCurve[animCurve.length - 1].value), mGridRect, curveGradRect);
        if (keyframeWorld.x < point.x)
        {
            closestPoint = point;
            float dist = Mathf.Abs(keyframeWorld.y - point.y);
            return dist * dist;
        }
        if (animCurve.length > 1)
        {
            for (int i = 0; i < animCurve.length - 1; ++i)
            {
                keyframeWorld = Convert(new Vector2(animCurve[i + 1].time, animCurve[i + 1].value), mGridRect, curveGradRect);
                if (point.x < keyframeWorld.x)
                {
                    Vector2 p1 = new Vector2(animCurve[i].time, animCurve[i].value);
                    p1 = CurveLines.Convert(p1, mGridRect, curveGradRect);
                    Vector2 p2 = keyframeWorld;

                    if (contextMenuManager.dictCurvesContextMenus[animCurve][i].rightTangent.constant ||
                       contextMenuManager.dictCurvesContextMenus[animCurve][i + 1].leftTangent.constant ||
                       animCurve[i].outTangent == float.PositiveInfinity || animCurve[i + 1].inTangent == float.PositiveInfinity)
                    {
                        float dist = Mathf.Abs(p1.y - point.y);
                        if ((p1.y < p2.y && p1.y <= point.y && point.y <= p2.y) ||
                            (p1.y > p2.y && p1.y >= point.y && point.y >= p2.y))
                        {
                            float distX = Mathf.Abs(p2.x - point.x);
                            if (distX < dist) dist = distX;
                        }
                        closestPoint = point;
                        return dist * dist;
                    }
                    else
                    {
                        Vector2 c1 = Vector2.zero;
                        Vector2 c2 = Vector2.zero;
                        float ratio = (mGridRect.height / mGridRect.width) * (curveGradRect.width / curveGradRect.height);
                        Curves.GetControlPoints(p1, p2, animCurve[i].outTangent * ratio, animCurve[i + 1].inTangent * ratio, out c1, out c2);

                        return PointBezier.DistPointToBezier(point, p1, c1, c2, p2, out closestPoint, out closestPointTValue);
                    }
                }
            }
        }
        //shouldn't get here
        return Mathf.Infinity;
    }

    void UpdateActiveCurveStruct(CurveStruct crvStruct)
    {
        if (mActiveCurveStr.curve1 == crvStruct.curve1)
        {
            mActiveCurveStr.firstCurveSelected = crvStruct.firstCurveSelected;
            mAlteredData = true;
            return;
        }
        mActiveCurveStr = crvStruct;
        this.gradRect = mActiveCurveStr.gradRect;
        mAlteredData = true;
    }


    /// <summary>
    /// Checks if the user wanna drag a tangent of the selected key
    /// </param>
    bool CheckMouseTangentSelection(Vector2 mousePos)
    {
        if (selectedKey == -1)
        {
            return false;
        }

        AnimationCurve curve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;
        if (crvWindow.IsTouched || Input.GetMouseButtonDown(0))
        {
            float ratio = (mGridRect.height / mGridRect.width) * (mGradRect.width / mGradRect.height);
            Vector2 keyframeWorld = Convert(new Vector2(curve[selectedKey].time, curve[selectedKey].value), mGridRect, mActiveCurveStr.gradRect);
            if (curve.length - selectedKey > 1)
            {
                float tangOut = curve[selectedKey].outTangent;
                float tangOutScaled = Mathf.Atan(tangOut * ratio);
                Vector2 tangPeak = new Vector2(keyframeWorld.x + tangFixedLength * Mathf.Cos(tangOutScaled),
                                                keyframeWorld.y + tangFixedLength * Mathf.Sin(tangOutScaled));
                if (Vector2.SqrMagnitude(tangPeak - mousePos) <= margin * margin)
                {
                    tangentSelected = true;
                    leftTangetSelected = false;
                    return true;
                }
            }
            if (selectedKey > 0)
            {
                float tangIn = curve[selectedKey].inTangent;
                float tangInScaled = Mathf.Atan(tangIn * ratio);
                Vector2 tangPeak = new Vector2(keyframeWorld.x - tangFixedLength * Mathf.Cos(tangInScaled),
                        keyframeWorld.y - tangFixedLength * Mathf.Sin(tangInScaled));
                if (Vector2.SqrMagnitude(tangPeak - mousePos) <= margin * margin)
                {
                    tangentSelected = true;
                    leftTangetSelected = true;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Checks what the user clicks(selects),a tangent,a key or a whole curve. 
    /// </summary>
    bool CheckMouseSelection(Vector2 mousePos, CurveStruct crvStruct)
    {
        int i;
        List<AnimationCurve> curves = new List<AnimationCurve>();
        curves.Add(crvStruct.curve1);
        if (crvStruct.curve2 != null)
        {
            curves.Add(crvStruct.curve2);
        }

        foreach (AnimationCurve curve in curves)
        {
            for (i = 0; i < curve.length; ++i)
            {
                Vector2 keyframeWorld = Convert(new Vector2(curve[i].time, curve[i].value), mGridRect, crvStruct.gradRect);

                if (Vector2.SqrMagnitude(keyframeWorld - mousePos) <= margin * margin)
                {
                    if (crvWindow.IsDoubleTap() || Input.GetMouseButton(1))
                    {
                        showContextMenuKey = true;
                        contextMenuPos = crvWindow.CursorPos();
                    }
                    else if (crvWindow.IsSingleTap() || Input.GetMouseButton(0))
                    {
                        keySelected = true;
                    }

                    selectedKey = i;
                    crvStruct.firstCurveSelected = (crvStruct.curve1 == curve);
                    UpdateCurveStruct(crvStruct);
                    UpdateActiveCurveStruct(crvStruct);
                    break;
                }
            }
            if (i == curve.length)
            {
                if (pointLineDist(new Vector2(mousePos.x, mousePos.y), curve, crvStruct.gradRect) <= margin * margin)
                {
                    if (crvWindow.IsDoubleTap() || Input.GetMouseButton(1))
                    {
                        showContextMenu = true;
                        contextMenuPos = crvWindow.CursorPos();
                    }
                    else if (crvWindow.IsTouched || Input.GetMouseButton(0))
                    {
                        lineSelected = true;
                        showCursorNormal = false;
#if UNITY_3_5 || UNITY_EDITOR_OSX
						Screen.showCursor = false;
#else
                        Cursor.SetCursor(textureNS, CurveWindow.hotspot, CursorMode.ForceSoftware);
#endif
                    }

                    crvStruct.firstCurveSelected = (crvStruct.curve1 == curve);
                    UpdateCurveStruct(crvStruct);
                    UpdateActiveCurveStruct(crvStruct);
                    selectedKey = -1;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// On each update,check if the mouse is clicked, if so check what(a basic shape, a tangent,key or a whole line).
    /// Also make updates of the mouse context menus list ,when new keys are added/deleted
    /// </summary>
    void LateUpdate()
    {
        //check if new keys have been added/deleted outside of curve
        if (mActiveCurveStr.curve1 != null)
        {
            if (mActiveCurveStr.firstCurveSelected ? (mActiveCurveStr.curve1.length != mActiveCurveStr.curve1KeysCount) :
                (mActiveCurveStr.curve2.length != mActiveCurveStr.curve2KeysCount))
            {
                selectedKey = -1;//just be sure that selected key is not an out of range key
                UpdateCurveKeys(mActiveCurveStr.curve1);
            }
        }
        //update the list of context menus ,when key has been added/deleted
        for (int i = 0; i < curveStructList.Count; ++i)
        {
            CurveStruct crvStruct = curveStructList[i];
            if (crvStruct.curve1.length != crvStruct.curve1KeysCount)
            {
                contextMenuManager.UpdateDictContextMenu(crvStruct.curve1, crvStruct.curve1.length - crvStruct.curve1KeysCount);
                crvStruct.curve1KeysCount = crvStruct.curve1.length;
                curveStructList.RemoveAt(i);
                curveStructList.Insert(i, crvStruct);
                if (crvStruct.curve1 == mActiveCurveStr.curve1) mActiveCurveStr = crvStruct;

                mAlteredData = true;
            }
            if (crvStruct.curve2 != null && crvStruct.curve2.length != crvStruct.curve2KeysCount)
            {
                contextMenuManager.UpdateDictContextMenu(crvStruct.curve2, crvStruct.curve2.length - crvStruct.curve2KeysCount);
                crvStruct.curve2KeysCount = crvStruct.curve2.length;
                curveStructList.RemoveAt(i);
                curveStructList.Insert(i, crvStruct);
                if (crvStruct.curve2 == mActiveCurveStr.curve2) mActiveCurveStr = crvStruct;

                mAlteredData = true;
            }
        }

        //now check user interaction
        if ((!crvWindow.IsTouchedBegan() && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) || somethingHovered) return;

        Vector2 mousePos = GetComponent<Camera>().ScreenToWorldPoint(crvWindow.CursorPos());

        //check first if the user tries to drag the tangent of the selected key (these should be selectable even if they are outside of the grid)
        if (CheckMouseTangentSelection(mousePos)) return;

        for (int i = 0; i < shapesCount; ++i)
        {
            if (basicShapesRect[i].Contains(mousePos))
            {
                ReplaceActiveCurve(basicShapes[i]);
                selectedKey = -1;
                break;
            }
        }

        showContextMenu = false; showContextMenuKey = false;
        if (mActiveCurveStr.curve1 != null &&
               (mGridRect.xMin - margin < mousePos.x) && (mGridRect.xMax + margin > mousePos.x) &&
            (mGridRect.yMin - margin < mousePos.y) && (mGridRect.yMax + margin > mousePos.y))
        {
            if (!CheckMouseSelection(mousePos, mActiveCurveStr))
            {
                foreach (CurveStruct crvStruct in curveStructList)
                {
                    if (crvStruct.curve1 == mActiveCurveStr.curve1) continue;
                    if (CheckMouseSelection(mousePos, crvStruct))
                    {
                        break;
                    }
                }
            }
        }
    }



    /// <summary>
    /// Add a key at a given position.
    /// </summary>
    void AddKey(Vector3 mousePosWorld)
    {
        Vector2 mousePosGrad = Convert(new Vector2(mousePosWorld.x, mousePosWorld.y), mGradRect, mGridRect);
        AnimationCurve activeCurve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;
        List<ContextMenuStruct> listContextMenus = contextMenuManager.dictCurvesContextMenus[activeCurve];
        ContextMenuStruct contextMenuStruct = new ContextMenuStruct();
        switch (lastSelectedOption)
        {
            case ContextOptions.freesmooth: contextMenuStruct.freeSmooth = true; break;
            case ContextOptions.auto: contextMenuStruct.auto = true; break;
            case ContextOptions.broken:
                contextMenuStruct.broken = true;
                contextMenuStruct.leftTangent.free = true;
                contextMenuStruct.rightTangent.free = true;
                contextMenuStruct.bothTangents.free = true;
                break;
        }

        if (mousePosGrad.x < activeCurve[0].time || mousePosGrad.x > activeCurve[activeCurve.length - 1].time)
        {

            Keyframe keyframeNeighbour = (mousePosGrad.x < activeCurve[0].time) ? activeCurve[0] : activeCurve[activeCurve.length - 1];

            selectedKey = activeCurve.AddKey(mousePosGrad.x, mousePosGrad.y);

            if (activeCurve.length > 2)
            {
                activeCurve.MoveKey((mousePosGrad.x < activeCurve[1].time) ? 1 : activeCurve.length - 2,
                                keyframeNeighbour); //hack needed( wierd:when a key is added in the clamped area,the neighbour's key changes its tangents...)
            }

            Keyframe keyframe = activeCurve[selectedKey];
            keyframe.inTangent = keyframe.outTangent = 0;

            if (contextMenuStruct.freeSmooth)
            {
                contextMenuStruct.flat = true;
            }
            listContextMenus.Insert(selectedKey, contextMenuStruct);
        }
        else
        {
            for (int i = 0; i < activeCurve.length - 1; ++i)
            {
                if (mousePosGrad.x > activeCurve[i].time && mousePosGrad.x < activeCurve[i + 1].time)
                {
                    Keyframe keyframe = new Keyframe(mousePosGrad.x, mousePosGrad.y);
                    if (listContextMenus[i].rightTangent.constant || listContextMenus[i + 1].leftTangent.constant ||
                       activeCurve[i].outTangent == float.PositiveInfinity || activeCurve[i + 1].inTangent == float.PositiveInfinity)
                    {
                        keyframe.inTangent = 0;
                        keyframe.outTangent = 0;
                        contextMenuStruct.freeSmooth = true;
                    }
                    else
                    {
                        Vector2 val = new Vector2(activeCurve[i].time, activeCurve[i].value);
                        val = CurveLines.Convert(val, mGridRect, mGradRect);
                        Vector2 val2 = new Vector2(activeCurve[i + 1].time, activeCurve[i + 1].value);
                        val2 = CurveLines.Convert(val2, mGridRect, mGradRect);
                        float tangOut = activeCurve[i].outTangent;
                        float tangIn = activeCurve[i + 1].inTangent;
                        float ratio = (mGridRect.height / mGridRect.width) * (mGradRect.width / mGradRect.height);
                        Vector2 c1 = Vector2.zero;
                        Vector2 c2 = Vector2.zero;
                        Curves.GetControlPoints(val, val2, tangOut * ratio, tangIn * ratio, out c1, out c2);

                        float t = closestPointTValue;
                        //de Casteljau's algorithm for dividing a bezier curve
                        Vector2 p00 = (1 - t) * val + t * c1;
                        Vector2 p11 = (1 - t) * c1 + t * c2;
                        Vector2 p22 = (1 - t) * c2 + t * val2;
                        Vector2 newC2 = (1 - t) * p00 + t * p11;
                        Vector2 newC1 = (1 - t) * p11 + t * p22;

                        //got the control points ,now find the tangents for the new point
                        Curves.GetTangents(val, closestPoint, c1, newC2, out tangOut, out tangIn);
                        tangIn /= ratio;
                        keyframe.inTangent = -tangIn;
                        Curves.GetTangents(closestPoint, val2, newC1, c2, out tangOut, out tangIn);
                        tangOut /= ratio;

                        keyframe.outTangent = tangOut;
                    }

                    selectedKey = activeCurve.AddKey(keyframe);

                    if (contextMenuStruct.freeSmooth && ContextMenuManager.IsKeyframeFlat(keyframe))
                    {
                        contextMenuStruct.flat = true;
                    }
                    listContextMenus.Insert(selectedKey, contextMenuStruct);
                    break;
                }
            }
        }

        if (contextMenuStruct.auto)
        {
            UpdateAutoTangents(activeCurve, selectedKey);
        }

        //update neighbours if they are auto	
        if (selectedKey > 0)
        {
            if (listContextMenus[selectedKey - 1].auto)
            {
                UpdateAutoTangents(activeCurve, selectedKey - 1);
            }
            //update the neighbour if it is linear in this direction
            if (listContextMenus[selectedKey - 1].leftTangent.linear)
            {
                UpdateLinearTangent(activeCurve, selectedKey - 1, false);
            }
        }
        if (selectedKey < activeCurve.keys.Length - 1)
        {
            if (listContextMenus[selectedKey + 1].auto)
            {
                UpdateAutoTangents(activeCurve, selectedKey + 1);
            }
            //update the neighbour if it is linear on this direction
            if (listContextMenus[selectedKey + 1].rightTangent.linear)
            {
                UpdateLinearTangent(activeCurve, selectedKey + 1, true);
            }
        }

        if (mActiveCurveStr.firstCurveSelected)
        {
            mActiveCurveStr.curve1KeysCount += 1;
        }
        else
        {
            mActiveCurveStr.curve2KeysCount += 1;
        }
        UpdateCurveStruct(mActiveCurveStr);

        mAlteredData = true;
    }

    /// <summary>
    /// Deletes the key(normally only by context menu, but in some rare cases,the key is deleted when the user drags it outside of interval).
    /// </summary>
    void DeleteKey(bool byContextMenu = true)
    {
        AnimationCurve activeCurve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;
        if (activeCurve.keys.Length == 1 && byContextMenu) return;

        List<ContextMenuStruct> listContextMenus = contextMenuManager.dictCurvesContextMenus[activeCurve];

        if (byContextMenu) activeCurve.RemoveKey(selectedKey);
        if (selectedKey > 0)
        {
            //update neighbours if they are auto
            if (listContextMenus[selectedKey - 1].auto)
            {
                UpdateAutoTangents(activeCurve, selectedKey - 1);
            }
            //update the neighbour if is linear on this direction
            if (listContextMenus[selectedKey - 1].leftTangent.linear)
            {
                UpdateLinearTangent(activeCurve, selectedKey - 1, false);
            }
        }

        if (selectedKey < activeCurve.keys.Length)//this condition should always be true
        {
            if (listContextMenus[selectedKey].auto)
            {//next key ,is equal with selectedKey(as the selectedKey has been already removed)
                UpdateAutoTangents(activeCurve, selectedKey);
            }
            //update the neighbour if is linear on this direction
            if (listContextMenus[selectedKey].rightTangent.linear)
            {
                UpdateLinearTangent(activeCurve, selectedKey, true);
            }
        }

        listContextMenus.RemoveAt(selectedKey);
        selectedKey = -1;
        keySelected = false;
        if (mActiveCurveStr.firstCurveSelected)
        {
            mActiveCurveStr.curve1KeysCount -= 1;
        }
        else
        {
            mActiveCurveStr.curve2KeysCount -= 1;
        }
        UpdateCurveStruct(mActiveCurveStr);

        mAlteredData = true;
    }

    /// <summary>
    /// Show the context menu.
    /// </summary>
    void OnGUI()
    {
        if (mWindowClosed) return;
        if (showContextMenu || showContextMenuKey)
        {
            somethingHovered = false;
            GUI.depth = -2;
            Rect rect = new Rect(contextMenuPos.x, Screen.height - contextMenuPos.y, menuOptionWidth, menuOptionHeight);
            if (showContextMenu)
            {
                if (GUI.Button(rect, "Add Key", contextMenuStyle))
                {
                    AddKey(closestPoint);
                    showContextMenu = false;
                    somethingHovered = false;
                }
                if (showContextMenu) ShowHover(rect);
            }
            else if (showContextMenuKey)
            {
                AnimationCurve activeCurve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;
                List<ContextMenuStruct> listContextMenus = contextMenuManager.dictCurvesContextMenus[activeCurve];
                ContextMenuStruct contextMenuStr = new ContextMenuStruct();
                contextMenuStr = listContextMenus[selectedKey];

                bool updateContextMenuDict = false;
                for (int i = 0; i < options.Length; ++i)
                {
                    if (GUI.Button(rect, options[i], contextMenuStyle) && (i < leftTangIndex))
                    {
                        showContextMenuKey = false;
                        somethingHovered = false;

                        if (i == 0)
                        {
                            DeleteKey();
                        }
                        else if (i == 1 && !contextMenuStr.auto)
                        {
                            contextMenuStr.Reset();
                            contextMenuStr.auto = true;
                            updateContextMenuDict = true;

                            if (activeCurve.keys.Length > 0)
                            {
                                UpdateAutoTangents(activeCurve, selectedKey);
                            }

                            lastSelectedOption = ContextOptions.auto;
                        }
                        else if (i == 2 && !contextMenuStr.freeSmooth)
                        {
                            contextMenuStr.Reset();
                            contextMenuStr.freeSmooth = true;
                            updateContextMenuDict = true;

                            Keyframe keyframe = activeCurve.keys[selectedKey];
                            float outTangRad = Mathf.Atan(keyframe.outTangent);
                            float inTangRad = Mathf.Atan(keyframe.inTangent);
                            float diff = Mathf.Abs(outTangRad - inTangRad);
                            if (outTangRad > inTangRad)
                            {
                                outTangRad -= diff / 2f; inTangRad += diff / 2f;
                            }
                            else
                            {
                                outTangRad += diff / 2f; inTangRad -= diff / 2f;
                            }
                            keyframe.inTangent = Mathf.Tan(inTangRad);
                            keyframe.outTangent = Mathf.Tan(outTangRad);
                            activeCurve.MoveKey(selectedKey, keyframe);

                            lastSelectedOption = ContextOptions.freesmooth;
                        }
                        else if (i == 3 && !contextMenuStr.flat)
                        {
                            contextMenuStr.Reset();
                            contextMenuStr.freeSmooth = true;
                            contextMenuStr.flat = true;
                            updateContextMenuDict = true;

                            Keyframe keyframe = activeCurve.keys[selectedKey];
                            keyframe.inTangent = keyframe.outTangent = 0;
                            activeCurve.MoveKey(selectedKey, keyframe);

                            lastSelectedOption = ContextOptions.freesmooth;
                        }
                        else if (i == 4 && !contextMenuStr.broken)
                        {
                            contextMenuStr.Reset();
                            contextMenuStr.broken = true;
                            updateContextMenuDict = true;

                            lastSelectedOption = ContextOptions.broken;

                            contextMenuStr.leftTangent.free = true;
                            contextMenuStr.rightTangent.free = true;
                            contextMenuStr.bothTangents.free = true;
                        }
                    }

                    if (i >= leftTangIndex)
                    {
                        GUI.Label(Rect.MinMaxRect(rect.xMax - 16, rect.center.y - 8, rect.xMax, rect.center.y + 8), arrow);
                    }
                    if (showContextMenuKey) ShowHover(rect, showTangOptions == i);

                    Rect rectChecked = Rect.MinMaxRect(rect.xMin + 3, rect.yMin + 3, rect.xMin + 0.75f * contextMenuStyle.padding.left - 3, rect.yMax - 3);

                    if (i >= leftTangIndex)
                    {
                        Vector3 cursorPos = crvWindow.CursorPos();
                        if (rect.Contains(new Vector3(cursorPos.x, Screen.height - cursorPos.y))
                            || (showTangOptions == i))
                        {
                            showTangOptions = i;
                            Rect rect2 = rect;
                            rect2.x = rect2.xMax - 5;
                            rect2.width /= 1.5f;
                            for (int j = 0; j < tangOptions.Length; ++j)
                            {
                                if (GUI.Button(rect2, tangOptions[j], contextMenuStyle))
                                {
                                    showContextMenuKey = false;
                                    somethingHovered = false;
                                    if (!contextMenuStr.broken)
                                    {
                                        contextMenuStr.Reset();
                                        contextMenuStr.broken = true;
                                        contextMenuStr.bothTangents.free = true;
                                        contextMenuStr.leftTangent.free = true;
                                        contextMenuStr.rightTangent.free = true;
                                    }
                                    if (i == leftTangIndex) contextMenuStr.leftTangent.Reset();
                                    if (i == rightTangIndex) contextMenuStr.rightTangent.Reset();
                                    if (i == bothTangIndex)
                                    {
                                        contextMenuStr.leftTangent.Reset();
                                        contextMenuStr.rightTangent.Reset();
                                    }
                                    contextMenuStr.bothTangents.Reset();
                                    if (j == 0)
                                    {
                                        if (i == leftTangIndex)
                                        {
                                            contextMenuStr.leftTangent.free = true;
                                            contextMenuStr.bothTangents.free = contextMenuStr.rightTangent.free;
                                        }
                                        else if (i == rightTangIndex)
                                        {
                                            contextMenuStr.rightTangent.free = true;
                                            contextMenuStr.bothTangents.free = contextMenuStr.leftTangent.free;
                                        }
                                        else if (i == bothTangIndex)
                                        {
                                            contextMenuStr.bothTangents.free = true;
                                            contextMenuStr.leftTangent.free = true;
                                            contextMenuStr.rightTangent.free = true;
                                        }

                                    }
                                    else if (j == 1)
                                    {
                                        if (i == leftTangIndex)
                                        {
                                            contextMenuStr.leftTangent.linear = true;
                                            contextMenuStr.bothTangents.linear = contextMenuStr.rightTangent.linear;
                                        }
                                        else if (i == rightTangIndex)
                                        {
                                            contextMenuStr.rightTangent.linear = true;
                                            contextMenuStr.bothTangents.linear = contextMenuStr.leftTangent.linear;
                                        }
                                        else if (i == bothTangIndex)
                                        {
                                            contextMenuStr.bothTangents.linear = true;
                                            contextMenuStr.leftTangent.linear = true;
                                            contextMenuStr.rightTangent.linear = true;
                                        }
                                        Keyframe keyframe = activeCurve.keys[selectedKey];
                                        if (contextMenuStr.leftTangent.linear && selectedKey > 0)
                                        {
                                            Keyframe keyframePrev = activeCurve.keys[selectedKey - 1];
                                            keyframe.inTangent = (keyframePrev.value - keyframe.value) / (keyframePrev.time - keyframe.time);
                                        }
                                        if (contextMenuStr.rightTangent.linear && (selectedKey < activeCurve.keys.Length - 1))
                                        {
                                            Keyframe keyframeNext = activeCurve.keys[selectedKey + 1];
                                            keyframe.outTangent = (keyframeNext.value - keyframe.value) / (keyframeNext.time - keyframe.time);
                                        }
                                        activeCurve.MoveKey(selectedKey, keyframe);
                                    }
                                    else if (j == 2)
                                    {
                                        if (i == leftTangIndex)
                                        {
                                            contextMenuStr.leftTangent.constant = true;
                                            contextMenuStr.bothTangents.constant = contextMenuStr.rightTangent.constant;
                                        }
                                        else if (i == rightTangIndex)
                                        {
                                            contextMenuStr.rightTangent.constant = true;
                                            contextMenuStr.bothTangents.constant = contextMenuStr.leftTangent.constant;
                                        }
                                        else if (i == bothTangIndex)
                                        {
                                            contextMenuStr.bothTangents.constant = true;
                                            contextMenuStr.leftTangent.constant = true;
                                            contextMenuStr.rightTangent.constant = true;
                                        }
                                        Keyframe keyframe = activeCurve.keys[selectedKey];
                                        if (contextMenuStr.leftTangent.constant && selectedKey > 0)
                                        {
                                            keyframe.inTangent = float.PositiveInfinity;
                                        }
                                        if (contextMenuStr.rightTangent.constant && (selectedKey < activeCurve.keys.Length - 1))
                                        {
                                            keyframe.outTangent = float.PositiveInfinity;
                                        }
                                        activeCurve.MoveKey(selectedKey, keyframe);
                                    }

                                    updateContextMenuDict = true;
                                }

                                if (showContextMenuKey) ShowHover(rect2);


                                Rect rectChecked2 = Rect.MinMaxRect(rect2.xMin + 3, rect2.yMin + 3, rect2.xMin + 0.75f * contextMenuStyle.padding.left - 3, rect2.yMax - 3);
                                if ((i == leftTangIndex && ((j == 0 && contextMenuStr.leftTangent.free) ||
                                                           (j == 1 && contextMenuStr.leftTangent.linear) ||
                                                           (j == 2 && contextMenuStr.leftTangent.constant))) ||
                                    (i == rightTangIndex && ((j == 0 && contextMenuStr.rightTangent.free) ||
                                                            (j == 1 && contextMenuStr.rightTangent.linear) ||
                                                            (j == 2 && contextMenuStr.rightTangent.constant))) ||
                                    (i == bothTangIndex && ((j == 0 && contextMenuStr.bothTangents.free) ||
                                                            (j == 1 && contextMenuStr.bothTangents.linear) ||
                                                            (j == 2 && contextMenuStr.bothTangents.constant))))
                                {
                                    GUI.Label(rectChecked2, "", checkedStyle);
                                }
                                rect2.y += menuOptionHeight;
                            }
                        }
                    }
                    else
                    {
                        Vector3 cursorPos = crvWindow.CursorPos();
                        if (rect.Contains(new Vector3(cursorPos.x, Screen.height - cursorPos.y)))
                        {
                            showTangOptions = -1;
                        }
                    }

                    if ((i == 1 && contextMenuStr.auto) || (i == 2 && contextMenuStr.freeSmooth) ||
                        (i == 3 && contextMenuStr.flat) || (i == 4 && contextMenuStr.broken))
                    {
                        GUI.Label(rectChecked, "", checkedStyle);
                    }

                    rect.y += menuOptionHeight;
                    if (i == 0 || i == 4) rect.y += 1;
                }
                if (updateContextMenuDict)
                {
                    listContextMenus[selectedKey] = contextMenuStr;
                }
            }
        }
#if UNITY_3_5 || UNITY_EDITOR_OSX
		if(!showCursorNormal)
		{
			Vector3 mousePos = crvWindow.CursorPos();
			mousePos.y = Screen.height - mousePos.y;
			Rect rect = Rect.MinMaxRect(mousePos.x - CurveWindow.hotspot.x,mousePos.y + CurveWindow.hotspot.y,
										mousePos.x + CurveWindow.hotspot.x,mousePos.y - CurveWindow.hotspot.y);
			GUI.DrawTexture(rect,textureNS);
		}
#endif
    }

    /// <summary>
    /// Checks if the mouse is hovering any context menu option(given by its rect).
    /// </summary>
    void ShowHover(Rect rect, bool showAlways = false)
    {
        Vector3 pos = crvWindow.CursorPos();
        bool hovered = rect.Contains(new Vector3(pos.x, Screen.height - pos.y));
        if (showAlways || hovered)
        {
            Rect rectSelection = Rect.MinMaxRect(rect.xMin + 3, rect.yMin + 3, rect.xMax - 3, rect.yMax - 3);
            GUI.Label(rectSelection, "", selectionStyle);
        }
        somethingHovered |= hovered;
    }

    public void MouseUp()
    {
        ResetSelections();
    }
    void ResetSelections()
    {
        if (lineSelected)
        {
            lineSelected = false;
            showCursorNormal = true;
#if UNITY_3_5 || UNITY_EDITOR_OSX
			Screen.showCursor = true;
#else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
#endif
        }
        else if (keySelected)
        {
            keySelected = false;
        }
        else if (tangentSelected)
        {
            tangentSelected = false;
        }

    }

    void MoveKeyframe(int index, Vector3 diff)
    {
        AnimationCurve activeCurve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;

        Vector2 keyframeWorld = Convert(new Vector2(activeCurve[index].time, activeCurve[index].value), mGridRect, mGradRect);
        keyframeWorld.x += diff.x;
        keyframeWorld.y += diff.y;

        Vector2 keyframeGrad = Convert(keyframeWorld, mGradRect, mGridRect);
        Keyframe keyframe = activeCurve[index];
        keyframe.time = keyframeGrad.x;
        keyframe.value = keyframeGrad.y;

        int newIndex = activeCurve.MoveKey(index, keyframe);

        if ((newIndex < 0) || (newIndex > activeCurve.length - 1))
        {
            DeleteKey(false);
        }
        else if (newIndex != index)
        {
            //the order of two consecutive keys has just been reversed(so reverse the order in the list of the context menus)
            selectedKey = newIndex;
            contextMenuManager.dictCurvesContextMenus[activeCurve].Reverse((index < newIndex) ? index : newIndex, 2);
        }
    }

    void UpdateAutoTangents(AnimationCurve curve, int selectedKey)
    {
        Keyframe keyframe = curve.keys[selectedKey];

        if (selectedKey > 0 && (selectedKey < curve.keys.Length - 1))
        {
            Keyframe keyframePrev = curve.keys[selectedKey - 1];
            Keyframe keyframeNext = curve.keys[selectedKey + 1];
            float tangPrev = (keyframe.value - keyframePrev.value) / (keyframe.time - keyframePrev.time);
            float tangNext = (keyframe.value - keyframeNext.value) / (keyframe.time - keyframeNext.time);
            keyframe.inTangent = (tangPrev + tangNext) / 2.0f;
            keyframe.outTangent = keyframe.inTangent;

        }
        else if (curve.keys.Length >= 2)
        {
            if (selectedKey == 0)
            {
                Keyframe keyframeNext = curve.keys[selectedKey + 1];
                keyframe.outTangent = (keyframe.value - keyframeNext.value) / (keyframe.time - keyframeNext.time);
            }
            else if (selectedKey == curve.keys.Length - 1)
            {
                Keyframe keyframePrev = curve.keys[selectedKey - 1];
                keyframe.inTangent = (keyframePrev.value - keyframe.value) / (keyframePrev.time - keyframe.time);
            }
        }
        curve.MoveKey(selectedKey, keyframe);
    }

    void UpdateLinearTangent(AnimationCurve activeCurve, int keyIndex, bool leftTangent)
    {
        Keyframe keyframe = activeCurve.keys[keyIndex];
        if (leftTangent)
        {
            Keyframe keyframePrev = activeCurve.keys[keyIndex - 1];
            keyframe.inTangent = (keyframePrev.value - keyframe.value) / (keyframePrev.time - keyframe.time);
        }
        else
        {
            Keyframe keyframeNext = activeCurve.keys[keyIndex + 1];
            keyframe.outTangent = (keyframeNext.value - keyframe.value) / (keyframeNext.time - keyframe.time);
        }
        activeCurve.MoveKey(keyIndex, keyframe);
    }

    public void MouseDrag(Vector3 diff)
    {
        AnimationCurve activeCurve = mActiveCurveStr.firstCurveSelected ? mActiveCurveStr.curve1 : mActiveCurveStr.curve2;
        if (activeCurve == null) return;
        List<ContextMenuStruct> listContextMenus = contextMenuManager.dictCurvesContextMenus[activeCurve];

        if (tangentSelected)
        {//if any tangent is selected
            Vector2 mousePos = GetComponent<Camera>().ScreenToWorldPoint(crvWindow.CursorPos());
            Keyframe keyframe = activeCurve[selectedKey];

            Vector2 keyframeWorld = Convert(new Vector2(keyframe.time, keyframe.value), mGridRect, mGradRect);

            float ratio = (mGradRect.height / mGradRect.width) * (mGridRect.width / mGridRect.height);

            if (leftTangetSelected)
            {
                if (keyframeWorld.x - mousePos.x < marginErr)
                {
                    keyframe.inTangent = float.PositiveInfinity;
                }
                else
                {
                    keyframe.inTangent = ratio * (mousePos.y - keyframeWorld.y) / (mousePos.x - keyframeWorld.x);
                }
                if (listContextMenus[selectedKey].freeSmooth)
                {
                    keyframe.outTangent = keyframe.inTangent;

                    ContextMenuStruct contextMenuStuct = listContextMenus[selectedKey];
                    contextMenuStuct.flat = (keyframe.inTangent == 0);
                    listContextMenus[selectedKey] = contextMenuStuct;
                }
                activeCurve.MoveKey(selectedKey, keyframe);
            }
            else if (!leftTangetSelected)
            {
                if (mousePos.x - keyframeWorld.x < marginErr)
                {
                    keyframe.outTangent = float.PositiveInfinity;
                }
                else
                {
                    keyframe.outTangent = ratio * (mousePos.y - keyframeWorld.y) / (mousePos.x - keyframeWorld.x);
                }
                if (listContextMenus[selectedKey].freeSmooth)
                {
                    keyframe.inTangent = keyframe.outTangent;

                    ContextMenuStruct contextMenuStuct = listContextMenus[selectedKey];
                    contextMenuStuct.flat = (keyframe.outTangent == 0);
                    listContextMenus[selectedKey] = contextMenuStuct;
                }
                activeCurve.MoveKey(selectedKey, keyframe);
            }
            mAlteredData = true;
        }
        else if (keySelected)
        {//if any key is selected
            int prevSelectedKey = selectedKey;

            MoveKeyframe(selectedKey, diff);
            if (!keySelected)
            {
                return;
            }

            if (listContextMenus[selectedKey].auto)
            {
                UpdateAutoTangents(activeCurve, selectedKey);
            }
            //adapt neighbours also if they are auto
            if ((selectedKey > 0) && listContextMenus[selectedKey - 1].auto)
            {
                UpdateAutoTangents(activeCurve, selectedKey - 1);
            }
            if ((selectedKey < activeCurve.keys.Length - 1) && listContextMenus[selectedKey + 1].auto)
            {
                UpdateAutoTangents(activeCurve, selectedKey + 1);
            }

            if (selectedKey > 0)
            {
                if (listContextMenus[selectedKey].leftTangent.linear)
                {
                    UpdateLinearTangent(activeCurve, selectedKey, true);
                }
                //update the neighbour if is linear on this direction
                if (listContextMenus[selectedKey - 1].rightTangent.linear)
                {
                    UpdateLinearTangent(activeCurve, selectedKey - 1, false);
                }
            }
            if (selectedKey < activeCurve.keys.Length - 1)
            {
                if (listContextMenus[selectedKey].rightTangent.linear)
                {
                    UpdateLinearTangent(activeCurve, selectedKey, false);
                }
                //update the neighbour if is linear on this direction
                if (listContextMenus[selectedKey + 1].leftTangent.linear)
                {
                    UpdateLinearTangent(activeCurve, selectedKey + 1, true);
                }
            }

            //this is the case,the order of two consecutive keys has been just reversed
            if (selectedKey < prevSelectedKey)
            {
                if (listContextMenus[prevSelectedKey].rightTangent.linear)
                {
                    UpdateLinearTangent(activeCurve, prevSelectedKey, false);
                }
            }
            else if (selectedKey > prevSelectedKey)
            {
                if (listContextMenus[prevSelectedKey].leftTangent.linear)
                {
                    UpdateLinearTangent(activeCurve, prevSelectedKey, true);
                }
            }

            mAlteredData = true;
        }
        else if (lineSelected)
        {//if any curve is selected
            if (showCursorNormal && mGridRect.Contains(GetComponent<Camera>().ScreenToWorldPoint(crvWindow.CursorPos())))
            {
                showCursorNormal = false;
#if UNITY_3_5 || UNITY_EDITOR_OSX
				Screen.showCursor = false;
#else
                Cursor.SetCursor(textureNS, CurveWindow.hotspot, CursorMode.ForceSoftware);
#endif
            }
            else if (!showCursorNormal && !mGridRect.Contains(GetComponent<Camera>().ScreenToWorldPoint(crvWindow.CursorPos())))
            {
                showCursorNormal = true;
#if UNITY_3_5 || UNITY_EDITOR_OSX
				Screen.showCursor = true;
#else
                Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
#endif
            }

            Vector3 vecDiff = diff;
            vecDiff.x = 0;
            for (int i = 0; i < activeCurve.length; ++i)
            {
                MoveKeyframe(i, vecDiff);
            }
            mAlteredData = true;
        }
    }

    /// <summary>
    /// Draws the curves.Calls static DrawCurve method of Curves class.
    /// </summary>
    void DrawCurves()
    {
        AnimationCurve activeCurve = mActiveCurveStr.curve1;
        if (activeCurve == null) return;

        foreach (CurveStruct crv in curveStructList)
        {
            if (crv.curve1 == mActiveCurveStr.curve1) continue;
            Curves.DrawCurve(crv.color / 2, crv.curve1, crv.curve2, false, false, selectedKey, mGridRect, crv.gradRect);
        }
        Curves.DrawCurve(mActiveCurveStr.color, mActiveCurveStr.curve1, mActiveCurveStr.curve2, mActiveCurveStr.firstCurveSelected, !mActiveCurveStr.firstCurveSelected, selectedKey, mGridRect, mGradRect);
    }

    /// <summary>
    /// Convert from 2nd rect coordinates to the 1st rect coordinates.
    /// </param>/
    public static Vector2 Convert(Vector2 val, Rect rect1, Rect rect2)
    {
        Vector2 convVal = Vector2.zero;
        convVal.x = Mathf.Lerp(rect1.xMin, rect1.xMax, Mathf.InverseLerp(rect2.xMin, rect2.xMax, val.x));
        convVal.y = Mathf.Lerp(rect1.yMin, rect1.yMax, Mathf.InverseLerp(rect2.yMin, rect2.yMax, val.y));
        return convVal;
    }

    /// <summary>
    /// Handy method to get the number of horyzontal lines that should be drawn for a given gap.
    /// </param>
    private int GetHorLines(float gap)
    {
        if (gap <= 0) return 1; //gap should allways be positive
        if (gap >= 10)
        {
            return GetHorLines(gap / 10.0f);
        }
        else if (gap < 1)
        {
            return GetHorLines(gap * 10.0f);
        }
        else
        {
            mRezid = 0;//varies from [0 to 1)
            int gapInt = Mathf.FloorToInt(gap);
            if ((float)gapInt != gap) mRezid = (gap - (float)gapInt) / gap;

            middHor = true;
            float ratio = mGridRect.height / mUnit;
            while (ratio >= gapInt)
            {
                gapInt *= (middHor ? 2 : 5);
                middHor = !middHor;
            }

            float percentSample = (1f - mRezid) / gapInt;
            if (mRezid > percentSample)
            {
                float floatExtraLines = mRezid / percentSample + marginErr;//add an error margin ,e.g. 4 might pe represented like 3.999 etc...
                int extralines = (int)floatExtraLines;
                mRezid -= extralines * percentSample;
                if (mRezid < marginErr) mRezid = 0;
                gapInt += extralines;
            }
            return gapInt;
        }
    }

    //below are the methods which deal with PersistenceManager

    public void SaveData(string configName, Object obj)
    {
        PersistenceManager.SaveData(configName, obj, crvWindow, curveStructList, activeCurveStr, contextMenuManager.dictCurvesContextMenus);
        mAlteredData = false;
    }
    public void LoadData(string configName, Object obj)
    {
        RemoveData();
        PersistenceManager.LoadData(configName, obj, crvWindow, transform.parent.Find("Collider"), this, curveStructList, contextMenuManager.dictCurvesContextMenus);
        mWindowClosed = crvWindow.windowClosed;
        mAlteredData = false;
    }

    void RemoveData()
    {
        curveStructList.Clear(); contextMenuManager.dictCurvesContextMenus.Clear(); listColors.Clear(); FillListColor();
        crvWindow.maximized = false;
        mActiveCurveStr = nullCurve;
        gradRect = mActiveCurveStr.gradRect;
    }

    public void NewWindow()
    {
        RemoveData();
        crvWindow.transform.localPosition = Vector3.zero;
        Transform collTransf = transform.parent.Find("Collider");
        collTransf.localPosition = Vector3.zero;

        crvWindow.transform.localScale = Vector3.one;
        collTransf.localScale = Vector3.one;

        PersistenceManager.RemoveLastFileKey();
        mAlteredData = false;
    }

    public List<string> GetNamesList()
    {
        return PersistenceManager.GetNamesList();
    }

    public void DeleteFile(string name)
    {
        PersistenceManager.DeleteFile(name);
    }

    public string GetLastFile()
    {
        return PersistenceManager.GetLastFile();
    }
}
