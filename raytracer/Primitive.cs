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
        public string type;

        public Primitive(Vector3 color)
        {
            this.color = color;
        }

        
    }

    public class Sphere : Primitive
    {
        public Vector3 position;
        public float radius;
        

        public Sphere(Vector3 position, float radius, Vector3 color) : base(color)
        {
            this.position = position;
            this.radius = radius;
            this.type = "sphere";
        }
    }

    public class Plane : Primitive
    {
        public Vector3 normal;
        public Vector3 point;
        public float distance;

        public Plane(Vector3 normal, float distance, Vector3 point, Vector3 color) : base(color)
        {
            this.normal = normal;
            this.point = point;
            this.distance = distance;
            this.type = "plane";
        }
    }
}
