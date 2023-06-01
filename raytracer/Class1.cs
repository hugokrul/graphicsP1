/*using System.Diagnostics;
using System.Diagnostics.Tracing;
using OpenTK.Mathematics;
using static System.Net.Mime.MediaTypeNames;

namespace INFOGR2023Template.Elements;

public class Raytracer
{
    // glossy op een mirror material
    public Scene Scene;
    public Camera Camera;
    public Surface Screen;
    private const float MaxRayDistance = 15f;
    public float Scale = 10f, Left = -5f, Top = -1f;
    public const float DebugY = 0;
    /// <summary>
    /// This is implemented for debug purposes :)
    /// </summary>
    public object[] DebugObjects;

    public Raytracer(Application app)
    {
        Screen = app.Screen;
        DebugObjects = new object[Screen.width * Screen.height];
        Scene = new Scene();
        Scene.LightSources.Add(new Light(new(-2f, 2.5f, 4f), new(1, 1, 1)));
        // scene.lightSources.Add(new Light(new(-2f,2.5f,4f), new(1,1,1)));
        Scene.Primitives.Add(new Plane(new(0, 1, 0), new(0, -1, 0)));
        //scene.primitives.Add(new Plane(new((float)Math.Cos(30 * Math.PI / 180), (float)Math.Sin(30 * Math.PI / 180), 0), 4, new(1,0,0)));
        //scene.primitives.Add(new Plane(new((float)Math.Cos(45 * Math.PI / 180), 0, (float)Math.Sin(45 * Math.PI / 180)),3, new(1, 0, 0)));
        Scene.Primitives.Add(new Sphere(new(-2.5f, 0, 5), 1, new(0, 1, 0), new Vector3(1, 1, 1), new Vector3(0.5f, 0.5f, 0f)));
        Scene.Primitives.Add(new Sphere(new(0, 0, 5), 1, new(1, 0, 0), new Vector3(1, 1, 1), new Vector3(0.7f, 1, 0)));
        // scene.primitives.Add(new Sphere(new(2.5f, 0, 5), 1, new(0, 0, 1)));
        // mirror ball
        Scene.Primitives.Add(new Sphere(new(3, 2, 8), 1, new(1, 1, 1), new(1, 1, 1), new(0.05f, 1, 0.8f)));
        Scene.Primitives.Add(new Sphere(new(2.5f, 0, 5f), 1, new(1, 1, 0), new(1, 1, 1), new(0.05f, 1, 1)));
        Camera = new Camera(new(0, 0, 0), new(0, 0, 1), new(0, 1, 0), 90);
        Vector3[] points = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            char[] s = Convert.ToString(i, 2).ToCharArray();
            while (s.Length < 3) s = s.Prepend('0').ToArray();

            points[i] = new Vector3(int.Parse(s[0].ToString()), int.Parse(s[1].ToString()), int.Parse(s[2].ToString()));
        }

        Vector3[] f =
        {
            new (0,0,4),
            new (0,0,6),
            new (2,0,5),
            new (1,2,5)
        };
        Triangle[] pyramid =
        {
            new (new (f[0],f[1],f[2])),
            new (new (f[0],f[1],f[3])),
            new (new (f[0],f[2],f[3])),
            new (new (f[1],f[2],f[3]))
        };
        Scene.Primitives.AddRange(pyramid);
        //Scene.Primitives.Add(new Triangle(new(new (0,0,4), new (2,0,4), new (1,2,4))));
    }

    /// <summary>
    /// Uses the camera to loop over the pixels of the screen plane and to
    /// generate a ray for each pixel, which is then used to find the nearest intersection.
    /// The result is then visualized by plotting a pixel. <br/> <br/>
    /// For the middle row of pixels (typically line 256 for a 512x512
    /// window), it generates debug output by visualizing every Nth ray (where N is e.g. 10).
    /// </summary>
    public void Render()
    {
        RenderDebug();
        RenderTracer();
        Screen.Line(Screen.height, 0, Screen.height + 1, Screen.height, Color.White);
    }

    private void RenderDebug()
    {


        // Plot the position of the camera as a white dot
        Screen.Plot(
            Tx(Camera.Position.X),
            Ty(Camera.Position.Z),
            Color.White
        );
        // Draw the plane which the camera uses
        Screen.Line(
            Tx(Camera.P0.X),
            Ty(Camera.P0.Z),
            Tx(Camera.P1.X),
            Ty(Camera.P1.Z),
            Color.White
        );

        foreach (var light in Scene.LightSources)
        {
            int x = Tx(light.Location.X), y = Ty(light.Location.Z);
            for (int i = x - 1; i < x + 1; i++)
            {
                for (int j = y - 1; j < y + 1; j++)
                {
                    Screen.Plot(i, j, Color.Cyan);
                }
            }
        }

        // 1/n is the amount of rays displayed
        for (int x = 0, n = 20; x < Screen.width / 2; x++)
        {
            if (x % n != 0) continue;

            var ray = CreateRay(x, Screen.height / 2);
            var closestIntersection = CalculateClosestIntersection(ray);
            float distance;
            int color;
            if (closestIntersection?.Distance != null)
            {
                distance = (float)closestIntersection.Distance;
                color = Color.Yellow;
            }
            else
            {
                distance = ray.Distance;
                color = Color.FromRGB(255, 120, 0);
            }
            var intersection = new Vector2(Camera.Position.X, Camera.Position.Z)
                                    + new Vector2(ray.Direction.X, ray.Direction.Z) * distance;

            Screen.Line(
                Tx(Camera.Position.X),
                Ty(Camera.Position.Z),
                Tx(intersection.X),
                Ty(intersection.Y),
                color
            );
        }

        foreach (var primitive in Scene.Primitives) switch (primitive)
            {
                case Sphere s:
                    if (Math.Abs(s.Position.Y - DebugY) > s.Radius) break;
                    float radius = Math.Abs(s.Position.Y - DebugY) > 0.1f
                        ? (float)Math.Sqrt(s.Radius * s.Radius - (s.Position.Y - DebugY) * (s.Position.Y - DebugY))
                        : s.Radius;
                    int radiusInt = (int)(radius * (Screen.height / Scale));
                    for (int i = 0; i < 100; i++)
                    {
                        int x = Tx(s.Position.X) + (int)(radiusInt * Math.Cos(Math.PI * i / 50));
                        int y = Ty(s.Position.Z) + (int)(radiusInt * Math.Sin(Math.PI * i / 50));
                        int x1 = Tx(s.Position.X) + (int)(radiusInt * Math.Cos(Math.PI * (i - 1) / 50));
                        int y1 = Ty(s.Position.Z) + (int)(radiusInt * Math.Sin(Math.PI * (i - 1) / 50));
                        Screen.Line(x, y, x1, y1, Color.FromColorVector(s.DiffuseRgb));
                    }
                    break;
                case Plane p:

                    Vector2[] planeLine = CalculatePlaneLine(p.Normal, p.PointOnPlane);
                    Vector2 originVector = planeLine[1];
                    Vector2 directionVector = planeLine[0];

                    //if (p.normal == new Vector3(0, 1, 0)) break;
                    // (x - pointOnPlane.X) * p.normal.X - pointOnPlane.Y * p.normal.Y + (z - pointOnPlane.Z) * p.normal.Z = 0
                    // (x - pointOnPlane.X) * p.normal.X - pointOnPlane.Y * p.normal.Y = -(z - pointOnPlane.Z) * p.normal.Z
                    // (z - pointOnPlane.Z) * p.normal.Z = -(x - pointOnPlane.X) * p.normal.X + pointOnPlane.Y * p.normal.Y
                    // z - pointOnPlane.Z = -(x - pointOnPlane.X) * p.normal.X + pointOnPlane.Y * p.normal.Y / p.normal.Z
                    // z = -(x - pointOnPlane.X) * p.normal.X + pointOnPlane.Y * p.normal.Y / p.normal.Z + pointOnPlane.Z
                    Vector2 p1;
                    Vector2 p2;
                    if (directionVector.X == 0) p1 = new Vector2((Top + Scale - originVector.Y / directionVector.Y) * directionVector.X + originVector.X, Top + Scale);
                    else p1 = new Vector2(Left, (Left - originVector.X / directionVector.X) * directionVector.Y + originVector.Y);
                    // if true then no intersection with the axis, therefore there is no point like that,
                    // this will mean that you have to intersect with y = t (x = l) and y = t + scale (x = l + scale)
                    // instead of y = t and x = l
                    if (directionVector.Y == 0) p2 = new Vector2(Left + Scale, (Left + Scale - originVector.X / directionVector.X) * directionVector.Y + originVector.Y);
                    else p2 = new Vector2((Top - originVector.Y / directionVector.Y) * directionVector.X + originVector.X, Top);

                    // screen.Line(
                    //     Tx(p1.X),
                    //     Ty(p1.Y), 
                    //     Tx(p2.X), 
                    //     Ty(p2.Y),
                    //     Color.Magenta
                    // );
                    break;
            }
    }

    private static Vector2[] CalculatePlaneLine(Vector3 normal, Vector3 pointOnPlane)
    {
        // n.X*x + n.Y*y + n.Z*z - (n.X*x0 + n.Y*y0 + n.Z*z0) = 0 = Ax + By + Cz + D
        float a = normal.X;
        float b = normal.Y;
        float c = normal.Z;
        float d = -(a * pointOnPlane.X + b * pointOnPlane.Y + c * pointOnPlane.Z);

        var directionVector = Vector2.Normalize(new Vector2(a / b, c / b));
        var originVector = new Vector2(d / b, 0);

        return new[] { directionVector, originVector };
    }
    private float? CalculateLine(float x, Vector3 pointOnPlane, Plane p)
    {
        try
        {
            float res = (-(x - pointOnPlane.X) * p.Normal.X + pointOnPlane.Y * p.Normal.Y) / p.Normal.Z + pointOnPlane.Z;
            if (res is not float.PositiveInfinity and not float.NegativeInfinity) return res;
            return null;
        }
        catch (DivideByZeroException)
        {
            return null;
        }
    }
    private void RenderTracer()
    {
        // var tasks = new Task[Screen.width / 2];
        // for (int x = 0; x < Screen.width / 2; x++)
        // {
        //     tasks[x] = new(xObj =>
        //     {
        //         if (xObj is int xint)
        //         {
        //             for (int y = 0; y < Screen.height; y++)
        //             {
        //                 Screen.pixels[y * Screen.width + xint + Screen.height] =
        //                     Color.FromColorVector(Trace(CreateRay(xint, y), xint, y));
        //             }
        //         }
        //     }, x);
        //     tasks[x].Start();
        // }
        //
        // Task.WaitAll(tasks);

        for (int x = 0; x < Screen.width / 2; x++)
        {
            for (int y = 0; y < Screen.height; y++)
            {
                Screen.pixels[y * Screen.width + x + Screen.height] =
                    Color.FromColorVector(Trace(CreateRay(x, y), x, y));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="x">Debug only</param>
    /// <param name="y">Debug only</param>
    /// <returns></returns>
    private Vector3 Trace(Ray ray, int x, int y)
    {
        var closestIntersection = CalculateClosestIntersection(ray);

        if (closestIntersection?.Distance != null)
        {
            if (y == Screen.height / 2 && x % 20 == 0)
            {
                var intersectionPoint = ray.Origin + (float)closestIntersection.Distance * ray.Direction;
                Screen.Line(Tx(ray.Origin.X), Ty(ray.Origin.Z), Tx(intersectionPoint.X), Ty(intersectionPoint.Z), Color.Magenta);
            }
            return ColorWithLightMaterial(closestIntersection, ray, x, y);
            // return ColorWithLight(closestIntersection, ray,x,y);
            // return ColorWithDistance(closestIntersection);
            // return ColorWithIntersection(closestIntersection);
        }

        return new Vector3(102 / 255f, 178 / 255f, 1); Color.FromRGB(102, 178, 255);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="closestIntersection"></param>
    /// <param name="ray"></param>
    /// <param name="x">Debug only</param>
    /// <param name="y">Debug only</param>
    /// <returns></returns>
    private Vector3 ColorWithLightMaterial(Intersection closestIntersection, Ray ray, int x, int y)
    {
        var primitive = closestIntersection.NearestPrimitive;
        var diffuseColor = primitive.Texture switch
        {
            Textures.Checkerboard => (((int)primitive.ScalarU + (int)primitive.ScalarV) & 1) * new Vector3(1, 1, 1),
            _ => primitive.DiffuseRgb
        };
        float intersectionDistance = closestIntersection.Distance ?? 0f;
        var intersection = Camera.Position + ray.Direction * intersectionDistance;
        var mirrorColor = new Vector3();
        if (Math.Abs(primitive.MaterialType.Z) > 0.0001f && ray.Bounces < 10)
        {
            var reflected =
                ray.Direction - 2 * Vector3.Dot(closestIntersection.Normal, ray.Direction)
                                  * closestIntersection.Normal;
            ray = new Ray(intersection, reflected, MaxRayDistance);
            ray.Bounces++;
            mirrorColor = diffuseColor * Trace(ray, x, y);
        }

        var ambient = diffuseColor * 0.1f;
        Refraction(ray.Direction, closestIntersection.Normal, primitive);
        foreach (var light in Scene.LightSources)
        {
            var lightVector = light.Location - intersection;
            var lightRay = new Ray(light.Location, Vector3.Normalize(-lightVector), lightVector.Length);
            var lightIntersection = CalculateClosestIntersection(lightRay);
            const float epsilon = 0.0001f;

            if (lightIntersection != null && !(lightIntersection.Distance > lightVector.Length - epsilon ||
                                               lightIntersection.Distance < epsilon)) continue;

            if (y == Screen.height / 2 && x % 10 == 0)
                Screen.Line(Tx(light.Location.X), Ty(light.Location.Z), Tx(intersection.X), Ty(intersection.Z),
                    Color.Red);


            var lightReflection =
                -lightVector.Normalized() - 2 * Vector3.Dot(closestIntersection.Normal, -lightVector.Normalized())
                                              * closestIntersection.Normal;
            var glossy =
                (float)Math.Pow(Math.Max(0, Vector3.Dot(-ray.Direction, lightReflection.Normalized())), 50f)
                * primitive.GlossyRgb;
            var diffuse =
                Math.Max(0, Vector3.Dot(closestIntersection.Normal, Vector3.Normalize(lightVector)))
                * diffuseColor;


            const float lightIntensityMod = 20;
            var material = primitive.MaterialType;
            var color = Math.Abs(material.Z) > 0.0001f
                ? light.IntensityRgb * (1 / lightVector.LengthSquared) * lightIntensityMod
                  * (material.X * diffuse + material.Y * glossy + material.Z * mirrorColor)
                : light.IntensityRgb * (1 / lightVector.LengthSquared) * lightIntensityMod
                  * (material.X * diffuse + material.Y * glossy);
            DebugObjects[y * Screen.width + x + Screen.height] = diffuse;
            ambient += color;
        }

        return ambient;
    }

    private void Refraction(Vector3 direction, Vector3 normal, Primitive primitive)
    {
        // this should be gotten from the primitive, the primitive is one of the floats, and the air is the other, idk what 
        const float nt = 1.5f, n = 1;
        // refracted ray
        var reflected = n / nt * direction - Vector3.Dot(normal, direction) * normal;
        var refracted = reflected - normal *
            (float)Math.Sqrt(1 - (n * n) / (nt * nt) * (1 - Math.Pow(Vector3.Dot(direction, normal), 2)));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="closestIntersection"></param>
    /// <param name="ray"></param>
    /// <param name="x">Debug only</param>
    /// <param name="y">Debug only</param>
    /// <returns></returns>
    private int ColorWithLight(Intersection closestIntersection, Ray ray, int x, int y)
    {
        float intersectionDistance = closestIntersection.Distance ?? 0f;
        var intersection = Camera.Position + ray.Direction * intersectionDistance;
        foreach (var light in Scene.LightSources)
        {
            var lightVector = intersection - light.Location;
            var lightRay = new Ray(light.Location, Vector3.Normalize(lightVector), lightVector.Length);
            var lightIntersection = CalculateClosestIntersection(lightRay);
            const float epsilon = 0.0001f;

            if (lightIntersection != null && !(lightIntersection.Distance > lightVector.Length - epsilon))
                return 0;

            var c = closestIntersection.NearestPrimitive.DiffuseRgb;
            DebugObjects[y * Screen.width + x + Screen.height] = lightIntersection?.Distance;
            //float decrement = maxRayDistance - intersectionDistance / maxRayDistance;
            return Color.FromColorVector(c);
        }

        return 0;
    }
    private static int ColorWithDistance(Intersection closestIntersection)
    {
        float intersectionDistance = closestIntersection.Distance ?? 0f;
        var c = closestIntersection.NearestPrimitive.DiffuseRgb;
        float decrement = MaxRayDistance - intersectionDistance / MaxRayDistance;
        return Color.DecreaseVibranceInt(c, decrement);
    }
    private static int ColorWithIntersection(Intersection closestIntersection)
    {
        var c = closestIntersection.NearestPrimitive.DiffuseRgb;
        return Color.FromColorVector(c);
    }
    private Ray CreateRay(int x, int y)
    {
        float a = x / ((float)Screen.width / 2), b = y / (float)Screen.height;
        var u = Camera.P1 - Camera.P0;
        var v = Camera.P2 - Camera.P0;
        var direction = Camera.P0 + a * u + b * v - Camera.Position;

        var ray = new Ray(Camera.Position, Vector3.Normalize(direction), MaxRayDistance)
        {
            Bounces = 0
        };
        return ray;
    }
    private Intersection? CalculateClosestIntersection(Ray ray)
    {
        Intersection? closestIntersection = null;
        foreach (var p in Scene.Primitives)
        {
            var intersection = new Intersection(p, ray);
            if (intersection.Distance is null) continue;

            if (closestIntersection is null || closestIntersection.Distance > intersection.Distance)
                closestIntersection = intersection;
        }

        return closestIntersection;
    }

    // if geometric shape, in scale 1x1 -> scale should be 1.5
    // scale = length of square, left = left-most coordinate of the square
    // x = float (x >= left && x <= left + scale)
    private int Tx(float x)
    {
        int min = Math.Min(Screen.width, Screen.height);
        return (int)((x - Left) * (min / Scale));
    }
    private int Ty(float y)
    {
        int min = Math.Min(Screen.width, Screen.height);
        float mid = Top + Scale / 2;
        float invertedY = y - (y - mid) * 2;
        return (int)((invertedY - Top) * (min / Scale));
    }
    Verstuurd vanaf mijn iPad


 using OpenTK.Mathematics;

namespace INFOGR2023Template.Elements;

public class Intersection
{
    /// <summary>
    /// The intersection distance. (we hebben nu distance van origin naar intersection point
    /// </summary>
    public float? Distance;
    /// <summary>
    /// The nearest Primitive to the intersection.
    /// </summary>
    public Primitive NearestPrimitive;
    /// <summary>
    /// The normal vector at the intersection point.
    /// </summary>
    public Vector3 Normal;

    public Intersection(Primitive primitive, Ray ray)
    {
        NearestPrimitive = primitive;
        switch (primitive)
        {
            case Sphere s:
                SphereIntersection(s, ray);
                break;
            case Plane p:
                PlaneIntersection(p, ray);
                break;
            case Triangle t:
                TriangleIntersection(t, ray);
                break;
        }
    }

    private void SphereIntersection(Sphere sphere, Ray ray)
    {
        // vector tussen origin en middelpunt
        Vector3 distanceVector = sphere.Position - ray.Origin;
        float dot = Vector3.Dot(distanceVector, ray.Direction);
        // vector van het middelpunt van de cirkel naar het dichtbijzijnde punt van de ray (90 graden)
        Vector3 q = distanceVector - dot * ray.Direction;
        // de afstand van het middelpunt van de cirkel naar het dichtbijzijnde punt van de ray in het kwadraat
        float p2 = q.LengthSquared;

        // als de ray buiten de cirkel valt, wordt de functie gestopt.
        if (p2 > sphere.Radius * sphere.Radius) return;
        // "dot" wordt gelijk aan de afstand tussen ray.origin en het punt waar de ray de sphere intersect
        dot -= (float)Math.Sqrt(sphere.Radius * sphere.Radius - p2);

        // wat failsafes voor de functie
        if (dot < ray.Distance && dot > 0)
        {
            ray.Distance = dot;
            Distance = dot;
            Vector3 intersectionPoint = ray.Origin + ray.Direction * (float)Distance;
            Normal = Vector3.Normalize(intersectionPoint - sphere.Position);
        }

    }

    private void PlaneIntersection(Plane plane, Ray ray)
    {
        Normal = plane.Normal;

        if (Vector3.Dot(plane.PointOnPlane - ray.Origin, plane.Normal) == 0)
        {
            Distance = 0;
            return;
        }
        if (Vector3.Dot(ray.Direction, plane.Normal) == 0) return;

        float d = Vector3.Dot(plane.PointOnPlane - ray.Origin, plane.Normal) / Vector3.Dot(ray.Direction, plane.Normal);
        // it will give the distance if the ray interacts with the plane on d
        // distance = Math.Sign(d) == -1 ? Math.Abs(d) : ray.distance;

        // float dPos = Math.Sign(d) == -1 ? Math.Abs(d) : ray.distance;
        if (d < ray.Distance && d > 0)
        {
            Distance = d;
            ray.Distance = d;
            if (plane.Texture != Textures.None)
            {
                var op = plane.PointOnPlane - ray.Origin + ray.Direction * d;
                plane.ScalarU = Vector3.Dot(op, plane.TexturingU) / plane.TexturingU.LengthSquared;
                plane.ScalarV = Vector3.Dot(op, plane.TexturingV) / plane.TexturingV.LengthSquared;
            }
        }
    }

    private void TriangleIntersection(Triangle triangle, Ray ray)
    {
        var plane =
            new Plane(Vector3.Cross(triangle.BPosition - triangle.APosition, triangle.CPosition - triangle.APosition).Normalized(),
                triangle.APosition);
        var intersection = new Intersection(plane, ray);
        if (intersection.Distance == null) return;

        var planeNormal = plane.Normal;
        var intersectionPoint = ray.Origin + ray.Direction * (float)intersection.Distance;
        if (Vector3.Dot(Vector3.Cross(triangle.BPosition - triangle.APosition,
                intersectionPoint - triangle.APosition), planeNormal) < 0 ||
            Vector3.Dot(Vector3.Cross(triangle.CPosition - triangle.BPosition,
                intersectionPoint - triangle.BPosition), planeNormal) < 0 ||
            Vector3.Dot(Vector3.Cross(triangle.APosition - triangle.CPosition,
                intersectionPoint - triangle.CPosition), planeNormal) < 0) return; // then intersectionPoint is outside of triangle

        Distance = intersection.Distance;
        float Area(Vector3 x, Vector3 y, Vector3 z)
        {
            return Vector3.Cross(z - y, x - y).Length / 2;
        }
        float alpha = Area(intersectionPoint, triangle.BPosition, triangle.CPosition) / Area(triangle.APosition, triangle.BPosition, triangle.CPosition);
        float beta = Area(intersectionPoint, triangle.CPosition, triangle.APosition) / Area(triangle.APosition, triangle.BPosition, triangle.CPosition);
        float gamma = 1 - alpha - beta;

        Normal = alpha * triangle.ANormal + beta * triangle.BPosition + gamma * triangle.CPosition;
        triangle.ScalarU = alpha * triangle.Uva.X + beta * triangle.Uvb.X + gamma * triangle.Uvc.X;
        triangle.ScalarV = alpha * triangle.Uva.Y + beta * triangle.Uvb.Y + gamma * triangle.Uvc.Y;
    }
}

*/