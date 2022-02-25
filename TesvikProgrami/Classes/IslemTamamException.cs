using System;

namespace TesvikProgrami
{
    public class IslemTamamException : Exception
    {
        public IslemTamamException(string message)
            : base(message)
        {
        }

    }
}
