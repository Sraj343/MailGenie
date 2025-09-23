using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailGenie.Domain.Response
{
    public class MailResult
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
