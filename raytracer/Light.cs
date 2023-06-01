using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Light
    {
        public Vector3 position;
        public float intensity;
        public Vector3 color;

        public Light(Vector3 p, int i, Vector3 c)
        {
            position = p;
            intensity = i;
            color = c;
        }
    }
    public class SpotLight : Light
    {
        public Vector3 Direction;
        public float maxAngle;

        public SpotLight(Vector3 p, int i, Vector3 c, Vector3 Direction, float maxAngle) : base(p, i, c)
        {
            this.Direction = Direction;
            this.maxAngle = maxAngle;
        }
    }
}
