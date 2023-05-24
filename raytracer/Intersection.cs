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
            this.distance = 0;

            switch (primitive)
            {
                case Sphere s:
                    intersectWithSphere(s, ray);
                    break;
                case Plane p:
                    intersectWithPlane(p, ray);
                    break;
            }
        }

        public void intersectWithSphere(Sphere s, Ray ray)
        {
            // page 23 van slides 4 intro to raytracing
            Vector3 A = ray.E;
            Vector3 B = ray.D;
            float t = ray.t;
            Vector3 C = s.position;
            float r2 = s.radius * s.radius;

            float a = Vector3.Dot(B, B);
            float b = 2 * Vector3.Dot(B, C- A);
            float c = Vector3.Dot(C - A, C - A) - r2;
            float disc = (b * b) - (4.0f * a * c);
            if (disc > 0)
            {
                this.distance = (float)Math.Min((-b - Math.Sqrt(disc)) / (2 * a), (-b + Math.Sqrt(disc)) / (2 * a));
                ray.t = (float)Math.Min((-b - Math.Sqrt(disc)) / (2 * a), (-b + Math.Sqrt(disc)) / (2 * a));
            }


        }

        public void intersectWithPlane(Plane p, Ray ray)
        {
            // page 21 van slides 4 intro to raytracing
            var denominator = Vector3.Dot(p.normal, ray.D);

            if (Math.Abs(denominator) > 0.001f)
            {
                var difference = p.point - ray.E;
                var t = Vector3.Dot(difference, p.normal) / denominator;

                if (t > 0.0001f)
                {
                    this.distance = t;
                    ray.t = t;
                }
            }
        }
    }
}
