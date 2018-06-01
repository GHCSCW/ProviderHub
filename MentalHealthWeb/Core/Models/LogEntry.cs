using System.Threading.Tasks;
using System;
using System.Linq;

namespace BehavorialHealthWeb.Models
{
    public class LogEntry{

        public DateTime EntryDate { get; set;}
        public string Message { get; set;}
        public LogLevel Level { get; set;}
        public object[] ExtraInfor { get; set;}
    }
}