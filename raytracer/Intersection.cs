using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Intersection
    {
        public float distance;        
        public Primitive primitive;
        public Vector3 normal;
        public Ray ray;

        public Intersection(Ray ray, Primitive primitive)
        {
            this.ray = ray;
            this.primitive = primitive;

            switch (primitive)
            {
                case Sphere s:
                    intersectWithSphere(s);
                    break;
                case Plane p:
                    intersectWithPlane();
                    break;
            }
        }

        public void intersectWithSphere(Sphere s)
        {
            // page 23 van slides 4 intro to raytracing
            Vector3 A = ray.E;
            Vector3 B = ray.D;
            float t = ray.t;
            Vector3 C = s.position;
            float r2 = s.radius * s.radius;

            float a = Vector3.Dot(B, B);
            float b = 2 * Vector3.Dot(B, A - C);
            float c = Vector3.Dot(A - C, A - C) - r2;

            float disc = b * b - 4 * a * c;
            if (disc < 0)
            {
                return;
            } else
            {
                this.distance = (float)Math.Min((-b - Math.Sqrt(disc)) / (2 * a), (-b + Math.Sqrt(disc)) / (2 * a));
                this.primitive = s;
                this.normal = Vector3.Normalize(ray.E);
            }


        }

        public void intersectWithPlane()
        {
            // page 21 van slides 4 intro to raytracing
        }
    }
}
