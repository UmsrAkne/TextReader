namespace TextReader.Models
{
    public class PlainText : IText
    {
        private string text = string.Empty;

        public PlainText(string text)
        {
            this.text = text;
        }

        public string Text => text;
    }
}
