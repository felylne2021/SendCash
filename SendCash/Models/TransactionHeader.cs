//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SendCash.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransactionHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TransactionHeader()
        {
            this.TransactionDetails = new HashSet<TransactionDetail>();
        }
    
        public long TransactionId { get; set; }
        public long SenderId { get; set; }
        public Nullable<System.DateTime> TransactionDt { get; set; }
    
        public virtual Account Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}
