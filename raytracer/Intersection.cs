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

        public Intersection(int distance, Primitive primitive)
        {
            this.distance = distance;
            this.primitive = primitive;
        }

//        public Intersection intersectSphere(Sphere sphere, Ray ray)
//        {
//            Vector3 center = sphere.center - ray.E;
//            float dot = Vector3.Dot(center, Ray.D);
//            Vector3 q = center - dot * ray.D;
//            float p2 = q.LengthSquared();
//
//            if (p2 > sphere.r2) return;
//           dot -= Math.Sqrt(sphere.r2 - p2);
//
//            if ((t < ray.t) && (t > 0)) ray.t = t;
//        }
    }
}
