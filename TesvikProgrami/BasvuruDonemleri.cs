//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TesvikProgrami
{
    using System;
    using System.Collections.Generic;
    
    public partial class BasvuruDonemleri
    {
        public long ID { get; set; }
        public long IsyeriID { get; set; }
        public string BasvuruDonem { get; set; }
        public string Aylar { get; set; }
    
        public virtual Isyerleri Isyerleri { get; set; }
    }
}
