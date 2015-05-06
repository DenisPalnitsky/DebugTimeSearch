// Guids.cs
// MUST match guids.h
using System;

namespace MyCompany.VariableExplorer
{
    static class GuidList
    {
        public const string guidVariableExplorerPkgString = "fa7a665f-3de8-4ebc-bd5b-91f365fe6dc0";
        public const string guidVariableExplorerCmdSetString = "215007cf-2e73-46a1-b14b-df751e014858";
        public const string guidToolWindowPersistanceString = "569ac44f-8998-443a-ba4d-40c5f2ad9077";
        public const string guidVariableExplorerContextMenu = "8E8E0366-25D2-4A4C-B821-2F06D856A21D";

        public static readonly Guid guidVariableExplorerCmdSet = new Guid(guidVariableExplorerCmdSetString);
    };
}