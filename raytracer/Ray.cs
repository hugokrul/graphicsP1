using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Ray
    {
        public Vector3 E;
        public Vector3 D;
        public float t;

        public Ray(Vector3 E, Vector3 D, float t)
        {
            this.E = E;
            this.D = D;
            this.t = t;
        }
    }
}
