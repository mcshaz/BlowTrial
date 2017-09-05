using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public abstract class NotifyChangeBase : INotifyPropertyChanged
    {
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
            VerifyNoDuplicatePropertyNames(propertyNames);
            foreach (var propertyName in propertyNames)
            {
                if (PropertyChanged != null) //on the inside of the loop in case a handler detaches itself
                {
                    VerifyPropertyName(propertyName);
                    var e = new PropertyChangedEventArgs(propertyName);
                    PropertyChanged(this, e);
                }
            }
        }
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        void VerifyNoDuplicatePropertyNames(params string[] propertyNames)
        {
            // Verify that the property name are not repeated
            List<String> duplicates = (from g in propertyNames.GroupBy(x => x)
                                        where g.Count() > 1
                                        select g.Key).ToList();
            if (duplicates.Any())
            {
                string msg = "Repeated notify on property :" + string.Join(",", duplicates);
                Debug.Fail(msg);
            }
        }

        #endregion // INotifyPropertyChanged Members
    }
}
