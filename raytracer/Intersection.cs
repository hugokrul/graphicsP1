﻿using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Intersection
    {
        // Distance can be null
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
            float sphereRadiusSquared = sphere.radius * sphere.radius;

            if (lengthSquared > sphereRadiusSquared) {
               //No intersection
                return;
            };

            // The distance between ray.E and the intersectionpoint of the sphere.
            distance -= (float)Math.Sqrt(sphereRadiusSquared - lengthSquared);

            // if the distance is bigger then the maxRayDistance (10f)
            // ray.t is initially set to maxRayDistance
            if ((distance < ray.t) && (distance > 0))
            {
                ray.t = distance;
                this.distance = distance;
                Vector3 intersection = ray.E + ray.D * distance;
                this.position = intersection;
                this.normal = Vector3.Normalize(intersection - sphere.position);
            }
        }

        public void intersectWithPlane(Plane p, Ray ray)
        {
            // https://samsymons.com/blog/math-notes-ray-plane-intersection/
            // The above website helped me to calculate the distance 
            // The function we need to solve is:
            /* 
                ((planePoint - rayÓrigin) Dot planeNormal) / (rayDirection Dot planeNormal)
            */
            // First calculate the denominator, if that is zero, 
            var denominator = Vector3.Dot(p.normal, ray.D);

            // make sure we don't get negative distances.
            if (Math.Abs(denominator) > 0.001f)
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

                    if (p.Texture) //if texturing is enabled
                    {
                        //Get the original point of the texturing plane
                        var op = ray.E + ray.D * t;
                        //The ScalarU is the dot product of the original point and the Texturing U
                        //The texturingU and V are the U and V components of the plane
                        p.scalarU = Vector3.Dot(op, p.TexturingU);
                        //To eliminate 2 rows lining up in the middle after going into the negative numbers add one and make the negative number positive
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
            // makes planes from each sides of the triangle. this way it's easier to calculate if ray hits the triangle or not.
            Plane plane = new Plane(Vector3.Cross(triangle.edge1, triangle.edge2).Normalized(), triangle.vert0, new Vector3(255, 0, 0), 1, 0, false);
            var intersection = new Intersection(ray, plane);

            // if there is no distance there is no intersection
            if (intersection.distance == null)
            {
                return;
            }

            var planeNormal = plane.normal;
            var intersectionPoint = ray.E + ray.D * intersection.distance;

            // if the ray is outside of the triangle
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
