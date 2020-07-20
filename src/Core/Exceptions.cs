using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class UnapplicableEvent : Exception { }
    
    public class ConcurrencyConflictOccurred : Exception { }

    public class NoHandlerRegistered : Exception { }

    public class CommandHandlerAlreadyRegistered : Exception { }
}