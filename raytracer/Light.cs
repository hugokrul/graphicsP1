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
        public int intensity;
        public Vector3 color;

        public Light(Vector3 p, int i, Vector3 c)
        {
            position = p;
            intensity = i;
            color = c;
        }
    }
}
