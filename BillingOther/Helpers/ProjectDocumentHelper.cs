using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BillingOther.Helpers
{
    public static class ProjectDocumentHelper
    {
        public static string BooleanSelectValue(object value)
        {
            string text = Convert.ToString(value).Trim();
            if (text.Length == 0) return "Select";
            return text == "1" || text.Equals("true", StringComparison.OrdinalIgnoreCase) || text.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "1" : "0";
        }

        public static void SaveRemaining(HttpRequest request, string inputName, string physicalFolder, string firstFileName, string addedBy)
        {
            Directory.CreateDirectory(physicalFolder);
            HttpFileCollection files = request.Files;
            bool skippedFirst = false;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile file = files[i];
                if (file == null || file.ContentLength == 0 || files.AllKeys[i] != inputName) continue;
                string original = Path.GetFileName(file.FileName);
                // The page handler already saves the first selected file.
                if (!skippedFirst) { skippedFirst = true; WriteMeta(Path.Combine(physicalFolder, firstFileName), addedBy); continue; }
                string saved = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + original;
                string fullPath = Path.Combine(physicalFolder, saved);
                file.SaveAs(fullPath);
                WriteMeta(fullPath, addedBy);
            }
        }

        private static void WriteMeta(string filePath, string addedBy)
        {
            File.WriteAllText(filePath + ".uploadmeta", addedBy ?? string.Empty);
        }

        public static string Render(string ndaPath, string msaPath)
        {
            StringBuilder rows = new StringBuilder();
            string knownPath = !string.IsNullOrWhiteSpace(ndaPath) ? ndaPath : msaPath;
            if (!string.IsNullOrWhiteSpace(knownPath))
            {
                string agreementFolder = VirtualPathUtility.GetDirectory(knownPath).TrimEnd('/');
                string projectFolder = VirtualPathUtility.GetDirectory(agreementFolder).TrimEnd('/');
                AddRows(rows, projectFolder + "/NDA", "NDA");
                AddRows(rows, projectFolder + "/MSA", "MSA");
            }
            if (rows.Length == 0) return "";
            return "<div style='margin-top:16px'><h6>Uploaded agreements</h6><table class='table table-bordered table-sm'><thead><tr><th>Type</th><th>File</th><th>Added by</th><th>Added date</th><th>Download</th></tr></thead><tbody>" + rows + "</tbody></table></div>";
        }

        private static void AddRows(StringBuilder rows, string virtualPath, string type)
        {
            if (string.IsNullOrWhiteSpace(virtualPath) || HttpContext.Current == null) return;
            string folder = HttpContext.Current.Server.MapPath(virtualPath);
            if (!Directory.Exists(folder)) return;
            HashSet<string> displayedContent = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string file in Directory.GetFiles(folder))
            {
                if (file.EndsWith(".uploadmeta", StringComparison.OrdinalIgnoreCase)) continue;
                string contentHash;
                using (FileStream stream = File.OpenRead(file))
                using (SHA256 sha = SHA256.Create())
                    contentHash = Convert.ToBase64String(sha.ComputeHash(stream));
                if (!displayedContent.Add(contentHash)) continue;
                string name = Path.GetFileName(file), meta = file + ".uploadmeta";
                string addedBy = File.Exists(meta) ? File.ReadAllText(meta) : "Existing file";
                string url = VirtualPathUtility.ToAbsolute(virtualPath.TrimEnd('/') + "/" + name);
                rows.Append("<tr><td>").Append(type).Append("</td><td>").Append(HttpUtility.HtmlEncode(name)).Append("</td><td>").Append(HttpUtility.HtmlEncode(addedBy)).Append("</td><td>").Append(File.GetCreationTime(file).ToString("dd-MMM-yyyy HH:mm")).Append("</td><td><a href='").Append(HttpUtility.HtmlAttributeEncode(url)).Append("' download>Download</a></td></tr>");
            }
        }
    }
}
