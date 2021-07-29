using System.IO;
using System.Linq;
using Tensorflow;

namespace BotFramework_QnAExample.Utils
{
    public static class ResourceFiles
    {
        public static void Add2ColumnLineToTextFile(string filePath, string name, string value, string separator)
        {
            var textLines = File.ReadAllLines(filePath).ToList();
            textLines.add($"{name}{separator}{value}"); 
            File.WriteAllLines(filePath, textLines);
        }

        public static string GetFileText(string filePath)
        {
            var text = File.ReadAllText(filePath);
            return text;
        }
    }
}