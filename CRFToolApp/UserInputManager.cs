using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRFToolApp
{
    public class UserInputManager : GWManager<UserInput>
    {
        protected override void OnRequest(UserInput request)
        {
            if (request.LookFor == UserInputLookFor.Folder)
            {
                using (var fbd = new FolderBrowserDialog()
                {
                    Description = request.TextForUser
                })
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        request.UserText = fbd.SelectedPath;
                        //string[] files = Directory.GetFiles(fbd.SelectedPath);

                        //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                    }
                }
            }
            else if (request.LookFor == UserInputLookFor.File)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Title = request.TextForUser;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    request.UserText = openFileDialog1.FileName;
                }
            }
        }
    }
}
