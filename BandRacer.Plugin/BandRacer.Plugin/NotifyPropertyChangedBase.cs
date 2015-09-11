using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TocaBoca.Platform.Data
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        // *** Events ***

        public event PropertyChangedEventHandler PropertyChanged;

        // *** Protected Methods ***
#if NETFX_CORE
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
#else
        protected virtual void OnPropertyChanged(string propertyName)
#endif
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = GetPropertyName(propertyExpression);
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;

            if (eventHandler != null)
                eventHandler(this, e);
        }

        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> propertyExpression)
        {
            string propertyName = GetPropertyName(propertyExpression);
            return SetProperty<T>(ref storage, value, propertyName);
        }

#if NETFX_CORE
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
#else
        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
#endif
        {
            // If the value hasn't changed then just return false

            if (object.Equals(storage, value))
                return false;

            // Set the property and raise the property changed notification

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        // *** Private Methods ***

        private string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            // Validate arguments

            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("ShouldBeAMemberAccessLambdaExpression", "propertyExpression");

            // Extract the property name

            MemberExpression memberExpression = (MemberExpression)propertyExpression.Body;
            return memberExpression.Member.Name;
        }
    }
}
