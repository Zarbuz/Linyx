//----------------------------------------------
// Runtime Curve Editor
// Copyright Â© 2013 Rus Artur PFA
// rus_artur@yahoo.com
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


/// <summary>
/// Curve editor's window,draws the window itself and gradations.
/// </summary>
public class CurveWindow : MonoBehaviour
{
    public Camera cameraOrtho;//curve editor's camera
    CurveLines crvLines;//keep a reference to the component drawing the curves

    public GUIStyle guiStyleYMax;//this style is used only for the text field that selects the ymax
    public GUIStyle guiStyle;
    public GUIStyle guiStyleHeader;

    public Texture2D close;
    public Texture2D maximize;
    public Texture2D textureNS;
    public Texture2D textureWE;
    public Texture2D textureNWSE;
    public Texture2D textureSWNE;

    enum cursors { cursorNS, cursorWE, cursorNWSE, cursorSWNE, normal };
    bool onLeftEdge = false;
    bool onRightEdge = false;
    bool onTopEdge = false;
    bool onBottomEdge = false;
    cursors mCursor = cursors.normal;

    float marginPixels = 3f;
    float margin;//marginPixels in object coordinates

    float headerPixels = 26f;//the size in pixels of the window's header, needed only when draging the window 
    float header;

    int screenHeight = 0;

    public static Vector2 hotspot = new Vector2(8, 8);//The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).

    Mesh mesh;//used for window,so that when scaling the borders keep their size
    Mesh meshColl;//used for collider,4 vertices are enough
    int[] triangles;
    Vector2[] uvs;

    float mMinW = 0.35f;
    float mMinH = 0.8f;
    float mH;
    float mW;
    float mX;
    float mY;
    const float k_hor = 3f;//constant value for initial horizontal scaling

    Transform cachedTransf;//cached transform
    Transform cachedCollTransf;//collider's transform
    Transform cachedParentTransf;//cached parent transform

    float mUnit = 0.0f;//keeps a unit in object coordinates(which is constant in pixels)	

    float iconSize = 16;//maximize and close icons size 

    Vector3 lastScale = Vector3.one;//keeps the last scale ,needed to check if scale got changed

    Vector3 mousePos;//current mouse pos
    bool headerMouseDown = false;//if true, the user is draging the whole curve editor window

    Rect closeRect = new Rect();//keeps the rect for the close button
    Rect maximizeRect = new Rect();//keeps the rect for the close button

    float mZ = -1f;//Z position of the window

    Rect mGridRect;//keeps the rect of the grid(in object coordinates)

    internal bool maximized = false;//true if the window is maximized
    Vector3 lastLocalScale;//used for un-maximizing 
    Vector3 lastLocalPosition;//used for un-maximizing

    const float marginErr = 0.00001f;

    const float defaultDPI = 96f;
    float dpiRatio = 0f;

    bool mouseDown = false;
    bool mouseUp = false;
    bool mouseDrag = false;
    bool touched = false;
    public bool IsTouched
    {
        get { return touched; }
    }
    public bool IsTouchedBegan()
    {
        if (Input.touchCount == 0) return false;
        return (Input.touches[0].phase == TouchPhase.Began);
    }
    public bool IsDoubleTap()
    {
        if (Input.touchCount != 1) return false;
        return (Input.touches[0].tapCount == 2);
    }
    public bool IsSingleTap()
    {
        if (Input.touchCount != 1) return false;
        return (Input.touches[0].tapCount == 1);
    }

    bool mWindowClosed = true;
    public bool windowClosed
    {
        set
        {
            if (value != mWindowClosed) mWindowClosed = value;
            if (cachedCollTransf != null)
            {
                cachedCollTransf.GetComponent<Collider>().enabled = !mWindowClosed;
                GetComponent<Renderer>().enabled = !mWindowClosed;
            }
        }
        get { return mWindowClosed; }
    }


    void Awake()
    {
        UpdatePixelLengths();
        mesh = new Mesh();

        //the window has 16 vertices, so that the scaling ,without altering the borders can be possible
        Vector3[] vertices = new Vector3[16];
        uvs = new Vector2[16];
        triangles = new int[54];

        //initial size and position of the window
        mH = mMinH;
        mW = k_hor * mMinW;
        mX = -mW / 2;
        mY = -mH / 2;

        int i1 = 0;
        int i2 = 0;
        float factor1 = 0f;
        float factor2 = 0f;
        for (int i = 0; i < 16; ++i)
        {
            i1 = i % 4;
            i2 = i / 4;
            uvs[i] = new Vector2(i1 / 3.0f, i2 / 3.0f);
            if (i1 == 1) factor1 = 1f / (k_hor * 3.0f);
            else if (i1 == 2) factor1 = 1f - (1f / (k_hor * 3.0f));
            else factor1 = i1 / 3.0f;
            factor2 = i2 / 3.0f;

            vertices[i] = new Vector3(mX + mW * factor1, mY + mH * factor2, mZ);
        }

        int j;
        for (int i = 0; i < 3; ++i)
        {
            for (j = 0; j < 3; ++j)
            {
                triangles[0 + (i * 3 + j) * 6] = 0 + j + i * 4;
                triangles[1 + (i * 3 + j) * 6] = 5 + j + i * 4;
                triangles[2 + (i * 3 + j) * 6] = 1 + j + i * 4;
                triangles[3 + (i * 3 + j) * 6] = 0 + j + i * 4;
                triangles[4 + (i * 3 + j) * 6] = 4 + j + i * 4;
                triangles[5 + (i * 3 + j) * 6] = 5 + j + i * 4;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        //colliders mesh needs only 4 vertices
        meshColl = new Mesh();
        meshColl.vertices = new Vector3[]{new Vector3(mX-margin,mY-margin,mZ),new Vector3(mX+mW+margin,mY-margin,mZ),
                                          new Vector3(mX+mW+margin,mY+mH+margin,mZ),new Vector3(mX-margin,mY+mH+margin,mZ)};
        meshColl.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        //the mesh collider component is on a different game object(being possible to assing different layer than the window's itself)
        transform.parent.Find("Collider").GetComponent<MeshCollider>().sharedMesh = meshColl;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Start()
    {
        //margin = 2f*marginPixels/(float)Screen.currentResolution.height;

        cachedTransf = transform;
        cachedCollTransf = transform.parent.Find("Collider");

        windowClosed = mWindowClosed;//in case windowClosed property has been set before Start(),mWindowClosed will be redundant

        cachedParentTransf = transform.parent;
        crvLines = cameraOrtho.GetComponent<CurveLines>();

        crvLines.textureNS = textureNS;
        UpdateGrid();

        if (Screen.dpi != 0f)
        {
            dpiRatio = Screen.dpi / 96f;
            iconSize *= (int)(dpiRatio);
        }

    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touched = true;
            //if so, the mouse inputs,if any,are not processed
            mouseDown = mouseUp = mouseDrag = false;

            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    mouseDown = true;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    mouseUp = true;
                }
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    mouseDrag = true;
                }
            }
        }
        else
        {
            touched = false;
        }
        if (mouseDown)
        {
            MouseDownOnUpdate();
            mouseDown = false;
        }
        if (mouseUp)
        {
            MouseUpOnUpdate();
            mouseUp = false;
        }
        if (mouseDrag)
        {
            MouseDragOnUpdate();
            mouseDrag = false;
        }

        GL.Clear(true, true, new Color(0.19f, 0.19f, 0.19f, 1.0f));
        UpdateVertices();

        UpdateGrid();
    }


    /// <summary>
    /// Update the grid size
    /// </param>
    private void UpdateGrid()
    {
        Vector3 pos = cachedTransf.position;
        Vector3 scale = cachedTransf.lossyScale;
        Vector3 parScale = cachedParentTransf.lossyScale;

        //keep the grid ,with .y being on bottom(don't call contains on it)
        mGridRect.x = pos.x + mX * scale.x + parScale.x * mMinW * 0.38f;
        mGridRect.width = mW * scale.x - parScale.x * mMinW * 0.62f;
        mGridRect.height = mH * scale.y - parScale.y * mMinH * 0.55f;
        mGridRect.y = pos.y + mY * scale.y + parScale.y * mMinH * 0.3f;

        if (crvLines.gridRect != mGridRect)
        {
            crvLines.gridRect = mGridRect;
        }
        float unit = mMinW * 0.38f * cachedParentTransf.lossyScale.y;
        if (unit != mUnit)
        {
            mUnit = unit;
            crvLines.unit = mUnit;
        }
        float bottomYMax = pos.y + mY * scale.y + parScale.y * mMinH * 0.04f;
        float bottomYMin = bottomYMax + parScale.y * mMinH * 0.07f;

        crvLines.windowBottom = Rect.MinMaxRect(0, bottomYMin, 0, bottomYMax);
    }

    void UpdateVertices()
    {
        //update verticess if any scaling takes place
        Vector3 s = cachedTransf.localScale;
        if (s == lastScale) return;
        lastScale = s;
        Vector3[] verts = mesh.vertices;

        Vector4 border = new Vector4(mX + mW / (k_hor * 3f * s.x), mY + mH / (3 * s.y), mX + mW - mW / (k_hor * 3f * s.x), mY + mH - mH / (3 * s.y));
        verts[1].x = verts[13].x = verts[5].x = verts[9].x = border.x;
        verts[2].x = verts[14].x = verts[6].x = verts[10].x = border.z;
        verts[4].y = verts[7].y = verts[5].y = verts[6].y = border.y;
        verts[8].y = verts[11].y = verts[9].y = verts[10].y = border.w;

        mesh.vertices = verts;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void GetWindowLimits(out float xleft, out float xright, out float ybottom, out float ytop)
    {
        Vector3 pos = cachedTransf.position;
        Vector3 scale = cachedTransf.lossyScale;

        xleft = pos.x + mX * scale.x;
        xright = xleft + mW * scale.x;
        ybottom = pos.y + mY * scale.y;
        ytop = ybottom + mH * scale.y;
    }

    public void MouseDown()
    {
        mouseDown = true;
    }
    public void MouseUp()
    {
        mouseUp = true;
    }
    public void MouseDrag()
    {
        mouseDrag = true;
    }

    public Vector3 CursorPos()
    {
        if (touched)
        {
            Vector2 touchPos = Input.touches[0].position;
            return new Vector3(touchPos.x, touchPos.y);
        }
        return Input.mousePosition;
    }

    void MouseDownOnUpdate()
    {
        Vector3 screenPos = CursorPos();
        mousePos = cameraOrtho.ScreenToWorldPoint(screenPos);
        float xleft, xright, ybottom, ytop;
        GetWindowLimits(out xleft, out xright, out ybottom, out ytop);

        if ((xleft + margin < mousePos.x) && (xright - margin > mousePos.x) && (ytop - margin > mousePos.y) && (ytop - header - margin < mousePos.y))
        {
            headerMouseDown = true;
        }
    }

    void MouseUpOnUpdate()
    {
        if (mCursor != cursors.normal) return;
        headerMouseDown = false;
        Vector2 mousePosUp = CursorPos();
        mousePosUp.y = Screen.height - mousePosUp.y;
        if (closeRect.Contains(mousePosUp))
        {
            windowClosed = true;
            crvLines.windowClosed = true;
            crvLines.alteredData = true;
        }
        else if (maximizeRect.Contains(mousePosUp))
        {
            if (!maximized)
            {
                Vector3 locScale = cachedTransf.localScale;
                Vector3 parentScale = cachedParentTransf.lossyScale;
                lastLocalScale = locScale;
                lastLocalPosition = cachedTransf.localPosition;

                locScale.x = (2f * Screen.width) / (Screen.height * mW * parentScale.x);
                locScale.y = 2f / (mH * parentScale.y);
                cachedTransf.localScale = locScale;
                cachedCollTransf.localScale = locScale;
                cachedTransf.localPosition = Vector3.zero;
                cachedCollTransf.localPosition = Vector3.zero;
                maximized = true;
            }
            else
            {
                cachedTransf.localScale = lastLocalScale;
                cachedCollTransf.localScale = lastLocalScale;
                cachedTransf.localPosition = lastLocalPosition;
                cachedCollTransf.localPosition = lastLocalPosition;
                maximized = false;
            }
            crvLines.alteredData = true;
        }
        else
        {
            crvLines.MouseUp();
        }
    }

    void MouseDragOnUpdate()
    {
        Vector2 mousePosDrag = CursorPos();
        mousePosDrag.y = Screen.height - mousePosDrag.y;
        if (closeRect.Contains(mousePosDrag) || maximizeRect.Contains(mousePosDrag))
        {
            return;
        }

        Vector3 newMousPos = cameraOrtho.ScreenToWorldPoint(CursorPos());

        Vector3 pos2 = newMousPos;
        Vector3 pos1 = mousePos;

        if ((Mathf.Abs(newMousPos.x - mousePos.x) < marginErr) &&
            (Mathf.Abs(newMousPos.y - mousePos.y) < marginErr)) return;

        pos1.x = Mathf.Clamp(pos1.x, mGridRect.xMin, mGridRect.xMax);
        pos2.x = Mathf.Clamp(pos2.x, mGridRect.xMin, mGridRect.xMax);
        pos1.y = Mathf.Clamp(pos1.y, mGridRect.yMin, mGridRect.yMax);
        pos2.y = Mathf.Clamp(pos2.y, mGridRect.yMin, mGridRect.yMax);
        Vector3 diff = pos2 - pos1;
        crvLines.MouseDrag(diff);

        if (mCursor == cursors.normal && !headerMouseDown)
        {
            mousePos = newMousPos;
            return;
        }

        diff = newMousPos - mousePos;
        diff.x /= cachedParentTransf.lossyScale.x;
        diff.y /= cachedParentTransf.lossyScale.y;

        Vector3 locPos = cachedTransf.localPosition;

        if (headerMouseDown)
        {
            locPos += diff;
            crvLines.alteredData = true;
        }
        else if (cursors.normal != mCursor)
        {
            Vector3 locScale = cachedTransf.localScale;
            if (cursors.cursorNS == mCursor)
            {
                locPos.y += diff.y / 2;
                locScale.y += (onTopEdge ? diff.y : -diff.y) / mH;
            }
            else if (cursors.cursorWE == mCursor)
            {
                locPos.x += diff.x / 2;
                locScale.x += (onRightEdge ? diff.x : -diff.x) / mW;
            }
            else if (cursors.cursorNWSE == mCursor || cursors.cursorSWNE == mCursor)
            {
                locPos.x += diff.x / 2;
                locScale.x += (onRightEdge ? diff.x : -diff.x) / mW;
                locPos.y += diff.y / 2;
                locScale.y += (onTopEdge ? diff.y : -diff.y) / mH;
            }
            if (mW * locScale.x < mMinW || mH * locScale.y < mMinH)
            {
                return;
            }
            cachedTransf.localScale = locScale;
            cachedCollTransf.localScale = locScale;
            crvLines.alteredData = true;
        }
        cachedTransf.localPosition = locPos;
        cachedCollTransf.localPosition = locPos;

        mousePos = newMousPos;
        if (maximized) { maximized = false; }
    }

    /// <summary>
    /// Display the window name and the maximize and close icons.
    /// </summary>
    void OnGUI()
    {
        if (mWindowClosed) return;
        cameraOrtho.Render();

        Vector3 pos = cachedTransf.position;
        Vector3 lossyScale = cachedTransf.lossyScale;
        Vector3 worldPoint = Vector3.zero;
        worldPoint.x = pos.x + (mX + mW / 2) * lossyScale.x;
        worldPoint.y = pos.y + (mY + mH) * lossyScale.y;
        worldPoint.z = -1;

        Vector3 screenPoint = cameraOrtho.WorldToScreenPoint(worldPoint);
        Rect rect = Rect.MinMaxRect(0, 0, 0, 0);
        rect.y = Screen.height - screenPoint.y + guiStyleHeader.fontSize;
        rect.x = screenPoint.x - guiStyleHeader.fontSize;
        GUI.Label(rect, "Curve Editor", guiStyleHeader);

        worldPoint.x = pos.x + (mX + mW) * lossyScale.x;
        screenPoint = cameraOrtho.WorldToScreenPoint(worldPoint);
        closeRect.x = screenPoint.x - iconSize * 0.75f;
        closeRect.y = Screen.height - screenPoint.y;
        closeRect.height = closeRect.width = iconSize;
        GUI.Label(closeRect, close);
        maximizeRect = closeRect;
        maximizeRect.x -= iconSize * 0.75f;
        GUI.Label(maximizeRect, maximize);

        ShowGradations();
        ShowResizeCursor();

    }
    /// <summary>
    /// Display here(instead out of CurveLines) the grid gradations.
    /// </summary>
    void ShowGradations()
    {
        GUI.depth = -1;
        Rect rect = Rect.MinMaxRect(0, 0, 0, 0);
        rect.height = rect.width = guiStyle.fontSize;
        //horyzontal gradations
        for (int i = 0; i <= crvLines.colls; ++i)
        {
            rect.x = Screen.width / 2.0f + (mGridRect.x + i * (mGridRect.width / crvLines.colls)) * Screen.height / 2.0f - guiStyle.fontSize;
            rect.y = (1 - mGridRect.y) * (Screen.height / 2.0f) + guiStyle.fontSize;
            GUI.Label(rect, (i * (crvLines.gradRect.xMax - crvLines.gradRect.xMin) / crvLines.colls + crvLines.gradRect.xMin).ToString(), guiStyle);
        }

        //vertical gradations
        foreach (KeyValuePair<float, decimal> pair in crvLines.rows)
        {
            rect.x = Screen.width / 2.0f + mGridRect.x * Screen.height / 2.0f;
            rect.y = (1 - pair.Key) * Screen.height / 2.0f - guiStyle.fontSize / 2;
            if (pair.Value != 1.0m)
            {
                rect.x -= 3 * guiStyle.fontSize;
                GUI.Label(rect, ((crvLines.gradRect.yMax - crvLines.gradRect.yMin) * (float)pair.Value + crvLines.gradRect.yMin * (1 - crvLines.Rezid)).ToString("0.###"), guiStyle);
            }
            else
            {
                rect.x -= guiStyleYMax.fixedWidth + guiStyle.fontSize / 2;

                string yMaxStr = GUI.TextField(rect, crvLines.gradRect.yMax.ToString("0.###"), guiStyleYMax);

                float result;
                if (float.TryParse(yMaxStr, out result))
                {
                    if (crvLines.gradRect.yMax != result)
                    {
                        //yMaxim is changed so update the gradation rect in CurveLines
                        float ymin = crvLines.gradRect.yMin;
                        if (ymin < 0f) { ymin = -result; }
                        crvLines.gradRect = Rect.MinMaxRect(crvLines.gradRect.xMin, ymin, crvLines.gradRect.xMax, result);//exchanged ymin to ymax
                        crvLines.UpdateActiveCurveKeys();
                        crvLines.alteredData = true;
                    }
                }
                else
                {
                    crvLines.gradRect = Rect.MinMaxRect(crvLines.gradRect.xMin, crvLines.gradRect.yMin, crvLines.gradRect.xMax, 0);//exchanged ymin to ymax
                    yMaxStr = crvLines.gradRect.yMax.ToString();
                    crvLines.UpdateActiveCurveKeys();
                    crvLines.alteredData = true;
                }
            }
        }
    }

    void ShowResizeCursor()
    {
        //show the cursor if it's over any of the window's edges
        if (!touched && Input.GetMouseButton(0))
        {
            return;
        }

        UpdatePixelLengths();
        Vector3 cursorPos = CursorPos();

        Vector3 mouse = cameraOrtho.ScreenToWorldPoint(cursorPos);

        Vector3 pos = cachedTransf.position;
        Vector3 scale = cachedTransf.lossyScale;

        float xleft = pos.x + mX * scale.x;
        float xright = xleft + mW * scale.x;
        float ybottom = pos.y + mY * scale.y;
        float ytop = ybottom + mH * scale.y;

        onLeftEdge = Mathf.Abs(xleft - mouse.x) < margin;
        onRightEdge = Mathf.Abs(xright - mouse.x) < margin;
        onTopEdge = Mathf.Abs(ytop - mouse.y) < margin;
        onBottomEdge = Mathf.Abs(ybottom - mouse.y) < margin;

        if ((mouse.x < xleft - margin) || (mouse.x > xright + margin) || (mouse.y < ybottom - margin) || (mouse.y > ytop + margin) ||
            (!onLeftEdge && !onRightEdge && !onTopEdge && !onBottomEdge))
        {
            if (mCursor == cursors.normal) return;
#if UNITY_3_5 || UNITY_EDITOR_OSX
			Screen.showCursor = true;
#else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
#endif
            mCursor = cursors.normal;
        }
        else if ((onLeftEdge && onTopEdge) || (onRightEdge && onBottomEdge))
        {
            mCursor = cursors.cursorNWSE;
#if UNITY_3_5 || UNITY_EDITOR_OSX
			Screen.showCursor = false;
#else
            Cursor.SetCursor(textureNWSE, hotspot, CursorMode.ForceSoftware);
#endif
        }
        else if ((onLeftEdge && onBottomEdge) || (onRightEdge && onTopEdge))
        {
            mCursor = cursors.cursorSWNE;
#if UNITY_3_5 || UNITY_EDITOR_OSX
			Screen.showCursor = false;
#else
            Cursor.SetCursor(textureSWNE, hotspot, CursorMode.ForceSoftware);
#endif
        }
        else if (onLeftEdge || onRightEdge)
        {
            mCursor = cursors.cursorWE;
#if UNITY_3_5 || UNITY_EDITOR_OSX
			Screen.showCursor = false;
#else
            Cursor.SetCursor(textureWE, hotspot, CursorMode.ForceSoftware);
#endif
        }
        else if (onTopEdge || onBottomEdge)
        {
            mCursor = cursors.cursorNS;
#if UNITY_3_5 || UNITY_EDITOR_OSX
			Screen.showCursor = false;
#else
            Cursor.SetCursor(textureNS, hotspot, CursorMode.ForceSoftware);
#endif
        }
    }

    /// <summary>
    /// Updates the object coordinates values (having constant values in pixels) when the Screen's height changes.
    /// </summary>
    void UpdatePixelLengths()
    {
        if (screenHeight != Screen.height)
        {
            screenHeight = Screen.height;
            margin = 2f * marginPixels / (float)screenHeight;
            if (dpiRatio != 0f) margin *= dpiRatio;
            header = 2f * headerPixels / (float)screenHeight;
        }
    }
}
