//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Site.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Blok
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Blok()
        {
            this.Daire = new HashSet<Daire>();
        }
    
        public int BlokID { get; set; }
        public string BlokAdi { get; set; }
        public string YoneticiAdiSoyadi { get; set; }
        public string YoneticiTelefonu { get; set; }
        public Nullable<int> YoneticiDaireID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Daire> Daire { get; set; }
    }
}
