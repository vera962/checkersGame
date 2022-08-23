using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectClient
{
   public class Coordinate
    {
      public int row { get; set; }
      public int column { get; set; }


    public Coordinate(int r,int c)
        {
            row = r;
            column = c;
        }

        public override string ToString()
        {
            return $"{row},{column}";
        }

    }

}
