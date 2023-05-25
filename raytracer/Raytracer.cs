using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    public class Raytracer
    {
        private Surface screen;
        private Camera camera;
        private float maxRayDistance;
        private Scene scene;
        public KeyboardState keyboard;

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
            scene.primitives.Add(new Sphere(new Vector3(0, 0, 7), 1f, new Vector3(255, 255, 255)));

            scene.lights.Add(new Light(new Vector3(3, 2, 2), 4000, new Vector3(255, 255, 255)));

            scene.primitives.Add(new Plane(new Vector3(0, 1f, 0), 0f, new Vector3(0, -1, 5), new Vector3(100, 100, 100)));
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 1f);
            maxRayDistance = 10f;
        }

        public void Render()
        {
            if (keyboard.IsKeyDown(Keys.W))
            {
                camera.position.Z += 0.5f;
            }
            else if (keyboard.IsKeyDown(Keys.S))
            {
                camera.position.Z -= 0.5f;
            }
            else if (keyboard.IsKeyDown(Keys.D))
            {
                camera.position.X += 0.5f;
            }
            else if (keyboard.IsKeyDown(Keys.A))
            {
                camera.position.X -= 0.5f;
            }

            camera.updatePosition();

            for (int x = 0;  x < screen.width/2; x++)
            {
                for (int y = 0; y < screen.height; y++)
                {
                    int position = x + y * screen.width + screen.width / 2;

                    Vector3 u = camera.p1 - camera.p0;
                    Vector3 v = camera.p2 - camera.p0;

                    float a = (float)x / (float)(screen.width/2);
                    float b = (float)y / (float)(screen.height);

                    Vector3 point = camera.p0 + a * u + b * v;
                    Vector3 Direction = Vector3.Normalize(point - camera.position);

                    Ray ray = new Ray(camera.position, Direction, maxRayDistance);

                    //Debug rays
                    
                    if (x % 20 == 0 && y == 0)
                    {
                        screen.Line(tx(camera.position.X), ty(camera.position.Z), tx(camera.position.X + Direction.X * maxRayDistance), ty(camera.position.Z + Direction.Z * maxRayDistance), 0x473f0a);
                    }

                    Intersection? closestIntersection = null;
                    foreach (Primitive primitive in scene.primitives)
                    {
                        Intersection intersection = new Intersection(ray, primitive);
                        // als de distance kleiner of gelijk aan maxdistance dan is er een intersection
                        // die primitive heeft een kleur. de kleur kan je gooien naar die pixel.
                        // screen.pixels[position] = die kleur
                        if (intersection.distance == 0) continue;

                        if (closestIntersection is null || closestIntersection.distance > intersection.distance)
                        {
                            closestIntersection = intersection;
                        }

                        Vector3 primaryIntersection = closestIntersection.ray.D * closestIntersection.distance;

                        //Lights
                        bool lightBlocked = false;
                        Vector3 shadowColor = new Vector3(0, 0, 0);
                        foreach (Light light in scene.lights) {

                            Vector3 LightDirection = primaryIntersection - light.position;
                            Vector3 LightDirectionNormalized = Vector3.Normalize(LightDirection);

                            float maxShadowRayDistance = 100f; //needs to be calculated (restrictions on t)

                            Ray shadowRay = new Ray(primaryIntersection, LightDirectionNormalized, maxShadowRayDistance);

                            Intersection? closestShadow = null;
                            foreach (Primitive primitiveObject in scene.primitives)
                            {
                                Intersection shadowIntersection = new Intersection(shadowRay, primitiveObject);
                                if (shadowIntersection.distance == 0) continue;

                                if (closestShadow is null || closestShadow.distance > shadowIntersection.distance)
                                {
                                    closestShadow = shadowIntersection;
                                }

                                if (closestShadow.distance < -0.01) {
                                    /*if (x % 20 == 0)
                                    {
                                        screen.Line(tx(primaryIntersection.X), ty(primaryIntersection.Z), tx(LightDirection.X * -maxShadowRayDistance), ty(LightDirection.Z * -maxShadowRayDistance), 0xccb20c);
                                    }*/

                                    //Light is blocked -> shadow
                                    lightBlocked = true;
                                }
                            }
                            shadowColor = CalculateDiffusion(primitive, light, primaryIntersection);
                        }

                        if (lightBlocked)
                        {
                            screen.pixels[position] = color(shadowColor);                
                        }
                        else {
                            screen.pixels[position] = color(shadowColor);
                        }
                    }
                } 
            }
        }

        Vector3 CalculateDiffusion(Primitive primitive, Light light, Vector3 primaryIntersection) {
            switch (primitive) {
                case Plane p: {
                        float Lradiance = light.intensity * (1 / (float)Math.Pow(Vector3.Distance(primaryIntersection, light.position), 2));

                        Vector3 NfromCenterToIntersection = p.normal;

                        float angle = Vector3.CalculateAngle((light.position - primaryIntersection), NfromCenterToIntersection);

                        Vector3 ReflectedLight = new Vector3(((light.color.X / 255) * (p.color.X / 255)) * 1, ((light.color.Y / 255) * (p.color.Y / 255)) * 1, ((light.color.Z / 255) * (p.color.Z / 255)) * 1);
                        
                        if (angle > 90)
                        {
                            angle = 0;
                        }

                        Vector3 PixelColor = Vector3.ComponentMin(Lradiance * Math.Max(0, (float)Math.Cos(angle)) * ReflectedLight, new Vector3(255, 255, 255));

                        return PixelColor;
                    }
                case Sphere s: {
                        float Lradiance = light.intensity * (1 / (float)Math.Pow(Vector3.Distance(primaryIntersection, light.position), 2));

                        Vector3 NfromCenterToIntersection = primaryIntersection - s.position;
                        //screen.Line(tx(NfromCenterToIntersection.X), ty(NfromCenterToIntersection.Z), tx(s.position.X), ty(s.position.Z), 0xffffff);

                        float angle = Vector3.CalculateAngle((light.position - primaryIntersection), NfromCenterToIntersection);

                        Vector3 ReflectedLight = new Vector3(((light.color.X/255) * (s.color.X / 255))*1, ((light.color.Y / 255) * (s.color.Y / 255)) * 1, ((light.color.Z / 255) * (s.color.Z / 255)) * 1);

                        //Debug.WriteLine((light.color, s.color, ReflectedLight));

                        if (angle > 90) { 
                            angle = 0;
                        }


                        Vector3 PixelColor = Vector3.ComponentMin(Lradiance * Math.Max(0, (float)Math.Cos(angle)) * ReflectedLight, new Vector3(255,255,255));

                        return PixelColor;
                        
                    }
            }
            return new Vector3(0, 0, 0);

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
                    case Plane p:
                        
                        break;
                }
            }
            foreach (var light in scene.lights)
            {
                switch (light)
                {
                    case Light l:
                        for (int i = 0; i <= 1000; i++)
                        {
                            float x = l.position.X + 0.1f * (float)Math.Cos(180 / (Math.PI * i / 360));
                            float y = l.position.Z + 0.1f * (float)Math.Sin(180 / (Math.PI * i / 360));

                            screen.Plot(tx(x), ty(y), color(l.color));
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
