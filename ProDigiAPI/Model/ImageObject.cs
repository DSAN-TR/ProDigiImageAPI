using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProDigiAPI.Model
{
    public class ImageObject
    {
        public int Id { get; set; }
        public string Key { get; set; } = "Not Found";

        public ColorModel Color { get; set; } = null;

        public bool IsColourDefined { get; set; } = false;
    }
}
