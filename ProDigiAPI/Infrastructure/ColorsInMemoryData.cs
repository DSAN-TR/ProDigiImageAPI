using ProDigiAPI.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ProDigiAPI.Infrastructure
{
    public static class ColorsInMemoryData
    {


        public static List<ColorModel> ColorsData
        {
            get
            {
                return GetColorList();
            }
        }

        private static List<ColorModel> GetColorList()
        {

            var ColorList = new List<ColorModel>();

            ColorList.Add(new ColorModel() { Name = "Grey", R = 56, G = 70, B = 87 });
            ColorList.Add(new ColorModel() { Name = "Teal", R = 0, G = 98, B = 110 });
            ColorList.Add(new ColorModel() { Name = "Navy", R = 0, G = 0, B = 80 });
            ColorList.Add(new ColorModel() { Name = "Black", R = 0, G = 0, B = 0 });


            return ColorList;
        }

    }
}
