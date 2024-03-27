using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database._CD;

public sealed class CDModel
{
    public enum DbRecordEntryType : byte
    {
         Medical, Security, Employment
    }

    [Table("cd_record_entries"), Index(nameof(Id))]
    public sealed class RecordEntry
    {
        public int Id { get; set;  }

        public string Title { get; set; } = null!;

        public string Involved { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DbRecordEntryType Type { get; set; }

        public int ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;

        /// <summary>
        /// Creator of this entry.
        ///
        /// Null represents that this entry was created by the owner
        /// </summary>
        public Guid? ForeignCreator { get; set; }
    }
}
