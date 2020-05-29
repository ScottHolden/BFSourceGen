using System;
using System.Xml.Serialization;

namespace BFSourceGen
{
    public class BFTranspilerOptions
    {
        public int MemSize { get; set; } = 1024;
        public string MemType { get; set; } = "int";
        public string EoFValue { get; set; } = "0";
        public bool Optimize { get; set; } = false;
        public bool Class { get; set; } = false;
        public string ClassName { get; set; } = "BFProgram";
        public string Namespace { get; set; } = "BF";

        public void ValidateWithExceptions()
        {
            if (MemSize <= 0)
            {
                throw new Exception("Invalid memSize, must be greater than 0!");
            }

            if (MemType != "byte" && MemType != "int" && MemType != "long")
            {
                throw new Exception($"Invalid memType '{MemType}'! Valid types are byte, int, long");
            }

            if (!EoFValue.Equals("same", StringComparison.OrdinalIgnoreCase) &&
                !(MemType == "byte" && byte.TryParse(EoFValue, out byte _)) &&
                !(MemType == "int" && int.TryParse(EoFValue, out int _)) &&
                !(MemType == "long" && long.TryParse(EoFValue, out long _)))
            {
                throw new Exception($"Invalid eofValue '{EoFValue}'! Does it match the memType '{MemType}'?");
            }

            if (CodeIdentifier.MakeValid(ClassName) != ClassName)
            {
                throw new Exception($"Invalid class name '{ClassName}'!");
            }
        }
    }
}