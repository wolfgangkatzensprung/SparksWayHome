using UnityEngine;

public class ShowGizmos : MonoBehaviour
{
    public enum GizmoType
    {
        Sphere,
        WireSphere,
        Cube,
        WireCube
    }

    public GizmoType gizmoType = GizmoType.Sphere;
    public Color gizmoColor = Color.yellow;
    public float gizmoRadius = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        switch (gizmoType)
        {
            case GizmoType.Sphere:
                Gizmos.DrawSphere(transform.position, gizmoRadius);
                break;
            case GizmoType.WireSphere:
                Gizmos.DrawWireSphere(transform.position, gizmoRadius);
                break;
            case GizmoType.Cube:
                Gizmos.DrawCube(transform.position, Vector3.one * gizmoRadius);
                break;
            case GizmoType.WireCube:
                Gizmos.DrawWireCube(transform.position, Vector3.one * gizmoRadius);
                break;
        }
    }
}
