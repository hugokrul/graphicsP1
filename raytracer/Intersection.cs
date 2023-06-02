﻿using System;
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
        public float? distance;        
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
                Vector3 intersection = ray.E + ray.D * distance;
                this.position = intersection;
                this.normal = Vector3.Normalize(intersection - sphere.position);

                /*if (sphere.Texture) //texture
                {
                    var op = new Vector3((sphere.position.X + sphere.radius*));
                    s.scalarU = Vector3.Dot(op, p.TexturingU);
                    p.scalarV = Vector3.Dot(op, p.TexturingV);
                }*/
            }
       

        }

        public void intersectWithPlane(Plane p, Ray ray)
        {
            var denominator = Vector3.Dot(p.normal, ray.D);

            if (Math.Abs(denominator) > 0.01f)
            {
                var difference = p.point - ray.E;
                var t = Vector3.Dot(difference, p.normal) / denominator;

                if (t > 0)
                {
                    this.distance = t;
                    ray.t = t;

                    Vector3 intersection = ray.E + ray.D * t;
                    this.position = intersection;
                    this.normal = p.normal;

                    if (p.Texture) //texture
                    {
                        var op = ray.E + ray.D * t;
                        p.scalarU = Vector3.Dot(op, p.TexturingU);
                        if (p.scalarU < 0) {
                            p.scalarU = 1 + -p.scalarU;
                        }
                        p.scalarV = Vector3.Dot(op, p.TexturingV);
                        if (p.scalarV < 0)
                        {
                            p.scalarV = 1 + -p.scalarV;
                        }
                    }
                }
            }
        }

        public void intersectWithTriangle(Triangle triangle, Ray ray)
        {
            Plane plane = new Plane(Vector3.Cross(triangle.edge1, triangle.edge2).Normalized(), triangle.vert0, new Vector3(255, 0, 0), 1, 0, false);
            var intersection = new Intersection(ray, plane);

            if (intersection.distance == null)
            {
                return;
            }

            var planeNormal = plane.normal;
            var intersectionPoint = ray.E + ray.D * intersection.distance;

            if (
                Vector3.Dot(Vector3.Cross((triangle.vert1 - triangle.vert0), (Vector3)(intersectionPoint - triangle.vert0)), planeNormal) < 0 ||
                Vector3.Dot(Vector3.Cross((triangle.vert2 - triangle.vert1), (Vector3)(intersectionPoint - triangle.vert1)), planeNormal) < 0 ||
                Vector3.Dot(Vector3.Cross((triangle.vert0 - triangle.vert2), (Vector3)(intersectionPoint - triangle.vert2)), planeNormal) < 0
                )
            {
                return;
            }

            this.distance = intersection.distance;
            ray.t = (float)this.distance;
            this.normal = planeNormal;
        }
    }
}
