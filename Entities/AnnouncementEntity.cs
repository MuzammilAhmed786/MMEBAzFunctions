using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEBAzFunctions.Entities
{
    class AnnouncementEntity : TableEntity
    {
        public AnnouncementEntity(string announcement, string active)
        {
            this.PartitionKey = active;
            this.RowKey = Guid.NewGuid().ToString();
            this.Announcement = announcement;
        }

        public AnnouncementEntity() { }

        public string Announcement { get; set; }

        public string Active { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
