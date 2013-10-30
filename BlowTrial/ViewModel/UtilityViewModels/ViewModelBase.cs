using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using BlowTrial.Infrastructure.Extensions;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace BlowTrial.ViewModel
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
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

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void NotifyPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                this.VerifyPropertyName(propertyName);

                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    handler(this, e);
                }
            }
        }

        #endregion // INotifyPropertyChanged Members

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