namespace Nano11Toolkit.Models
{
    public class TogglableEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string EnableCommand { get; set; }
        public string DisableCommand { get; set; }
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
        public string EnabledOutput { get; set; }
        public bool Enabled { get; set; }
    }
    public class ButtonEntry
    {
        public string Name { get; set; }
        public string Command { get; set; }
    }
}