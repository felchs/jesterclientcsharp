using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class FncRet
    {
		public bool immediateCall;
		
		public FncRet(bool immediateCall = true) 
		{
			this.immediateCall = immediateCall;
		}
    }
}
