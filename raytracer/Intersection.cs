using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOGR2023Template
{
    public class Intersection
    {
        public int distance;
        public Primitive primitive;
        public int normal;

        public Intersection(int d, Primitive p)
        {
            distance = d;
            primitive = p;
        }
    }
}
