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
        protected static KeyValuePair<bool?, string>[] CreateBoolPairs(
            string trueString = null, 
            string falseString = null, 
            string nullString = null)
        {
            if (trueString ==null) { trueString = Strings.DropDownList_True; }
            if (falseString ==null) { falseString = Strings.DropDownList_False; }
            if (nullString ==null) { nullString = Strings.DropDownList_PleaseSelect; }
            return new KeyValuePair<bool?,string>[]
            {
                new KeyValuePair<bool?,string>((bool?)null,nullString),
                new KeyValuePair<bool?,string>(true,trueString),
                new KeyValuePair<bool?,string>(false,falseString)
            };
        }
        protected static IEnumerable<KeyValuePair<T, string>> EnumToListOptions<T>() where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            string enumName = enumType.Name + '_';
            var culture = CultureInfo.CurrentUICulture;
            var resourceManager = Strings.ResourceManager;
            return Enum.GetValues(enumType)
                .Cast<T>()
                .Select(t=>
                    {
                        string st = t.ToString();
                        st = (st=="Missing")
                            ?Strings.DropDownList_PleaseSelect
                            :resourceManager.GetString(enumName + st, culture);
                        return new KeyValuePair<T, string>(t, st);
                    });
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
}