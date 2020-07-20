using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Core
{
    public interface Handles<TCommand>
    {
        IEnumerable Handle(TCommand c);
    }
}
