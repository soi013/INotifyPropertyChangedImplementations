using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace FodyPerson
{
    #region Fody版
    [AddINotifyPropertyChangedInterface]
    public class PersonFD
    {
        public string Name { get; set; } = "Hejlsberg";

        public string FullName => $"Anders {Name}";
    }

    //ILからC#に再変換したもの
    //public class PersonFD_ILSpy : INotifyPropertyChanged
    //{
    //    [field: NonSerialized]
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public virtual void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
    //        if (propertyChanged != null)
    //        {
    //            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    //public string Name
    //{
    //    [CompilerGenerated]
    //    get { return this.< Name > k__BackingField; }
    //    [CompilerGenerated]
    //    set
    //    {
    //        if (string.Equals(this.< Name > k__BackingField, value, StringComparison.Ordinal))
    //        {
    //            return;
    //        }
    //        this.< Name > k__BackingField = value;
    //        this.OnPropertyChanged("FullName");
    //        this.OnPropertyChanged("Name");
    //    }
    //}

    //public string FullName { get { return string.Format("Anders {0}", this.Name); } }

    //public PersonFD()
    //{
    //    this.< Name > k__BackingField = "Hejlsberg";
    //    base..ctor();
    //}
    //}
    #endregion
}
