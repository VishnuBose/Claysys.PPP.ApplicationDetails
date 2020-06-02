using Claysys.PPP.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
namespace Claysys.PPP.ApplicationDetails.Export
{
    public class ExportManagement
    {

        private static string _filePath = ConfigurationManager.AppSettings["FilePath"];


        static async Task WriteTextAsync(string filePath, byte[] docuDoc)
        {
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    DirectorySecurity accessControl = directoryInfo.GetAccessControl();
                    accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    directoryInfo.SetAccessControl(accessControl);
                }
                using (FileStream sourceStream = new FileStream(filePath,
                        FileMode.Append, FileAccess.Write, FileShare.None,
                        bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(docuDoc, 0, docuDoc.Length);
                };
            }
            catch (Exception ex)
            {

                if (Utility.Utility.IsEventLogged)
                {
                    Utility.Utility.LogAction("exception : " + ex);
                }

                throw ex;
            }
        }


        public static Task WriteToDisk(string loanAppNo, string sbaLoanNo, byte[] docuDoc)
        {
            try
            {
                var fileName = loanAppNo + "_" + sbaLoanNo + ".pdf";
                var filePath = Path.Combine(_filePath, fileName);
                return WriteTextAsync(filePath, docuDoc);
            }
            catch (Exception ex)
            {
                if (Utility.Utility.IsEventLogged)
                {
                    Utility.Utility.LogAction("exception : " + ex);
                }
                throw ex;
            }
        }
    }
}
