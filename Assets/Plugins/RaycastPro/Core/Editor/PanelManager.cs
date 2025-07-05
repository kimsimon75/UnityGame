#if UNITY_EDITOR
namespace RaycastPro.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName = "IconPrefabDatabase", menuName = "RCPro/Panel Database", order = 0)]
    public class PanelDataBase : ScriptableObject
    {
        public MonoScript[] favorite;
        public MonoScript[] utility;
        public MonoScript[] raySensors, detectors, planers, casters, bullets;
        public MonoScript[] raySensors2D, detectors2D, planers2D, casters2D, bullets2D;
    }
}
#endif
