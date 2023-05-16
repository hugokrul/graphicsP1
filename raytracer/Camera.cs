using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Camera
    {
        public Vector3 position; 
        public Vector3 direction;
        public Vector3 upDirection;
        public float fov;
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;

        public Camera(Vector3 position, Vector3 direction, Vector3 upDirection, float fov)
        {
            this.position = position;
            this.direction = direction;
            this.upDirection = upDirection;
            this.fov = fov;
            Vector3 right = Vector3.Cross(direction, upDirection);
            Vector3 center = position + fov * direction;
            p0 = center + upDirection + right;
            p1 = center + upDirection - right;
            p2 = center - upDirection + right;

        }
    }
}
