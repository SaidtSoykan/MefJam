using UnityEngine;

public class PlantRenderHandler: MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    
    public void SetGrowthVisual(float value01)
    {
        if (renderers == null || renderers.Length == 0)
            return;

        value01 = Mathf.Repeat(value01, 1f); // IMPORTANT: looping

        int count = renderers.Length;

        if (count == 1)
        {
            SetRendererAlpha(renderers[0], 1f);
            return;
        }

        float segmentSize = 1f / count;

        int index = Mathf.FloorToInt(value01 / segmentSize);
        int nextIndex = (index + 1) % count;

        float localT = (value01 - index * segmentSize) / segmentSize;

        for (int i = 0; i < count; i++)
        {
            if (i == index)
            {
                SetRendererAlpha(renderers[i], 1f - localT);
                renderers[i].gameObject.SetActive(true);
            }
            else if (i == nextIndex)
            {
                SetRendererAlpha(renderers[i], localT);
                renderers[i].gameObject.SetActive(true);
            }
            else
            {
                SetRendererAlpha(renderers[i], 0f);
                renderers[i].gameObject.SetActive(false);
            }
        }
    }


    void SetRendererAlpha(Renderer renderer, float alpha)
    {
        foreach (Material mat in renderer.materials)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }
    }

    public void SetColor(Color color)
    {
        foreach (var ren in renderers)
        {
            foreach (Material mat in ren.materials)
            {
                mat.color = color;
            }
        }
    }
}
