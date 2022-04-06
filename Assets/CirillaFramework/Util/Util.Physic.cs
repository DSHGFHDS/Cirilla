using UnityEngine;

namespace Cirilla
{
    public partial class Util
    {
        public static float UnSqrtDistance(Vector2 start, Vector2 end){
            return Mathf.Pow(end.x - start.x, 2) + Mathf.Pow(end.y - start.y, 2);
        }

        public static float UnSqrtDistance(Vector3 start, Vector3 end) {
            return Mathf.Pow(end.x - start.x, 2) + Mathf.Pow(end.y - start.y, 2) + Mathf.Pow(end.z - start.z, 2);
        }
    }
}