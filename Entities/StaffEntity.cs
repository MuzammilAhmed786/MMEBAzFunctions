using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEBAzFunctions.Entities
{
    class StaffEntity : TableEntity
    {
        public StaffEntity(string staffName, string active)
        {
            this.PartitionKey = active;
            this.RowKey = Guid.NewGuid().ToString();
            this.StaffName = staffName;
        }

        public StaffEntity() { }

        public string StaffName { get; set; }

        public string Active { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string ImageURL { get; set; }

        public string Designation { get; set; }
    }
    
}
