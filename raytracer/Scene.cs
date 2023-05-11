using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023Template
{
    internal class Scene
    {
        List<Primitive> primitives;
        List<Light> lights;

        public Scene(List<Primitive> p, List<Light> l)
        {
            primitives = p;
            lights = l;
        }
    }
}
