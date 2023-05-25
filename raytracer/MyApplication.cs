using OpenTK.Windowing.GraphicsLibraryFramework;

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

            screen.Line(screen.width / 2, 0, screen.width / 2, screen.height, 0xffffff);
        }
    }
}