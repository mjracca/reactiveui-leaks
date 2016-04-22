using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ReactiveUI;
using Splat;

//#if UNIFIED && UIKIT
using UIKit;
using Foundation;
//#elif UNIFIED && COCOA
//using Foundation;
//#elif UIKIT
//using MonoTouch.UIKit;
//using MonoTouch.Foundation;
//#else
//using MonoMac.Foundation;
//#endif
using System.Collections.Generic;

namespace ReactiveUI
{
    /// <summary>
    /// This class provides notifications for Cocoa Framework objects based on
    /// Key-Value Observing. Unfortunately, this class is a bit Tricky™, because
    /// of the caveat mentioned below - there is no way up-front to be able to
    /// tell whether a given property on an object is Key-Value Observable, we 
    /// only have to hope for the best :-/
    /// </summary>
    public class KVONotReallyObservableForProperty : ICreatesObservableForProperty
    {
        static readonly MemoizingMRUCache<Tuple<Type, string>, bool> declaredInNSObject;

        static KVONotReallyObservableForProperty()
        {
            var monotouchAssemblyName = typeof(NSObject).Assembly.FullName;

            declaredInNSObject = new MemoizingMRUCache<Tuple<Type, string>, bool>((pair, _) => {
                var thisType = pair.Item1;

                // Types that aren't NSObjects at all are uninteresting to us
                if (typeof(NSObject).IsAssignableFrom(thisType) == false) {
                    return false;
                }

                while(thisType != null) {
                    if (thisType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Any(x => x.Name == pair.Item2)) {
                        // NB: This is a not-completely correct way to detect if
                        // an object is defined in an Obj-C class (it will fail if
                        // you're using a binding to a 3rd-party Obj-C library).
                        return thisType.Assembly.FullName == monotouchAssemblyName;
                    }

                    thisType = thisType.BaseType;
                }

                // The property doesn't exist at all
                return false;
            }, RxApp.BigCacheLimit);
        }

        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            lock (declaredInNSObject) {
                return declaredInNSObject.Get(Tuple.Create(type, propertyName)) ? 30 : 0;
            }
        }

        static readonly Dictionary<Type, bool> hasWarned = new Dictionary<Type, bool>();
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var type = sender.GetType();
            if (!hasWarned.ContainsKey(type)) {
                this.Log().Warn(
                    "{0} is a POCO type and won't send change notifications, WhenAny will only return a single value!",
                    type.FullName);
                hasWarned[type] = true;
            }

            return Observable.Return(new ObservedChange<object, object>(sender, expression), RxApp.MainThreadScheduler)
                .Concat(Observable.Never<IObservedChange<object, object>>());
        }
    }
}