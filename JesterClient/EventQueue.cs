using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class EventQueue
    {
        protected List<Event> functionList = new List<Event>();

        public EventQueue()
        {
        }

        public void addFunction(Event method)
        {
            this.functionList.Add(method);
        }

        public Event queueMethod()
        {
            Event method = functionList.First();
            functionList.RemoveAt(0);
            return method;
        }

        public void addFunctionOnBegin(Event method)
        {
            functionList.Insert(0, method);
        }

        public bool hasElementsOnList()
        {
            return functionList.Count > 0;
        }
    }
}