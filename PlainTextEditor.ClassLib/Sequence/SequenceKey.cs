using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainTextEditor.ClassLib.Sequence;

public record SequenceKey(Guid Guid)
{
    public static SequenceKey NewSequenceKey()
    {
        return new SequenceKey(Guid.NewGuid());
    }
}
