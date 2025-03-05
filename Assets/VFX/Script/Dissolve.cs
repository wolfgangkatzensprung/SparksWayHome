using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    Player player;
    public SkinnedMeshRenderer skinnedMesh;
    public MeshRenderer dutt;
    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;

    private List<Material> skinnedMaterials;

    Coroutine dissolveRoutine;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.OnDeath += StartDissolve;
        player.OnRespawn += ResetDissolve;
    }

    private void OnDisable()
    {
        player.OnDeath -= StartDissolve;
        player.OnRespawn -= ResetDissolve;
    }

    void Start()
    {
        if (skinnedMesh != null)
        {
            skinnedMaterials = skinnedMesh.materials.ToList();
            skinnedMaterials.Add(dutt.material);
        }
    }

    private void StartDissolve()
    {
        dissolveRoutine = StartCoroutine(DissolveRoutine());
    }

    private void ResetDissolve(Player _)
    {
        StopCoroutine(dissolveRoutine);

        if (skinnedMaterials.Count > 0)
        {
            for (int i = 0; i < skinnedMaterials.Count; i++)
            {
                skinnedMaterials[i].SetFloat("_Dissolve_Amount", 0f);
            }
        }
    }

    IEnumerator DissolveRoutine()
    {
        if(skinnedMaterials.Count > 0)
        {
            float counter = 0;
            while (skinnedMaterials[0].GetFloat("_Dissolve_Amount") < 1)
            {
                counter += dissolveRate;
                for(int i=0; i<skinnedMaterials.Count; i++)
                {
                    skinnedMaterials[i].SetFloat("_Dissolve_Amount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
