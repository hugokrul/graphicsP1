using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Vector3 position;

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

        public void intersectWithSphere(Sphere sphere, Ray ray)
        {
            Vector3 c = sphere.position - ray.E;
            float t = Vector3.Dot(c, ray.D);
            Vector3 q = c - t * ray.D;
            float p2 = q.LengthSquared;

            if (p2 > (sphere.radius * sphere.radius)) {
               //No intersection
                return;
            } ;
            t -= (float)Math.Sqrt((sphere.radius * sphere.radius) - p2);

            if ((t < ray.t) && (t > 0.001))
            {
                ray.t = t;
                this.distance = t;
                Vector3 intersection = ray.E + ray.D * this.distance;
                this.position = intersection;
                this.normal = Vector3.Normalize(intersection - sphere.position);
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

                    Vector3 intersection = ray.E + ray.D * this.distance;
                    this.position = intersection;
                }
            }
        }
    }
}
