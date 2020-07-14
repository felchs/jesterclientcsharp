using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JesterClient
{
    public interface ActionGameGraphicsInterface
    {
        void onSnapshot(BinaryReader message);
        void onAction(BinaryReader message);
        void onMove(BinaryReader message);
    }
}
