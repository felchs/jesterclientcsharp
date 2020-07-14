using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
   	public interface ObjectGraphicsInterface 
	{
		void setEnabled(int objectId, bool enabled);
		void setVisible(int objectId, bool visible);
		void setSelected(int objectId, bool selected);
		void setSelectable(int objectId, bool selectable);
		
		void setAllEnabled(bool enabled);
		void setAllVisible(bool visible);
		void setAllSelected(bool selection);
		void setAllSelectable(bool selected);
	}
}