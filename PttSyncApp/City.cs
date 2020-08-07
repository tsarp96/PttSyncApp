using System;
using System.Collections.Generic;
using System.Text;

namespace PttSyncApp
{
    class City
    {
        public City()
        {
            this.Districts = new List<District> { };
        }
        public string Name { get; set; }

        public List<District> Districts { get; set; }
    }
}
