//@CodeCopy

#if IDINT_ON
global using IdType = System.Int32;
#elif IDLONG_ON
    global using IdType = System.Int64;
#elif IDGUID_ON
    global using IdType = System.Guid;
#else
global using IdType = System.Int32;
#endif
global using Common = SupportChat.Common;
global using CommonModules = SupportChat.Common.Modules;
global using SupportChat.Common.Extensions;
global using CommonStaticLiterals = SupportChat.Common.StaticLiterals;
global using TemplatePath = SupportChat.Common.Modules.Template.TemplatePath;
