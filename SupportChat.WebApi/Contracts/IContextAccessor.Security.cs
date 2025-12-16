//@CodeCopy
#if ACCOUNT_ON
namespace SupportChat.WebApi.Contracts
{
    partial interface IContextAccessor
    {
        string SessionToken { set; }
    }
}
#endif
