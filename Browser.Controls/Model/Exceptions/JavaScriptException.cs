using System;

namespace Browser.Controls.Model.Exceptions
{
    public class JavaScriptException : Exception
    {
        public JavaScriptException(string message, string script) : base(message)
        {
            Script = script;
        }

        public string Script { get; }
    }
}