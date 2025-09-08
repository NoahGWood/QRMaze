using UnityEngine;

namespace spookymfg.qrng {
    public class TileGenerator : MonoBehaviour
    {
        [Header("Generator Settings")]
//        [SerializeField]
//        NoiseMapGeneration NoiseMapGeneration;
        [SerializeField]
        private float scale;
        [SerializeField]
        [Header("Mesh Settings")]
        private MeshCollider meshCollider;
        [SerializeField]
        private MeshRenderer meshRenderer;
        [SerializeField]
        private MeshFilter meshFilter;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

