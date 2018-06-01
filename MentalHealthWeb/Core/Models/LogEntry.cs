using System.Threading.Tasks;
using System;
using System.Linq;

namespace BehavorialHealthWeb.Models
{
    public class LogEntry{

        public DateTime EntryDate { get; set;}
        public string message { get; set;}
        public LogLevel level { get; set;}
        public object[] ExtraInfor { get; set;}
    }
}