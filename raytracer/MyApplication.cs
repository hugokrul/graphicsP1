using OpenTK.Graphics.OpenGL;

namespace INFOGR2023Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }

        // initialize
        public void Init()
        {
            Camera camera = new Camera();
            Console.WriteLine(camera.position);
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Line(220, 150, 420, 150, 0xffffff); 
            screen.Line(220, 150, 220, 250, 0xffffff); 
            screen.Line(420, 150, 420, 250, 0xffffff); 
            screen.Line(220, 250, 420, 250, 0xffffff);
        }
    }
}