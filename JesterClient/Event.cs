using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class Event
    {
        public int id;

        public List<Object> objects = new List<Object>();

        public Event(int id, List<Object> objects)
        {
            this.id = id;
            this.objects = objects;
        }
    }
}
