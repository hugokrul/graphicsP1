using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.VisualBasic;

namespace INFOGR2023Template
{
    public class Raytracer
    {
        public Surface screen;
        public Camera camera;
        public float maxRayDistance;
        public Scene scene;
        public KeyboardState keyboard;

        public float top = -1f;
        public float left = -5f;
        public float scale = 10f;
        public float rotationSpeed = 5f;
        public int debugX;
        public int debugY;

        public float ambientLightingAmount = 0.1f;


        public Raytracer(MyApplication app)
        {
            screen = app.screen;
            scene = new Scene();
            //Add primitives
            scene.primitives.Add(new Sphere(new Vector3(0, 0, 5), 1f, new Vector3(100, 100, 100), 3, 0, true));
            scene.primitives.Add(new Sphere(new Vector3(-2.5f, 0, 5), 1f, new Vector3(255, 0, 0), 3, 0.7f, false));
            scene.primitives.Add(new Sphere(new Vector3(2.5f, 0, 5), 1f, new Vector3(0, 0, 255), 1, 0, false));
            scene.primitives.Add(new Sphere(new Vector3(0, 0, 7), 1f, new Vector3(255, 255, 255), 1, 0, false));

            scene.lights.Add(new Light(new Vector3(4, 5, 2), 3, new Vector3(255, 255, 255)));
            scene.lights.Add(new Light(new Vector3(0, 6, 5), 5, new Vector3(255, 255, 255)));

           scene.primitives.Add(new Plane(new Vector3(0, 1f, 0), new Vector3(0, -1, 5), new Vector3(150, 150, 150), 0, 0, true));

           scene.primitives.Add(new Pyramide(new Vector3(2, -1, 7), new Vector3(4, -1, 7), new Vector3(3, -1, 9), new Vector3(3, 0, 8), new Vector3(255, 0, 0), 1, 0, this));

            camera = new Camera(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 0.1f);
            maxRayDistance = 10f;
        }

        public void Render()
        {
            
            if (keyboard.IsKeyDown(Keys.T))
            {
                camera.position = new Vector3(0, 0, 1);
                camera.direction = new Vector3(0, 0, 1);
                camera.upDirection = new Vector3(0, 1, 0);
            }
            while (keyboard.IsKeyDown(Keys.W))
            {
                camera.position += new Vector3(camera.direction.X, 0, camera.direction.Z) * 0.1f;
                break;
            }
            while (keyboard.IsKeyDown(Keys.S))
            {
                camera.position -= new Vector3(camera.direction.X, 0, camera.direction.Z) * 0.1f;
                break;
            }
            while (keyboard.IsKeyDown(Keys.D))
            {
                camera.position -= new Vector3(camera.right.X, 0, camera.right.Z) * 0.1f;
                break;
            }
            while (keyboard.IsKeyDown(Keys.A))
            {
                camera.position += new Vector3(camera.right.X, 0, camera.right.Z) * 0.1f;
                break;
            }
            while (keyboard.IsKeyDown(Keys.Space))
            {
                camera.position.Y += 0.1f;
                break;
            }
            while (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
            {
                camera.position.Y -= 0.1f;
                break;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                camera.rotateHorizontal(rotationSpeed);
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                camera.rotateHorizontal(-rotationSpeed);
            }
            else if (keyboard.IsKeyDown(Keys.Up))
            {
                camera.rotateVertical(rotationSpeed);
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                camera.rotateVertical(-rotationSpeed);
            }
            else if (keyboard.IsKeyDown(Keys.Q))
            {
                camera.roll(rotationSpeed);
            }
            else if (keyboard.IsKeyDown(Keys.E))
            {
                camera.roll(-rotationSpeed);
            }

            //go trough every pixel of the screen
            for (debugX = 0;  debugX < screen.width/2; debugX++)
            {
                for (debugY = 0; debugY < screen.height; debugY++)
                {
                    //define the position of the pixel in the screen
                    int position = debugX + debugY * screen.width + screen.width / 2;

                    Vector3 u = camera.p1 - camera.p0;
                    Vector3 v = camera.p2 - camera.p0;

                    float a = (float)debugX / (float)(screen.width / 2);
                    float b = (float)debugY / (float)(screen.height);

                    Vector3 point = camera.p0 + a * u + b * v;
                    Vector3 Direction = Vector3.Normalize(point - camera.position);

                    //Make a ray from the camera to the pixel of the screen
                    Ray ray = new Ray(camera.position, Direction, maxRayDistance);

                    //Get the pixel color of that pixel
                    Vector3 pixelColor = Trace(ray, 0, debugX);

                    //draw the pixelColor on the screen
                    screen.pixels[position] = color(pixelColor);
                }
            } 
        }

        Vector3 Trace(Ray ray, int bounce, int debugX) {
            Vector3 PixelColor = new Vector3(0, 0, 0); //black, initial color

            if (bounce < 10) { //maxBounces
                //Get the closest intersection of the ray with any primitive
                (Intersection closestIntersection, Primitive primitive) = CalculateClosestIntersection(ray);
            
                //If there is an intersection
                if (closestIntersection != null) {
                    Vector3 primaryIntersection = closestIntersection.position;
                    Vector3 NormalVector = closestIntersection.normal;

                    //Debug the secondary rays
                    if (bounce >= 1 && closestIntersection.distance < 8f && closestIntersection.ray.D.Y == 0 )
                    {
                        screen.Line(tx(closestIntersection.position.X), ty(closestIntersection.position.Z), tx(closestIntersection.ray.E.X), ty(closestIntersection.ray.E.Z), 0xffffff);
                    }

                    //Check if the ray will be reflected
                    if (primitive.pureSpecular > 0) {
                        //Get the standard color as if the primitive was not reflective
                        Vector3 materialColor = CalculateShading(primaryIntersection, primitive, debugX);
                        Vector3 CameraVector = primaryIntersection - camera.position;
                        Vector3 ReflectedVector = Vector3.Normalize(CameraVector - 2 * (Vector3.Dot(CameraVector, NormalVector)) * NormalVector);
                        Ray reflectedRay = new Ray(primaryIntersection, ReflectedVector, maxRayDistance);

                        //Trace the environment again with the reflected Ray and add this to the pixelColor
                        PixelColor = materialColor * primitive.pureSpecular + Trace(reflectedRay, bounce + 1, debugX);

                    } else {
                        //Add the pixelColor with shading of the lights in the environment
                        PixelColor = CalculateShading(primaryIntersection, primitive, debugX);

                        //Add an ambient light to the color of the object. This eliminates black pixels of the objects.
                        Vector3 ambient = new Vector3(primitive.color.X, primitive.color.Y, primitive.color.Z) * new Vector3(ambientLightingAmount);
                        
                        switch (primitive)
                        {
                            case Plane p:
                                if (p.Texture)
                                {
                                    //Texturing a checkboard pattern to a plane that has the Texture set to true
                                    ambient *= ((int)p.scalarU + (int)p.scalarV & 1) * new Vector3(1, 1, 1);
                                }
                                break;
                           /* case Sphere s:
                                if (s.Texture)
                                {
                                    ambient *= ((int)p.scalarU + (int)p.scalarV & 1) * new Vector3(1, 1, 1);
                                }
                                break;*/
                        }
                        
                        PixelColor += ambient;
                    }
                    //The pixelColor can't be higher than the color white as it will generate artifects when it goes over 255
                    return Vector3.ComponentMin(PixelColor, new Vector3(255, 255, 255));
                }
            }
            //Color of the sky
            return new Vector3(135, 206, 235);

        }
        (Intersection, Primitive) CalculateClosestIntersection(Ray ray)
        {
            Intersection? closestIntersection = null;
            Primitive? closestPrimitve = null;
            foreach (Primitive primitive in scene.primitives)
            {
                //Intersect the ray with every primitive in the scene
                Intersection intersection = new Intersection(ray, primitive);
                // als de distance kleiner of gelijk aan maxdistance dan is er een intersection
                // die primitive heeft een kleur. de kleur kan je gooien naar die pixel.
                // screen.pixels[position] = die kleur
                if (intersection.distance == 0) continue;

                
                if (closestIntersection is null || closestIntersection.distance > intersection.distance)
                {
                    //store the intersection in closestIntersection and overwrite it if the distance of the new intersection is less than the stored distance
                    closestIntersection = intersection;
                    closestPrimitve = primitive;
                }
                

            }
            
        return (closestIntersection, closestPrimitve);
        }

        

        Vector3 CalculateShading(Vector3 primaryIntersection, Primitive primitive, int debugX) {
            Vector3 PixelColor = new Vector3(0,0,0); //base color: black

            foreach (Light light in scene.lights)
            {
                //go through every light in the scene

                //Debug the shadow rays, depending on the shape. Planes are excluded as the ground generates too many rays.
                if (debugX % 10 == 0) {
                    switch (primitive)
                    {
                        case (Sphere s):
                            //if the middle of the sphere is equal to the height primaryIntersection
                            if (s.position.Y == primaryIntersection.Y)
                            {
                                screen.Line(tx(light.position.X), ty(light.position.Z), tx(primaryIntersection.X), ty(primaryIntersection.Z), 0x696969);
                            }
                            break;
                        case (Triangle t):
                            //if the vert0 point is equal to the height of the primaryIntersection
                            if (t.vert0.Y == primaryIntersection.Y)
                            {
                                screen.Line(tx(light.position.X), ty(light.position.Z), tx(primaryIntersection.X), ty(primaryIntersection.Z), 0x696969);
                            }
                            break;
                    }
                    
                }

                //if the shadow ray doesn't hit anything calculate the pixel color
                Vector3 LightDirection = light.position - primaryIntersection;
                Vector3 LightDirectionNormalized = Vector3.Normalize(LightDirection);
                /*switch (light)
                {
                    case SpotLight sl:
                       renderLight = (180 / Math.PI) * Vector3.CalculateAngle(sl.Direction, LightDirectionNormalized) <= sl.maxAngle;
                        break;
                }*/

                float LightDistance = Vector3.Distance(LightDirection, primaryIntersection);
                //shoot a shadow ray from the intersection to the light
                Ray shadowRay = new Ray(primaryIntersection, LightDirectionNormalized, 1000);


                bool shadowRayHit = false;
                //Check if the ray hits anything (with a margin of 0.0001 to eliminate light acne)
                foreach (Primitive primitiveObject in scene.primitives)
                {
                    Intersection shadowIntersection = new Intersection(shadowRay, primitiveObject);
                    if (shadowIntersection.distance > 0.0001)
                    {
                        shadowRayHit = true;
                    }
                }

                //if there is no shadow in this pixel
                if (!shadowRayHit)
                {
                    //calculate the Lradiance of the pixel compared to the current light
                    float Lradiance = light.intensity * (1 / (float)Math.Pow(Vector3.Distance(primaryIntersection, light.position), 2));
                    Vector3 CameraDirection = primaryIntersection - camera.position;
                    Vector3 Normal = new Vector3(0, 0, 0);
                    Vector3 ReflectedColor = new Vector3(0, 0, 0);
                    float glossiness = primitive.glossiness;

                    //Get the Reflected color and the normal of the current primitive type
                    switch (primitive)
                    {
                        case Sphere s:
                            {
                                Normal = primaryIntersection - s.position;
                                ReflectedColor = CalculateReflectedColor(light, s);

                                break;
                            }
                        case Plane p:
                            {
                                Normal = p.normal;
                                ReflectedColor = CalculateReflectedColor(light, p);
                                break;
                            }
                        case Triangle t:
                            {
                                    
                                Normal = t.normal;
                                ReflectedColor = CalculateReflectedColor(light, t);
                                break;
                            }
                    }
                    Vector3 ReflectedVector = LightDirection - 2 * (Vector3.Dot(LightDirection, Normal)) * Normal;

                    float diffuseAngle = Vector3.Dot(Normal, LightDirection);
                    float glossyAngle = Vector3.Dot(CameraDirection, ReflectedVector);

                    //Add the PixelColor which is calculated with the diffusement and glossiness added together
                    PixelColor += CalculatePixelColor(Lradiance, diffuseAngle, glossyAngle, ReflectedColor, new Vector3(0.1f, 0.1f, 0.1f), glossiness);
                }
            }
            return PixelColor;
        }

        Vector3 CalculateReflectedColor(Light light, Primitive p)
        {
            //The product of  the base color and the light color together
            return new Vector3(((light.color.X / 255) * (p.color.X / 255)) * 255, ((light.color.Y / 255) * (p.color.Y / 255)) * 255, ((light.color.Z / 255) * (p.color.Z / 255)) * 255);
        }

        Vector3 CalculatePixelColor(float Lradiance, float diffuseAngle, float glossyAngle, Vector3 ReflectedColor, Vector3 GlossyColor, float glossiness)
        {
            //The diffuseShading and the Specular shading added together
            //Math.Max(0) to eliminate angles greater than 90deg.
            Vector3 DiffuseShading = (Math.Max(0, diffuseAngle)) * ReflectedColor;
            Vector3 SpecularShading = (float)Math.Pow(Math.Max(0, glossyAngle), glossiness) * GlossyColor;

            return Lradiance * (DiffuseShading + SpecularShading);
        }   

        public void RenderDebug()
        {
            //screen.Print(camera.position.ToString(), 0, 0, 0xffffff);
            // plot the camera
            screen.Plot(tx(camera.position.X), ty(camera.position.Z), 0xffffff);

            // use a line to visualize the screen plane
            screen.Line(tx(camera.p0.X), ty(camera.p0.Z), tx(camera.p1.X), ty(camera.p1.Z), 0xffffff);

            for (int x = 0, n = 20; x < screen.width / 2; x++)
            {
                if (x % n != 0) continue;

                float a = x / ((float)screen.width / 2), b = (screen.height / 2) / (float)screen.height;
                var u = camera.p1 - camera.p0;
                var v = camera.p2 - camera.p0;
                var direction = camera.p0 + a * u + b * v - camera.position;

                var ray = new Ray(camera.position, Vector3.Normalize(direction), 10f);
                (Intersection? closestIntersection, Primitive primitive) = CalculateClosestIntersection(ray);
                float distance;
                int color;
                if (closestIntersection?.distance != null)
                {
                    distance = (float)closestIntersection.distance;
                    color = 0xffff66;
                } else
                {
                    distance = 10f;
                    color = 0xff0000;
                }

                var intersection = new Vector2(camera.position.X, camera.position.Z) + new Vector2(ray.D.X, ray.D.Z) * distance;

                screen.Line(tx(camera.position.X), ty(camera.position.Z), tx(intersection.X), ty(intersection.Y), color);
            }

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
                    case Pyramide t:
                        screen.Line(tx((int)t.vert0.X), ty((int)t.vert0.Z), tx((int)t.vert1.X), ty((int)t.vert0.Z), color(t.color));
                        screen.Line(tx((int)t.vert0.X), ty((int)t.vert0.Z), tx((int)t.vert2.X), ty((int)t.vert2.Z), color(t.color));
                        screen.Line(tx((int)t.vert1.X), ty((int)t.vert1.Z), tx((int)t.vert2.X), ty((int)t.vert2.Z), color(t.color));
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

        public float returnX(int x)
        {
            x += (int)left;
            x /= (int)Math.Min(screen.width, screen.height) / (int)scale;
            return (float)x;
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
