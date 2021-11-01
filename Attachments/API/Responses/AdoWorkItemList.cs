using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attachments.API.Responses
{
    class AdoWorkItemList
    {
        public string QueryType { get; set; }

        public string QueryResultType { get; set; }

        public string AsOf { get; set; }

        public List<Column> Columns { get; set; }

        public List<WorkItem> WorkItems { get; set; }

    }

    public class Column
    {
        public string ReferenceName { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class WorkItem
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
