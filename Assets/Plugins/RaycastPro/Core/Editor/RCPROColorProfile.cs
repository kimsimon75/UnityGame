using UnityEngine;

#if UNITY_EDITOR
namespace RaycastPro.Editor
{
    [CreateAssetMenu(fileName = "RCProColorProfile", menuName = "RCPRO/ColorProfile", order = 1)]
    public class RCPROColorProfile : ScriptableObject
    {
        public Color DefaultColor = RCProEditor.Aqua;
        public Color DetectColor = new Color(.3f, 1, .3f, 1f);
        public Color HelperColor = new Color(1f, .7f, .0f, 1f);
        public Color BlockColor = new Color(1f, .2f, .2f, 1f);
    }
}
#endif
