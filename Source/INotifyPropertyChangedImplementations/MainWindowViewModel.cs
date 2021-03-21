using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive;
using Reactive.Bindings.Extensions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace INotifyPropertyChangedImplementations
{
    class MainWindowViewModel
    {
        public object Person { get; set; }
        //= new PersonNN();  //通知なし
        //= new Person3();  //C#3版
        //= new Person5();    //C#5版
        //= new Person6();  //C#6版
        = new Person7();  //C#7版
        //= new PersonVM(); //独自ViewModel継承版
        //= new PersonNB(); //独自ViewModel継承バッキングフィールド無し版
        //= new PersonVM(); //独自ViewModel継承版
        //= new PersonEx(); //拡張メソッド使用版
        //= new PersonMV(); //MVVMライブラリ使用版
        //= new PersonRP(); //ReactiveProperty版
        //= new FodyPerson.PersonFD(); //Fody使用版
    }
    #region 通知なし
    public class PersonNN
    {
        public string Name { get; set; } = "Hejlsberg";

        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region C#3版
    public class Person3 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Name = "Hejlsberg";
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value == _Name)
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
                RaisePropertyChanged("FullName");
            }
        }

        public string FullName
        {
            get { return "Anders " + Name; }
        }
    }
    #endregion

    #region C#5版
    public class Person5 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Name = "Hejlsberg";
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value == _Name)
                    return;
                _Name = value;
                RaisePropertyChanged();
                RaisePropertyChanged("FullName");
            }
        }

        public string FullName
        {
            get { return "Anders " + Name; }
        }
    }
    #endregion

    #region C#6版
    public class Person6 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _Name = "Hejlsberg";
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value == _Name)
                    return;
                _Name = value;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }

        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region C#7版
    public class Person7 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string _Name = "Hejlsberg";
        public string Name
        {
            get => _Name;
            set
            {
                if (value == _Name)
                    return;
                _Name = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FullName));
            }
        }

        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region C# Future?版
#if X
public class PersonX : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public string Name
    {
        get;
        set
        {
            if (value == field)
                return;
            field = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FullName));
        }
    } = "Anders";

    public string FullName => $"{Name} Hejlsberg";
}
    public static class PersonExt
    {
        public static void NotifyPropertyChanged(this PersonX person, [CallerMemberName]string propertyName = null)
            => person.PropertyChanged?.Invoke(person, new PropertyChangedEventArgs(propertyName));
    }
#endif
    #endregion

    #region 独自ViewModel継承版
    public class MyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// 前と値が違うなら変更してイベントを発行する
        /// </summary>
        /// <typeparam name="TResult">プロパティの型</typeparam>
        /// <param name="source">元の値</param>
        /// <param name="value">新しい値</param>
        /// <param name="propertyName">プロパティ名/param>
        /// <returns>値の変更有無</returns>
        protected bool RaisePropertyChangedIfSet<TResult>(ref TResult source, TResult value, [CallerMemberName]string propertyName = null)
        {
            //値が同じだったら何もしない
            if (EqualityComparer<TResult>.Default.Equals(source, value))
                return false;

            source = value;
            //イベント発行
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
    public class PersonVM : MyViewModel
    {

        private string _Name = "Hejlsberg";
        public string Name
        {
            get => _Name;
            set
            {
                if (RaisePropertyChangedIfSet(ref _Name, value))
                    RaisePropertyChanged(nameof(FullName));
            }
        }

        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region 独自ViewModel継承 バッキングフィールド無し版
    public class MyViewModelNobackingField : MyViewModel
    {
        //プロパティ名をKeyとしたバッキングフィールド代わりのDictionary
        private Dictionary<string, object> currentPropertyValues = new Dictionary<string, object>();

        /// <summary>
        /// 現在のプロパティ値を取得
        /// </summary>
        protected TResult GetPropertyValue<TResult>([CallerMemberName]string propertyName = null)
            //プロパティの型の既定値を初期値とする
            => GetPropertyValue(default(TResult), propertyName);

        /// <summary>
        /// 現在のプロパティ値を取得
        /// </summary>
        /// <param name="initialValue">初期値</param>
        protected TResult GetPropertyValue<TResult>(TResult initialValue, [CallerMemberName]string propertyName = null)
        {
            //キーに値が無かったら初期値を現在値に入力
            if (!currentPropertyValues.ContainsKey(propertyName))
                currentPropertyValues[propertyName] = initialValue;

            //Dictionaryから現在値を取得してプロパティの型にアンボクシングする
            return (TResult)currentPropertyValues[propertyName];
        }

        /// <summary>
        /// 前と値が違うなら変更してイベントを発行する
        /// </summary>
        /// <param name="value">新しい値</param>
        /// <returns>値の変更有無</returns>
        protected bool RaisePropertyChangedIfSet<TResult>(TResult value, [CallerMemberName]string propertyName = null)
        {
            //値が同じだったら何もしない
            if (EqualityComparer<TResult>.Default.Equals(GetPropertyValue<TResult>(propertyName), value))
                return false;

            //プロパティの現在値に入力
            currentPropertyValues[propertyName] = value;
            //イベント発行
            RaisePropertyChanged(propertyName);
            return true;
        }
    }

    public class PersonNB : MyViewModelNobackingField
    {
        public string Name
        {
            get => GetPropertyValue(initialValue: "Hejlsberg");
            set
            {
                if (RaisePropertyChangedIfSet(value))
                    RaisePropertyChanged(nameof(FullName));
            }
        }

        public List<int> Indexes
        {
            get => GetPropertyValue(new List<int> { 0, 1, 2 });
            set => RaisePropertyChangedIfSet(value);
        }


        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region 拡張メソッド版
    public static class PropertyChangedEventHandlerExtensions
    {
        /// <summary>
        /// イベントを発行する
        /// </summary>
        /// <typeparam name="TResult">プロパティの型</typeparam>
        /// <param name="_this">イベントハンドラ</param>
        /// <param name="propertyName">プロパティ名を表すExpression。() => Nameのように指定する。</param>
        public static void Raise<TResult>(this PropertyChangedEventHandler _this, Expression<Func<TResult>> propertyName)
        {
            // ハンドラに何も登録されていない場合は何もしない
            if (_this == null) return;

            // ラムダ式のBodyを取得する。MemberExpressionじゃなかったら駄目
            if (!(propertyName.Body is MemberExpression memberEx))
                throw new ArgumentException();

            // () => NameのNameの部分の左側に暗黙的に存在しているオブジェクトを取得する式をゲット
            // ConstraintExpressionじゃないと駄目
            if (!(memberEx.Expression is ConstantExpression senderExpression))
                throw new ArgumentException();

            // ○：定数なのでValueプロパティからsender用のインスタンスを得る
            var sender = senderExpression.Value;

            // 下準備が出来たので、イベント発行！！
            _this(sender, new PropertyChangedEventArgs(memberEx.Member.Name));
        }

        /// <summary>
        /// 前と値が違うなら変更してイベントを発行する
        /// </summary>
        /// <typeparam name="TResult">プロパティの型</typeparam>
        /// <param name="_this">イベントハンドラ</param>
        /// <param name="propertyName">プロパティ名を表すExpression。() => Nameのように指定する。</param>
        /// <param name="source">元の値</param>
        /// <param name="value">新しい値</param>
        /// <returns>値の変更有無</returns>
        public static bool RaiseIfSet<TResult>(this PropertyChangedEventHandler _this, Expression<Func<TResult>> propertyName, ref TResult source, TResult value)
        {
            //値が同じだったら何もしない
            if (EqualityComparer<TResult>.Default.Equals(source, value))
                return false;

            source = value;
            //イベント発行
            Raise(_this, propertyName);
            return true;
        }
    }

    public class PersonEx : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Name = "Hejlsberg";
        public string Name
        {
            get => _Name;
            set
            {
                if (PropertyChanged.RaiseIfSet(() => Name, ref _Name, value))
                    PropertyChanged.Raise(() => FullName);
            }
        }

        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region MVVMライブラリで継承版
    public class PersonMV : GalaSoft.MvvmLight.ViewModelBase
    {
        private string _Name = "Hejlsberg";
        public string Name
        {
            get => _Name;
            set
            {
                if (Set(ref _Name, value))
                    RaisePropertyChanged(nameof(FullName));
            }
        }

        public string FullName => $"Anders {Name}";
    }
    #endregion

    #region ReactiveProperty版
    public class PersonRP
    {
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>("Hejlsberg");

        public ReadOnlyReactiveProperty<string> FullName { get; }

        public PersonRP()
        {
            FullName = Name
                .Select(x => $"Anders {x}")
                .ToReadOnlyReactiveProperty();
        }
    }
    #endregion
}
