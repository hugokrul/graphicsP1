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
                case Triangle t:
                    intersectWithTriangle(t, ray);
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

        public void intersectWithTriangle(Triangle t, Ray ray)
        {
            var pvec = Vector3.Cross(ray.D, t.edge2);

            var det = Vector3.Dot(t.edge1, pvec);

            if (!(det > -0.001 && det < 0.001))
            {
                var invDet = 1f / det;

                var tvec = ray.E - t.vert0;

                var x = Vector3.Dot(tvec, pvec) * invDet;

                if (!(x < 0 || x > 1))
                {
                    var qvec = Vector3.Cross(tvec, t.edge1);

                    var y = Vector3.Dot(ray.D, qvec) * invDet;

                    if (!(y < 0 || x + y > 1))
                    {
                        var z = Vector3.Dot(t.edge2, qvec) * invDet;

                        this.distance = (new Vector3(x, y, z) - ray.E).Length;
                        ray.t = this.distance;

                        this.position = new Vector3(x, y, z);
                    }
                }
            }
        }
    }
}
