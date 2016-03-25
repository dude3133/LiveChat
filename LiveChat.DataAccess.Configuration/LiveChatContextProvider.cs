namespace LiveChat.DataAccess.Configuration
{
    public interface ILiveChatContextProvider
    {
        /// <summary>
        /// IMHO it is bad idea to define Context as property here, method would be better
        /// each time we return a new instance of context, but using properties 
        /// suggests it is still the same
        /// </summary>
        ILiveChatContext Context { get; }
    }
    class LiveChatContextProvider : ILiveChatContextProvider
        {

            public ILiveChatContext Context
            {
                get { return new LiveChatContext(); }
            }
        }
    }
