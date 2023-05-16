namespace INFOGR2023Template
{
    public class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;
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
            screen.Clear(0);
            raytracer.RenderDebug();
        }
    }
}