using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JelaCoreLib.Model.Interface
{
    /// <summary>
    /// Interface that forces certain fields into wanted manner so they can be assumed.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface IJelaModel<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Primary key. Forced to be ID.
        /// </summary>
        [Column("ID")]
        [Key]
        TKey ID { get; set; }
    }
}
