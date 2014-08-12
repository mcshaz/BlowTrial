using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace BlowTrial.ViewModel
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase :NotifyChangeBase
    {
        #region Constructor

        protected ViewModelBase()
        {
        }

        #endregion // Constructor

        #region Utilities
        /// <summary>
        /// Returns an array of KayValuePairs for use in a combo box. Default values are from the Strings.DropDownList_ resx.
        /// </summary>
        /// <param name="trueString">string to associate with true</param>
        /// <param name="falseString">string to associate with false</param>
        /// <param name="nullString">string to associate with null</param>
        /// <returns></returns>
        protected static KeyDisplayNamePair<bool?>[] CreateBoolPairs(
            string trueString = null, 
            string falseString = null, 
            string nullString = null)
        {
            if (trueString ==null) { trueString = Strings.DropDownList_True; }
            if (falseString ==null) { falseString = Strings.DropDownList_False; }
            if (nullString ==null) { nullString = Strings.DropDownList_PleaseSelect; }
            return new KeyDisplayNamePair<bool?>[]
            {
                new KeyDisplayNamePair<bool?>((bool?)null,nullString),
                new KeyDisplayNamePair<bool?>(true,trueString),
                new KeyDisplayNamePair<bool?>(false,falseString)
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">must be an enum</typeparam>
        /// <returns></returns>
        protected static IList<KeyDisplayNamePair<T>> EnumToListOptions<T>(T? toPleaseSelect, params T[] excludeFromList) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
#if DEBUG
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type supplied must be an enum");
            }
#endif
            string enumName = enumType.Name + '_';
            var culture = CultureInfo.CurrentUICulture;
            var resourceManager = Strings.ResourceManager;
            return 
                (from e in Enum.GetValues(enumType).Cast<T>()
                 where !excludeFromList.Contains(e)
                 select new KeyDisplayNamePair<T>(e,
                    toPleaseSelect.Equals(e)
                        ? Strings.DropDownList_PleaseSelect
                        : resourceManager.GetString(enumName + e.ToString(), culture)))
                .ToList();
        }
        protected static IList<KeyDisplayNamePair<T>> EnumToListOptions<T>() where T : struct, IConvertible
        {
            return EnumToListOptions((T?)default(T));
        }

        protected static IList<KeyDisplayNamePair<T?>> NullableEnumToListOptions<T>() where T : struct, IConvertible
        {
            List<KeyDisplayNamePair<T?>> returnVar = new List<KeyDisplayNamePair<T?>>();
            returnVar.Add(new KeyDisplayNamePair<T?>((T?)null, Strings.DropDownList_PleaseSelect));
            returnVar.AddRange(EnumToListOptions((T?)null).Select(o=>new KeyDisplayNamePair<T?>((T?)o.Key,o.Value)));
            return returnVar;
        }

        protected static System.Security.Principal.IPrincipal GetCurrentPrincipal()
        {
            return System.Threading.Thread.CurrentPrincipal;
        }
        #endregion

        #region DisplayName
        private string _displayName;
        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName
        {
            get { return _displayName; }
            protected set
            {
                if (_displayName == value) { return; }
                _displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }

        #endregion // DisplayName

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif
    }

    public class KeyDisplayNamePair<T>
    {
        public KeyDisplayNamePair() { }
        public KeyDisplayNamePair(T key, string displayName)
        {
            Key = key;
            Value = displayName;
        }
        public T Key { get; set; }
        public string Value { get; set; }
    }
}