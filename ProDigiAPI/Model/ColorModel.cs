using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProDigiAPI.Model
{
    public class ColorModel
    {
        public bool KnownColor { get; set; } = true;
        public string Name { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}
