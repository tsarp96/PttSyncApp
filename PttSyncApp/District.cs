using System;
using System.Collections.Generic;
using System.Text;

namespace PttSyncApp
{
    class District
    {
        public District()
        {
            this.Neighborhoods = new List<Neighborhood> { };
        }
        public string Name { get; set; }
        public List<Neighborhood> Neighborhoods { get; set; }
    }
}
