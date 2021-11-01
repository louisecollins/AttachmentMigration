using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attachments.API.Responses
{
    class AdoWorkItemRevisions
    {
        public int Count { get; set; }
        public List<Value> Values { get; set; }
    }

    public class Value
    {
        public int Id { get; set; }
        public string Rev { get; set; }

    }
}
