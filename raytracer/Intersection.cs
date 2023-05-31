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
            // Calculate the vector from the origin of the ray to the sphere center
            Vector3 rayVector = sphere.position - ray.E;

            float distance = Vector3.Dot(rayVector, ray.D);
            Vector3 length = rayVector - distance * ray.D;
            float lengthSquared = length.LengthSquared;

            if (lengthSquared > (sphere.radius * sphere.radius)) {
               //No intersection
                return;
            };
            distance -= (float)Math.Sqrt((sphere.radius * sphere.radius) - lengthSquared);

            // if the distance is bigger then the maxRayDistance (10f)
            // ray.t is initially set to maxRayDistance
            if ((distance < ray.t) && (distance > 0))
            {
                ray.t = distance;
                this.distance = distance;
                Vector3 intersection = ray.E + ray.D * this.distance;
                this.position = intersection;
                this.normal = Vector3.Normalize(intersection - sphere.position);
            }
       

        }

        public void intersectWithPlane(Plane p, Ray ray)
        {
            var denominator = Vector3.Dot(p.normal, ray.D);

            if (Math.Abs(denominator) > 0.001f)
            {
                var difference = p.point - ray.E;
                var t = Vector3.Dot(difference, p.normal) / denominator;

                if (t > 0)
                {
                    this.distance = t;
                    ray.t = t;

                    Vector3 intersection = ray.E + ray.D * this.distance;
                    this.position = intersection;
                    this.normal = p.normal;
                }
            }
        }
    }
}
