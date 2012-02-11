using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{

    public interface IFeedAble
    {
        double FeedSync { get; }
        double FeedASync { get; }
    }

    public interface IPlungeFeedAble
    {
        double PlungeFeedSync { get; }
        double PlungeFeedAsync { get; }
    }
}
