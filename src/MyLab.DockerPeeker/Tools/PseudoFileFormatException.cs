using System;

namespace MyLab.DockerPeeker.Tools
{
    class PseudoFileFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PseudoFileFormatException"/>
        /// </summary>
        public PseudoFileFormatException(string msg): base(msg)
        {
            
        }
    }
}