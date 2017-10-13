using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class UserInput : GWRequest<UserInput>
    {
        public UserInput(UserInputLookFor lookFor = UserInputLookFor.File)
        {
            LookFor = lookFor;
        }
        public string UserText { get; set; }
        public string TextForUser { get; set; }
        public string DefaultPath { get; set; }
        public UserInputLookFor LookFor { get; set; } = UserInputLookFor.File;
    }
    public enum UserInputLookFor
    {
        File,
        Folder
    }
}
