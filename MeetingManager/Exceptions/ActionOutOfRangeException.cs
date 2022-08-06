using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingManager.Exceptions
{
    internal class ActionOutOfRangeException : Exception
    {
        public ActionOutOfRangeException(string? message) : base(message)
        {
        }
    }
}