using System.Linq;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    MeshRenderer mr;

    [SerializeField, Tooltip("Leave empty if no material change from Activation is intended! Or if it already has a script that does this!")]
    Material activatedMaterial;

    Material deactivatedMaterial;

    [SerializeField, Tooltip("Index of the Glow Material in the Renderer Component")]
    int glowMaterialIndex = 2;

    Activatable activatable;

    private void Awake()
    {
        activatable = GetComponentInParent<Activatable>();
        if (activatable == null)    // Only get own Activatable if its not a nested object that is controlled by parent
        {
            activatable = GetComponent<Activatable>();
        }

        if (activatedMaterial == null) return;

        mr = GetComponent<MeshRenderer>();
        deactivatedMaterial = mr.materials[glowMaterialIndex];
    }

    private void OnEnable()
    {
        if (activatable == null) return;

        activatable.onActivatableEnabled += SetActivatedMaterial;
        activatable.onActivatableDisabled += SetDeactivatedMaterial;
    }

    private void OnDisable()
    {
        if (activatable == null) return;

        activatable.onActivatableEnabled -= SetActivatedMaterial;
        activatable.onActivatableDisabled -= SetDeactivatedMaterial;
    }

    private void SetActivatedMaterial()
    {
        if (activatedMaterial == null) return;
        var materials = mr.materials.ToList(); ;
        materials[glowMaterialIndex] = activatedMaterial;

        mr.SetMaterials(materials);
    }

    private void SetDeactivatedMaterial()
    {
        if (activatedMaterial == null) return;
        var materials = mr.materials.ToList();
        materials[glowMaterialIndex] = deactivatedMaterial;

        mr.SetMaterials(materials);
    }
}
