using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023Template
{
    public class Scene
    {
        public List<Primitive> primitives;
        public List<Light> lights;

        public Scene()
        {
            primitives = new List<Primitive>();
            lights = new List<Light>();
        }
    }
}
