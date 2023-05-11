using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023Template
{
    public class Camera
    {
        public (int, int, int) position; 
        public (int, int, int) direction;
        public (int, int, int) upDirection;
        public Surface plane;

        public Camera() : this((0,0,0), (0,0,1), (0, 1, 0)) { }

        public Camera((int, int, int) p, (int, int, int) d, (int, int, int) u)
        {
            position = p;
            direction = d;
            upDirection = u;
        }
    }
}
