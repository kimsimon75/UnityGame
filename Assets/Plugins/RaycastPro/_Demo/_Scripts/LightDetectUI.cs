using System.Collections;
using System.Collections.Generic;
using RaycastPro.Detectors;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class LightDetectUI : MonoBehaviour
    {
        public LightDetector detector;
        public MeshRenderer agentLight;

        public Image lightUI;
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            lightUI.color = detector.Performed ? Color.green : Color.red;

            agentLight.materials[1].SetColor(EmissionColor, lightUI.color);
            agentLight.materials[2].SetColor(EmissionColor, lightUI.color);
        }
    }
}
