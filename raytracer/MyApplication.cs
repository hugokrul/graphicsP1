using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;

namespace INFOGR2023Template
{
    public class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;
        public KeyboardState keyboard;
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }

        // initialize
        public void Init()
        {
            raytracer = new Raytracer(this);
        }
        // tick: renders one frame
        public void Tick()
        {
            raytracer.keyboard = keyboard;
            screen.Clear(0);
            raytracer.RenderDebug();
            raytracer.Render();

            raytracer.camera.updatePosition();
            //Console.WriteLine($"{raytracer.camera.direction}, {raytracer.camera.upDirection}");
            screen.Line(screen.width / 2, 0, screen.width / 2, screen.height, 0xffffff);
        }
    }
}