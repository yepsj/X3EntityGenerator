namespace X3TableReader
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal class Program
    {
        private const string TextFileDirectory = "C:\\Users\\scott.jones\\Desktop\\X3Tables";
        private const string DestinationDirectory =
            "C:\\Data\\Projects\\SCAL\\CONNEXT\\Clarity.SageX3\\Clarity.SageX3.Entities\\";

        private static void Main()
        {
            var dinfo = new DirectoryInfo($"{TextFileDirectory}");
            var files = dinfo.GetFiles($"*.txt");
            foreach (var file in files)
            {
                var data = new Data { Rows = new List<Row>() };
                var className = file.Name.Replace(".txt", "");
                using (var reader = new StreamReader(file.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var fields = line.Split('\t');
                        var row = new Row
                        {
                            Column = fields[0].Trim(),
                            NormalTitle = fields[1].Trim(),
                            Dim = fields[2].Trim(),
                            Type = fields[3].Trim(),
                            Length = fields[4].Trim(),
                            Menu = fields[5].Trim(),
                            LinkExpression = fields[6].Trim(),
                            Cancellation = fields[7].Trim(),
                            Act = fields[8].Trim()
                        };
                        data.Rows.Add(row);
                    }
                }
                var sb = new StringBuilder();
                sb.AppendLine("namespace Clarity.SageX3.Entities");
                sb.AppendLine("{");
                sb.AppendLine("    using System;");
                sb.AppendLine($"    public abstract class {className}");
                sb.AppendLine("    {");
                foreach (var row in data.Rows)
                {
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine($"        /// {row.NormalTitle}");
                    if (!string.IsNullOrEmpty(row.Dim))
                    {
                        sb.AppendLine($"        /// NOTE: This variable has a DIM of {row.Dim}");
                        sb.AppendLine("        /// NOTE: and needs to be wrapped in TAB/LIN");
                    }
                    if (!string.IsNullOrEmpty(row.LinkExpression))
                    {
                        sb.AppendLine($"        /// Link Expression: {row.LinkExpression}");
                    }
                    sb.AppendLine("        /// </summary>");
                    sb.AppendLine($"        public {GetVariableType(row.Type)} {row.Column} {{ get; set; }}");
                }
                sb.AppendLine("    }");
                sb.AppendLine("}");
                File.WriteAllText($"{DestinationDirectory}{className}.cs", sb.ToString());
            }
        }

        public class Data
        {
            public List<Row> Rows { get; set; }
        }
        public class Row
        {
            public string Column { get; set; }
            public string NormalTitle { get; set; }
            public string Dim { get; set; }
            public string Type { get; set; }
            public string Length { get; set; }
            public string Menu { get; set; }
            public string LinkExpression { get; set; }
            public string Cancellation { get; set; }
            public string Act { get; set; }
        }

        public static string GetVariableType (string x3Type)
        {
            switch (x3Type)
            {
                case "C":
                case "TBO":
                case "SFI":
                case "ADV":
                case "COV":
                    return "short";
                case "D":
                case "ADATIM":
                    return "DateTime";
                case "AUUID":
                    return "Guid";
                case "DCB":
                case "MD5":
                case "COE":
                case "VOL":
                case "WEI":
                case "RAT":
                case "QTY":
                case "MD8":
                case "LTI":
                    return "decimal";
                case "L":
                case "ATX":
                    return "long";
                default:
                    return "string";
            }

        }
    }
}
