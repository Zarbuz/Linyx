using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class KaleidoscopeEffect : MonoBehaviour
{
    public enum EffectType
    {
        Circle,
        Triangle60Tiling,
        Triangle90Tiling
    }
    public EffectType effectType;
    public int number = 6;
    public Vector2 center = new Vector2(0.5f, 0.5f);
    public float radius = 0.5f;
    public float angle = 0;
    public float scale = 1;

    private static Material _Material;
    public Shader shader;

    private static Material _Material2;
    public Shader shader2;

    private void OnDisable()
    {
        if (_Material)
        {
            DestroyImmediate(_Material);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material == null)
        {
            _Material = new Material(shader);
            _Material2 = new Material(shader2);
        }

        float xCorrect = Screen.height / (float)Screen.width;
        Graphics.SetRenderTarget(destination);
        GL.PushMatrix();
        GL.LoadOrtho();
#if UNITY_EDITOR
        GL.Clear(true, false, Color.black);
        _Material2.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(new Color(0, 0, 0, 1));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();
#else
			GL.Clear(true,true,Color.black);
#endif

        _Material.mainTexture = source;
        _Material.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        if (effectType == EffectType.Circle)
        {

            int n = (number / 2) * 2;
            if (n < 4) n = 4;

            float angle1 = (angle / 360 - 0.5f / (float)n) * Mathf.PI * 2;
            float angle2 = (angle / 360 + 0.5f / (float)n) * Mathf.PI * 2;
            Vector2 uv1 = center + new Vector2(Mathf.Sin(angle1) * xCorrect, Mathf.Cos(angle1)) * radius / scale;
            Vector2 uv2 = center + new Vector2(Mathf.Sin(angle2) * xCorrect, Mathf.Cos(angle2)) * radius / scale;
            for (int i = 0; i < n; i++)
            {
                angle1 = (angle / 360 + (i - 0.5f) / (float)n) * Mathf.PI * 2;
                angle2 = (angle / 360 + (i + 0.5f) / (float)n) * Mathf.PI * 2;
                GL.TexCoord((i % 2 == 0) ? uv1 : uv2);
                GL.Vertex(new Vector3(center.x + Mathf.Sin(angle1) * xCorrect * radius, center.y + Mathf.Cos(angle1) * radius, 0));
                GL.TexCoord((i % 2 == 0) ? uv2 : uv1);
                GL.Vertex(new Vector3(center.x + Mathf.Sin(angle2) * xCorrect * radius, center.y + Mathf.Cos(angle2) * radius, 0));
                GL.TexCoord(center);
                GL.Vertex(center);
            }

        }
        else if (effectType == EffectType.Triangle60Tiling)
        {
            int n = number;
            if (n < 1) n = 1;
            float angle1 = (angle / 360 - 0.5f / 6) * Mathf.PI * 2;
            float angle2 = (angle / 360 + 0.5f / 6) * Mathf.PI * 2;
            Vector3[] uv = new Vector3[]{
                center,
                center+new Vector2(Mathf.Sin(angle1)*xCorrect, Mathf.Cos(angle1))*radius/scale,
                center+new Vector2(Mathf.Sin(angle2)*xCorrect, Mathf.Cos(angle2))*radius/scale
                };
            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    angle1 = (angle / 360 + (j - 0.5f) / (float)6) * Mathf.PI * 2;
                    angle2 = (angle / 360 + (j + 0.5f) / (float)6) * Mathf.PI * 2;
                    Vector3 up1 = new Vector3(center.x + Mathf.Sin(angle1) * xCorrect * radius * i, center.y + Mathf.Cos(angle1) * radius * i, 0);
                    Vector3 up2 = new Vector3(center.x + Mathf.Sin(angle2) * xCorrect * radius * i, center.y + Mathf.Cos(angle2) * radius * i, 0);
                    Vector3 down1 = new Vector3(center.x + Mathf.Sin(angle1) * xCorrect * radius * (i - 1), center.y + Mathf.Cos(angle1) * radius * (i - 1), 0);
                    Vector3 down2 = new Vector3(center.x + Mathf.Sin(angle2) * xCorrect * radius * (i - 1), center.y + Mathf.Cos(angle2) * radius * (i - 1), 0);
                    for (int k = 0; k <= i; k++)
                    {
                        if (j % 2 == 0) GL.TexCoord(uv[(i + k) % 3]);
                        else GL.TexCoord(uv[2 - (i + k + 2) % 3]);
                        GL.Vertex(Vector3.Lerp(up1, up2, k / (float)(i)));
                        if (j % 2 == 0) GL.TexCoord(uv[(i + k + 1) % 3]);
                        else GL.TexCoord(uv[2 - (i + k + 1 + 2) % 3]);
                        GL.Vertex(Vector3.Lerp(up1, up2, (k + 1) / (float)(i)));
                        if (j % 2 == 0) GL.TexCoord(uv[(i - 1 + k) % 3]);
                        else GL.TexCoord(uv[2 - (i - 1 + k + 2) % 3]);
                        if (i == 1) GL.Vertex(center);
                        else GL.Vertex(Vector3.Lerp(down1, down2, (k) / (float)(i - 1)));

                        if (k > 0)
                        {
                            if (j % 2 == 0) GL.TexCoord(uv[(i + k - 1) % 3]);
                            else GL.TexCoord(uv[2 - (i + k - 1 + 2) % 3]);
                            GL.Vertex(Vector3.Lerp(down1, down2, k / (float)(i - 1)));
                            if (j % 2 == 0) GL.TexCoord(uv[(i + k - 2) % 3]);
                            else GL.TexCoord(uv[2 - (i + k - 2 + 2) % 3]);
                            GL.Vertex(Vector3.Lerp(down1, down2, (k - 1) / (float)(i - 1)));

                            if (j % 2 == 0) GL.TexCoord(uv[(i + k) % 3]);
                            else GL.TexCoord(uv[2 - (i + k + 2) % 3]);
                            GL.Vertex(Vector3.Lerp(up1, up2, (k) / (float)(i)));
                        }

                    }
                }
            }
        }
        else if (effectType == EffectType.Triangle90Tiling)
        {
            int n = number;
            if (n < 1) n = 1;

            float angle1 = (angle / 360) * Mathf.PI * 2;
            float angle2 = (angle / 360 + 0.125f) * Mathf.PI * 2;
            Vector3[] uv = new Vector3[]{
                center+new Vector2(Mathf.Sin(angle2)*xCorrect, Mathf.Cos(angle2))*Mathf.Sqrt (radius*radius*2)/scale,
                center+new Vector2(Mathf.Sin(angle1)*xCorrect, Mathf.Cos(angle1))*radius/scale,
                center
            };
            Vector3 right = new Vector3(Mathf.Sin(angle / 180 * Mathf.PI + Mathf.PI / 2), Mathf.Cos(angle / 180 * Mathf.PI + Mathf.PI / 2), 0);
            Vector3 up = new Vector3(Mathf.Sin(angle / 180 * Mathf.PI), Mathf.Cos(angle / 180 * Mathf.PI), 0);
            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    Vector3 x = new Vector3();
                    Vector3 y = new Vector3();
                    if (j == 0)
                    {
                        x = right; y = up;
                    }
                    else if (j == 1)
                    {
                        x = -up; y = right;
                    }
                    else if (j == 2)
                    {
                        x = -right; y = -up;
                    }
                    else if (j == 3)
                    {
                        x = up; y = -right;
                    }
                    x *= radius;
                    y *= radius;
                    x.x *= xCorrect;
                    y.x *= xCorrect;
                    for (int k = 0; k < i; k++)
                    {

                        GL.TexCoord(uv[i % 2 == 0 ? 2 : 0]);
                        GL.Vertex((Vector3)center + (-i + k * 2) * x + i * y);
                        GL.TexCoord(uv[1]);
                        GL.Vertex((Vector3)center + (-i + k * 2 + 1) * x + i * y);
                        GL.TexCoord(uv[i % 2 == 0 ? 0 : 2]);
                        GL.Vertex((Vector3)center + (-i + k * 2 + 1) * x + (i - 1) * y);
                        //

                        GL.TexCoord(uv[1]);
                        GL.Vertex((Vector3)center + (-i + k * 2 + 1) * x + i * y);
                        GL.TexCoord(uv[i % 2 == 0 ? 2 : 0]);
                        GL.Vertex((Vector3)center + (-i + k * 2 + 2) * x + i * y);
                        GL.TexCoord(uv[i % 2 == 0 ? 0 : 2]);
                        GL.Vertex((Vector3)center + (-i + k * 2 + 1) * x + (i - 1) * y);

                        if (i > 1 && k < i - 1)
                        {

                            GL.TexCoord(uv[i % 2 == 0 ? 2 : 0]);
                            GL.Vertex((Vector3)center + (-i + k * 2 + 2) * x + i * y);
                            GL.TexCoord(uv[1]);
                            GL.Vertex((Vector3)center + (-i + k * 2 + 2) * x + (i - 1) * y);
                            GL.TexCoord(uv[i % 2 == 0 ? 0 : 2]);
                            GL.Vertex((Vector3)center + (-i + k * 2 + 1) * x + (i - 1) * y);
                            //

                            GL.TexCoord(uv[i % 2 == 0 ? 2 : 0]);
                            GL.Vertex((Vector3)center + (-i + k * 2 + 2) * x + i * y);
                            GL.TexCoord(uv[i % 2 == 0 ? 0 : 2]);
                            GL.Vertex((Vector3)center + (-i + k * 2 + 3) * x + (i - 1) * y);
                            GL.TexCoord(uv[1]);
                            GL.Vertex((Vector3)center + (-i + k * 2 + 2) * x + (i - 1) * y);

                        }

                    }
                }
            }

        }
        GL.End();
        GL.PopMatrix();
        Graphics.SetRenderTarget(null);
        //Graphics.Blit (source, destination);
    }
    /*
	void  OnDrawGizmosSelected (){
		float xCorrect = Screen.height/(float)Screen.width;
		Vector3 pt1 = Vector3.zero;
		Vector3 pt2 = Vector3.zero;
		Vector3 pt3 = Vector3.zero;
		if(effectType == EffectType.Circle){
			int n = (number/2)*2;
			if(n<4)n=4;
			float angle1 = (angle/360-0.5f/(float)n)*Mathf.PI*2;
			float angle2 = (angle/360+0.5f/(float)n)*Mathf.PI*2;
			pt1 = transform.TransformPoint(new Vector3(center.x,center.y,0)+new Vector3(Mathf.Sin(angle1)*xCorrect, Mathf.Cos(angle1),0)*radius/scale);
			pt2 = new Vector3(center.x,center.y,0)+new Vector3(Mathf.Sin(angle2)*xCorrect, Mathf.Cos(angle2),0)*radius/scale;
			pt3 = new Vector3(center.x,center.y,0);
		}else if(effectType == EffectType.Triangle60Tiling){
		}else if(effectType == EffectType.Triangle90Tiling){
		}
		pt1.z =1;
		pt2.z =1;
		pt3.z =1;
		Gizmos.DrawLine(pt1,pt2);
		Gizmos.DrawLine(pt2,pt3);
		Gizmos.DrawLine(pt3,pt1);
	}*/
}
