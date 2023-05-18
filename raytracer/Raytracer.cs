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

        public float top = -1f;
        public float left = -5f;
        public float scale = 10f;


        public Raytracer(MyApplication app)
        {
            screen = app.screen;
            scene = new Scene();
            scene.primitives.Add(new Sphere(new Vector3(-2.5f, 0, 5), 1f, new Vector3(255, 0, 0)));
            scene.primitives.Add(new Sphere(new Vector3(0, 0, 5), 1f, new Vector3(0, 255, 0)));
            scene.primitives.Add(new Sphere(new Vector3(2.5f, 0, 5), 1f, new Vector3(0, 0, 255)));
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 1f);
            maxRayDistance = 10f;
        }

        public void Render()
        {
            for (int x = 0;  x < screen.width/2; x++)
            {
                for (int y = 0; y < screen.height; y++)
                {
                    int position = x + y * screen.width + screen.width / 2;

                    Vector3 u = camera.p1 - camera.p0;
                    Vector3 v = camera.p2 - camera.p0;

                    int a = x / screen.width / 2;
                    int b = y / screen.height / 2;

                    Vector3 point = camera.p0 + a * u + b * v;
                    Vector3 direction = Vector3.Normalize(point - camera.position);


                    Ray ray = new Ray(camera.position, direction, maxRayDistance);

                    foreach (var primitive in scene.primitives)
                    {
                        Intersection intersection = new Intersection(ray, primitive);
                        // als de distance kleiner of gelijk aan maxdistance dan is er een intersection
                        // die primitive heeft een kleur. de kleur kan je gooien naar die pixel.
                        // screen.pixels[position] = die kleur
                        screen.pixels[position] = 255;
                    }
                } 
            }
        }

        public void RenderDebug()
        {
            // plot the camera
            screen.Plot(tx(camera.position.X), ty(camera.position.Z), 0xffffff);

            // use a line to visualize the screen plane
            screen.Line(tx(camera.p0.X), ty(camera.p0.Z), tx(camera.p1.X), ty(camera.p1.Z), 0xffffff);

            foreach (var primitive in scene.primitives)
            {
                switch (primitive)
                {
                    case Sphere s:
                        for (int i = 0; i <= 1000; i++)
                        {
                            float x = s.position.X + s.radius * (float)Math.Cos(180 / (Math.PI * i / 360));
                            float y = s.position.Z + s.radius * (float)Math.Sin(180 / (Math.PI * i / 360));

                            screen.Plot(tx(x), ty(y), color(s.color));
                        }
                        break;
                }
            }
        }

        public int color(Vector3 c)
        {
            return ((int)c.X << 16) + ((int)c.Y << 8) + (int)c.Z;
        }

        public int tx(float x)
        {
            x -= left;
            x *= Math.Min(screen.width, screen.height) / scale;
            return (int)x;
        }

        public int ty(float y)
        {
            float center = top + scale/2;
            float difference = center - y;
            y = center + difference;
            y -= top;
            y *= Math.Min(screen.height, screen.width) / scale;
            return (int)y;
        }
    }

}
