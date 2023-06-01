using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Primitive
    {
        public Vector3 color;
        public float glossiness;
        public string type;
        public float pureSpecular;

        public Primitive(Vector3 color)
        {
            this.color = color;
        }
    }

    public class Sphere : Primitive
    {
        public Vector3 position;
        public float radius;

        public float scalarU;
        public float scalarV;
        public Vector3 TexturingU;
        public Vector3 TexturingV;

        public bool Texture = false;

        public Sphere(Vector3 position, float radius, Vector3 color, float glossiness, float pureSpecular, bool texture) : base(color)
        {
            this.position = position;
            this.radius = radius;
            this.type = "sphere";
            this.glossiness = glossiness;
            this.pureSpecular = pureSpecular;

            this.Texture = texture;
        }
    }

    public class Plane : Primitive
    {
        public Vector3 normal;
        public Vector3 point;
        public float distance;

        public float scalarU;
        public float scalarV;
        public Vector3 TexturingU;
        public Vector3 TexturingV;

        public bool Texture = false;

        public Plane(Vector3 normal, float distance, Vector3 point, Vector3 color, float glossiness, float pureSpecular, bool texture) : base(color)
        {
            this.normal = normal;
            this.point = point;
            this.distance = distance;
            this.type = "plane";
            this.glossiness = glossiness;
            this.pureSpecular = pureSpecular;

            this.TexturingU = Vector3.Normalize(new Vector3(this.normal.Y, -this.normal.X, 0));
            this.TexturingV = Vector3.Cross(this.normal, TexturingU);
            this.Texture = texture;
        }
    }

    public class Triangle : Primitive
    {
        public Vector3 vert0, vert1, vert2;
        public Vector3 edge1;
        public Vector3 edge2;
        public Vector3 normal;

        

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 color, float glossiness, float pureSpecular) : base(color)
        {
            this.vert0 = v0;
            this.vert1 = v1;
            this.vert2 = v2;
            this.type = "triangle";
            this.glossiness = glossiness;
            this.pureSpecular = pureSpecular;

            this.edge1 = vert1 - vert0;
            this.edge2 = vert2 - vert0;

            this.normal = Vector3.Cross(this.edge1, this.edge2);

            
        }
    }
}
