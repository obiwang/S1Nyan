// -----------------------------------------------------------------------
// Copyright (c) David Kean. All rights reserved.
// -----------------------------------------------------------------------

namespace System.Net.Entities
{
    internal struct Token
    {
        public int StartIndex;
        public int EndIndex;
        public string Text;
        public TokenType Type;
    }
}
