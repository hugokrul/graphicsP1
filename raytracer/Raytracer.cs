using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Raytracer
    {
        private Surface screen;
        private Camera camera;
        private float maxRayDistance;
        private Scene scene;

        public Raytracer(MyApplication app)
        {
            screen = app.screen;
            scene = new Scene();
            scene.primitives.Add(new Sphere(new Vector3(-2.5f, 0, 5), 1f, new Vector3(255, 0, 0)));
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 1f);
            maxRayDistance = 10f;
        }

        public void Render()
        {

        }

        public void RenderDebug()
        {
            
        }
    }

}
