using UnityEngine;

namespace Scripts.Util
{
    [ExecuteInEditMode]
    public class SetMaterialToAllChildren : MonoBehaviour
    {
        [SerializeField] private Material _material;

        void Update()
        {
            if (_material != null)
            {
                var renderers = GetComponentsInChildren<MeshRenderer>();
                foreach (var renderer in renderers)
                    renderer.material = _material;

                _material = null;
            }
        }
    }
}
