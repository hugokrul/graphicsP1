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
        public Vector2 oldMousePosition;
        

        public Camera(Vector3 position, Vector3 direction, Vector3 upDirection, float fov)
        {
            this.position = position;
            this.direction = direction;
            this.upDirection = upDirection;
            this.fov = fov;
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

        public void updateMouse(Vector2 newMousePosition)
        {
            Vector2 mouseDelta = newMousePosition - this.oldMousePosition;
            if (mouseDelta.Length > 50.0f)
            {
                this.oldMousePosition = newMousePosition;
                return;
            }

            float rotationSpeed = 0.03f;

            // Vector3 toRotateAround = Vector3.Cross(this.direction, this.upDirection) * -1;
            Quaternion rotateHorizontal = new Quaternion(this.upDirection * rotationSpeed * mouseDelta.X);
            Quaternion rotateVertical = new Quaternion(new Vector3(1, 0, 0) * rotationSpeed * mouseDelta.Y);
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rotateHorizontal * rotateVertical));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, new Quaternion(new Vector3(1, 0, 0) * rotationSpeed * mouseDelta.Y)));

            this.oldMousePosition = newMousePosition;
        }
    }
}
