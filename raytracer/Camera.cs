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

            // Calculate from angle of degrees, to distance to screen.
            this.fov = (float)Math.Abs(Math.Tan(alpha/2));
            this.right = Vector3.Cross(direction, upDirection);
            Vector3 center = position + fov * direction;
            p0 = center + upDirection + right;
            p1 = center + upDirection - right;
            p2 = center - upDirection + right;
        }

        // Updates the screen to move with the camera
        public void updatePosition()
        {
            this.right = Vector3.Cross(direction, upDirection);
            Vector3 center = this.position + this.fov * this.direction;
            this.p0 = center + this.upDirection + right;
            this.p1 = center + this.upDirection - right;
            this.p2 = center - this.upDirection + right;
        }

        // Function to calculate horizontal rotation
        public void rotateHorizontal(float rotateX)
        {
            // Calculate the rotation angle to rotate around.
            // If you rotate horizontally, you always rotate around the Y axis.
            Quaternion rotateHorizontal = new Quaternion(new Vector3(0, 1, 0) * rotationSpeed * rotateX);

            // Rotate both the direction and the updirection around that angle.
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rotateHorizontal));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, rotateHorizontal));
        }

        // Function to calculate vertical rotation
        public void rotateVertical(float rotateY)
        {
            // Calculate the rotation angle to rotate around
            // If you rotate vertically you rotate around the axis to the right of you.
            Quaternion rotateVertical = new Quaternion(this.right * rotationSpeed * rotateY);
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rotateVertical));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, rotateVertical));
        }

        // Function to calculate the roll rotation
        public void roll(float roll)
        {
            // If you roll, you rotate around the direction in which you are looking.
            Quaternion rollPitch = new Quaternion(this.direction * rotationSpeed * roll);
            this.direction = Vector3.Normalize(Vector3.Transform(this.direction, rollPitch));
            this.upDirection = Vector3.Normalize(Vector3.Transform(this.upDirection, rollPitch));
        }
    }
}
