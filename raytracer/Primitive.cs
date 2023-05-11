using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023Template
{
    public class Primitive
    {
        public float color = 0x000000;

        public Primitive(float c)
        {
            color = c;
        }
    }

    public class Sphere : Primitive
    {
        public int position;
        public int radius;

        public Sphere(int p, int r, float c) : base(c)
        {
            position = p;
            radius = r;
        }
    }

    public class Plane : Primitive
    {
        public int normal;
        public int distance;

        public Plane(int n, int d, float c) : base(c)
        {
            normal = n;
            distance = d;
        }
    }
}
