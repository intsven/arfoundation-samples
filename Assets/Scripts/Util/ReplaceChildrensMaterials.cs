using UnityEngine;

namespace Scripts.Util
{
    [ExecuteInEditMode]
    public class ReplaceChildrensMaterials : MonoBehaviour
    {
        public MaterialReplacement[] MaterialReplacements;
        public bool Execute;

        void Update()
        {
            if (Execute)
            {
                var renderers = GetComponentsInChildren<MeshRenderer>();
                foreach (var renderer in renderers)
                {
                    var materials = renderer.materials;
                    bool didChange = false;
                    for (var i = 0; i < renderer.materials.Length; i++)
                    {
                        foreach (var replacement in MaterialReplacements)
                        {
                            if (renderer.materials[i].name.Contains(replacement.MaterialName))
                            {
                                materials[i] = replacement.NewMaterial;
                                didChange = true;
                            }
                        }                                                 
                    }
                    if (didChange)
                        renderer.materials = materials;                    
                }
                Execute = false;
            }
        }
    }
}
