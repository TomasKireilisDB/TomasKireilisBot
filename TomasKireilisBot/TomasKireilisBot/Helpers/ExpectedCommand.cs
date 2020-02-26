using System;

namespace TomasKireilisBot.Dialogs
{
    public class ExpectedCommand
    {
        public ExpectedCommand(string openDialogId, string longName, string shortName = null)
        {
            OpenDialogId = openDialogId;
            LongName = longName;
            ShortName = shortName;
        }

        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string OpenDialogId { get; set; }

        public bool CheckIfCalledThisCommand(string textInput)
        {
            textInput = textInput?.Trim().ToLower();
            if (string.IsNullOrEmpty(textInput))
            {
                return false;
            }

            if (textInput.ToLower().StartsWith(LongName.ToLower()) || textInput.ToLower().StartsWith(ShortName.ToLower()))
            {
                return true;
            }

            return false;
        }
    }
}