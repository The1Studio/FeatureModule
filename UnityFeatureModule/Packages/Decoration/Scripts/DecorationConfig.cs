namespace TheOneStudio.GameFeature.Decoration
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "DecorationConfig", menuName = "DecorationFeature/DecorationConfig", order = 1)]
    public class DecorationConfig : ScriptableObject
    {
        public Vector3 CameraPosition;
        public Vector3 CameraRotation;
    }
}