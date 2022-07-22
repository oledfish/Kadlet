namespace Kadlet
{
    public class KdlPrintOptions 
    {
        /// <value>Defines whether CR, LF, backspaces, tabs and form feed must be escaped.</value>
        public bool EscapeCommon = true;
        
        /// <value>Defines whether newline characters must be escaped.</value>
        public bool EscapeLinespace = true;
        
        /// <value>Defines whether non-ASCII characters (Unicode value > 127) must be escaped.</value>
        public bool EscapeNonAscii = false;
        
        /// <value>Defines whether non-printable ASCII characters (Unicode value < 32) must be escaped.</value>
        public bool EscapeNonPrintableAscii = true;
        
        /// <value>Defines whether Unicode characters U+202A to U+202E and U+2066 to U+2069 (both inclusive) must be escaped. </value>
        public bool EscapePotentiallyDangerousUnicode = true;
        
        /// <value>Defines the exponent character used when printing floating point numbers.</value>
        public char ExponentChar = 'E';
        
        /// <value>Defines the character used for indentation.</value>
        public char IndentChar = ' ';
        
        /// <value>Defines the amount of indentation to use.</value>
        public int IndentSize = 4;
        
        /// <value>Defines whether hexadecimal, octal and binary numbers should be printed in their original format.</value>
        public bool KeepRadix = false;
        
        /// <value>Defines the newline string.</value>
        public string Newline = "\n";
        
        /// <value>Defines whether arguments with a null value should be ignored.</value>
        public bool PrintNullArguments = true;
        
        /// <value>Defines whether properties with a null value should be ignored.</value>
        public bool PrintNullProperties = true;
        
        /// <value>Defines whether {} should be printed for nodes with empty children.</value>
        public bool PrintEmptyChildren = true;
        
        /// <value>Defines whether nodes are terminated with a semicolon.</value>
        public bool TerminateNodesWithSemicolon = false;

        /// <summary>
        /// A "pretty" bundle of print settings, using four spaces for indentation, sensible escapes, and printing empty children.
        /// </summary>
        public static KdlPrintOptions PrettyPrint = new KdlPrintOptions{};

        /// <summary>
        /// A configuration with no indentation and no empty children.
        /// </summary>
        public static KdlPrintOptions Raw = new KdlPrintOptions {
            IndentSize = 0,
            PrintEmptyChildren = false
        };

        /// <summary>
        /// A configuration used for internal unit tests, same as PrettyPrint but with CRLF newlines and no empty children.
        /// </summary>
        public static KdlPrintOptions Testing = new KdlPrintOptions {
            Newline = "\r\n",
            PrintEmptyChildren = false
        };
    }
}