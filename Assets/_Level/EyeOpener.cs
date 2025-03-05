using System.Collections.Generic;
using UnityEngine;

public class EyeOpener : MonoBehaviour
{
    MeshRenderer mr;

    [SerializeField]
    Material activatedMaterial;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    public void SetActivatedMaterial()
    {
        if (activatedMaterial == null) return;

        List<Material> materials = new()
        {
            activatedMaterial
        };

        mr.SetMaterials(materials);
    }
}
