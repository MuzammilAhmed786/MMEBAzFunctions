using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace MMEBAzFunctions.Entities
{
    class SalahTimeEntity : TableEntity
    {
        public SalahTimeEntity(string salahTime, string active)
        {
            this.PartitionKey = active;
            this.RowKey = Guid.NewGuid().ToString();
            this.Adhan = salahTime;
        }

        public SalahTimeEntity() { }

        public int Id { get; set; }

        public string Adhan { get; set; }

        public string IqamaTime { get; set; }

        public string Active { get; set; }

        public string CreatedDate { get; set; }
        
        public string JsonForm { get; set; }
        
    }
}
