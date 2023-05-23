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
                    intersectWithPlane();
                    break;
            }
        }

        public void i(Sphere sphere, Ray ray)
        {
            Vector3 c = sphere.position - ray.E;
            float t = Vector3.Dot(c, ray.D);
            Vector3 q = c - t * ray.D;
            float p2 = q.LengthSquared;

            if (p2 > sphere.radius*sphere.radius) return;

            t -= (float)Math.Sqrt(sphere.radius*sphere.radius - p2);
            Console.WriteLine(t);

            if ((t < ray.t) && (t > 0))
            {
                ray.t = t;
                this.distance = t;
                Console.WriteLine(t);
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

        public void intersectWithPlane()
        {
            // page 21 van slides 4 intro to raytracing
        }
    }
}
