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
        public Vector3 right;
        private float rotationSpeed = 0.03f;
        

        public Camera(Vector3 position, Vector3 direction, Vector3 upDirection, float alpha)
        {
            this.position = position;
            this.direction = direction;
            this.upDirection = upDirection;
            this.fov = (float)Math.Abs(Math.Tan(alpha/2));
            this.right = Vector3.Cross(direction, upDirection);
            Vector3 center = position + fov * direction;
            p0 = center + upDirection + right;
            p1 = center + upDirection - right;
            p2 = center - upDirection + right;

            
        }

        public void updatePosition()
        {
            this.right = Vector3.Cross(direction, upDirection);
            Vector3 center = this.position + this.fov * this.direction;
            this.p0 = center + this.upDirection + right;
            this.p1 = center + this.upDirection - right;
            this.p2 = center - this.upDirection + right;
        }

        public void rotateHorizontal(float rotateX)
        {
            Vector3 toRotateAround = this.upDirection.Y > 0 ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
            Quaternion rotateHorizontal = new Quaternion(new Vector3(0, 1, 0) * rotationSpeed * rotateX);
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rotateHorizontal));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, rotateHorizontal));
        }

        public void rotateVertical(float rotateY)
        {
            Quaternion rotateVertical = new Quaternion(this.right * rotationSpeed * rotateY);
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rotateVertical));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, rotateVertical));
        }

        public void roll(float roll)
        {
            Quaternion rollPitch = new Quaternion(this.direction * rotationSpeed * roll);
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rollPitch));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, rollPitch));
        }
    }
}
