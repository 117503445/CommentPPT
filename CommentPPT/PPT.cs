using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ppt = Microsoft.Office.Interop.PowerPoint;
namespace CommentPPT
{
    class PPT
    {
        public static ppt.Application TryGetApplication()
        {
            try
            {
                return Marshal.GetActiveObject("PowerPoint.Application") as ppt.Application;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
